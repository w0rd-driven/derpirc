using System;
using System.Linq;
using System.Text.RegularExpressions;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;

namespace derpirc.Core
{
    public class ChannelsSupervisor : IDisposable
    {
        private bool _isDisposed;

        private DataUnitOfWork _unitOfWork;

        // Regex for splitting space-separated list of command parts until first parameter that begins with '/'.
        private static readonly Regex commandPartsSplitRegex = new Regex("(?<! /.*) ", RegexOptions.Compiled);

        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<MessageItemEventArgs> ChannelItemReceived;
        public event EventHandler<MessageItemEventArgs> MentionItemReceived;

        public ChannelsSupervisor(DataUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public void AttachLocalUser(IrcLocalUser localUser)
        {
            if (localUser != null)
            {
                localUser.JoinedChannel += this.LocalUser_JoinedChannel;
                localUser.LeftChannel += this.LocalUser_LeftChannel;
                // localUser.InviteReceived
            }
        }

        public void DetachLocalUser(IrcLocalUser localUser)
        {
            if (localUser != null)
            {
                localUser.JoinedChannel -= this.LocalUser_JoinedChannel;
                localUser.LeftChannel -= this.LocalUser_LeftChannel;
                // localUser.InviteReceived
            }
        }

        #region UI-facing methods

        public void SendMessage(ChannelItem message)
        {
            if (message != null)
            {
                var summary = message.Summary;
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);

                // TODO: Error check: If localUser == null, alert UI/internals
                if (localUser != null)
                {
                    localUser.SendMessage(summary.Name, message.Text);

                    // Add the source and MessageType at the last minute
                    message.Source = localUser.NickName;
                    message.Type = MessageType.Mine;

                    summary.Messages.Add(message);
                    this._unitOfWork.Commit();

                    // HACK: Use MessageSent event for this eventually
                    var eventArgs = new MessageItemEventArgs()
                    {
                        NetworkId = summary.NetworkId,
                        SummaryId = summary.Id,
                        MessageId = message.Id,
                    };
                    this.OnChannelItemReceived(eventArgs);
                }
            }
        }

        public void SendMessage(MentionItem message)
        {
            if (message != null)
            {
                var summary = message.Summary;
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);

                // TODO: Error check: If localUser == null, alert UI/internals
                if (localUser != null)
                {
                    localUser.SendMessage(summary.ChannelName, GetMentionText(summary.Name, message.Text));

                    // Add the source and MessageType at the last minute
                    message.Source = localUser.NickName;
                    message.Type = MessageType.Mine;

                    summary.Messages.Add(message);
                    this._unitOfWork.Commit();

                    // HACK: Use MessageSent event for this eventually
                    var eventArgs = new MessageItemEventArgs()
                    {
                        NetworkId = summary.NetworkId,
                        SummaryId = summary.Id,
                        MessageId = message.Id,
                    };
                    this.OnMentionItemReceived(eventArgs);
                }
            }
        }

        public void SetTopic(IMessage target, string topic)
        {
            var channel = SupervisorFacade.Default.GetIrcChannelBySummary(target);
            if (channel != null && !string.IsNullOrEmpty(topic))
                channel.SetTopic(topic);
        }

        public void SetModes(IMessage target, string modes)
        {
            var channel = SupervisorFacade.Default.GetIrcChannelBySummary(target);
            if (channel != null && !string.IsNullOrEmpty(modes))
                channel.SetModes(modes);
        }

        #endregion

        private void LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            e.Channel.MessageReceived += this.Channel_MessageReceived;
            e.Channel.NoticeReceived += this.Channel_NoticeReceived;
            e.Channel.TopicChanged += this.Channel_TopicChanged;
            e.Channel.ModesChanged += this.Channel_ModesChanged;
            this.JoinChannel(e.Channel);
        }

        private void LocalUser_LeftChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            e.Channel.MessageReceived -= this.Channel_MessageReceived;
            e.Channel.NoticeReceived -= this.Channel_NoticeReceived;
            e.Channel.TopicChanged -= this.Channel_TopicChanged;
            e.Channel.ModesChanged -= this.Channel_ModesChanged;
            this.LeaveChannel(e.Channel);
        }

        private void Channel_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            var messageType = MessageType.Theirs;
            if (e.Source is IrcUser)
            {
                messageType = MessageType.Theirs;
            }

            // Mentions are a simple string parse
            var isMention = e.Text.Contains(channel.Client.LocalUser.NickName);
            if (isMention)
            {
                // TODO: Error check: If summary == null, alert UI/internals
                var summary = this.GetMentionSummary(channel, e.Source as IrcUser);
                if (summary != null)
                {
                    // HACK: This should never be null but just in case, keep an eye on it
                    var message = this.GetIrcMessage(summary, e, messageType);
                    summary.Messages.Add(message);
                    this._unitOfWork.Commit();

                    var eventArgs = new MessageItemEventArgs()
                    {
                        NetworkId = summary.NetworkId,
                        SummaryId = summary.Id,
                        MessageId = message.Id,
                    };
                    this.OnMentionItemReceived(eventArgs);
                }
            }
            else
            {
                // TODO: Error check: If summary == null, alert UI/internals
                var summary = this.GetChannelSummary(channel);
                if (summary != null)
                {
                    // HACK: This should never be null but just in case, keep an eye on it
                    var message = this.GetIrcMessage(summary, e, messageType);
                    summary.Messages.Add(message);
                    this._unitOfWork.Commit();

                    var eventArgs = new MessageItemEventArgs()
                    {
                        NetworkId = summary.NetworkId,
                        SummaryId = summary.Id,
                        MessageId = message.Id,
                    };
                    this.OnChannelItemReceived(eventArgs);
                }
            }
        }

        private void Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            //OnChannelNoticeReceived(channel, e);
        }

        private void Channel_TopicChanged(object sender, IrcUserEventArgs e)
        {
            var channel = sender as IrcChannel;
        }

        private void Channel_ModesChanged(object sender, IrcUserEventArgs e)
        {
            var channel = sender as IrcChannel;
        }

        private void JoinChannel(IrcChannel channel)
        {
            // TODO: Error check: If summary == null, alert UI/internals
            var summary = this.GetChannelSummary(channel);
            if (summary != null)
            {
                var eventArgs = new ChannelStatusEventArgs()
                {
                    SummaryId = summary.Id,
                    Status = ChannelStatus.Join,
                };
                this.OnChannelJoined(eventArgs);
            }
        }

        private void LeaveChannel(IrcChannel channel)
        {
            // TODO: Error check: If summary == null, alert UI/internals
            var summary = this.GetChannelSummary(channel);
            if (summary != null)
            {
                var eventArgs = new ChannelStatusEventArgs()
                {
                    SummaryId = summary.Id,
                    Status = ChannelStatus.Leave,
                };
                this.OnChannelLeft(eventArgs);
            }
        }

        #region Events

        private void OnChannelJoined(ChannelStatusEventArgs e)
        {
            var handler = ChannelJoined;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        private void OnChannelLeft(ChannelStatusEventArgs e)
        {
            var handler = ChannelLeft;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        private void OnChannelItemReceived(MessageItemEventArgs e)
        {
            var handler = ChannelItemReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        private void OnMentionItemReceived(MessageItemEventArgs e)
        {
            var handler = MentionItemReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        #endregion

        #region Lookup methods

        private Channel GetChannelSummary(IrcChannel channel)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(channel.Client);
            if (network != null)
            {
                var result = network.Channels.FirstOrDefault(x => x.Name == channel.Name.ToLower());
                if (result == null)
                {
                    result = new Channel() { Name = channel.Name };
                    network.Channels.Add(result);
                    this._unitOfWork.Commit();
                }
                return result;
            }
            return null;
        }

        private Mention GetMentionSummary(IrcChannel channel, IrcUser user)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(channel.Client);
            if (network != null)
            {
                var result = network.Mentions.FirstOrDefault(x => x.Name == user.NickName.ToLower());
                if (result == null)
                {
                    result = new Mention() { Name = user.NickName, ChannelName = channel.Name };
                    network.Mentions.Add(result);
                    this._unitOfWork.Commit();
                }
                return result;
            }
            return null;
        }

        private ChannelItem GetIrcMessage(Channel summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new ChannelItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            result.Source = eventArgs.Source.Name;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
        }

        private MentionItem GetIrcMessage(Mention summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new MentionItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            // This is technically a target but why bother changing one property for one IMessage?
            if (eventArgs.Targets.Count >= 1)
                result.Source = eventArgs.Targets[0].Name;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
        }

        private string GetMentionText(string name, string message)
        {
            var result = message;
            // TODO: UI setting for mention char string. Default to ":"
            if (!string.IsNullOrEmpty(name))
                result = name + ": " + message;
            return result;
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                }
            }
            this._isDisposed = true;
        }
    }
}
