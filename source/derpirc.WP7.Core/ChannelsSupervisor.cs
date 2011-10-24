using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;

namespace derpirc.Core
{
    public class ChannelsSupervisor : IDisposable
    {
        private bool _isDisposed;

        private DataUnitOfWork _unitOfWork;

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
                localUser.InviteReceived += this.LocalUser_InviteReceived;
            }
        }

        public void DetachLocalUser(IrcLocalUser localUser)
        {
            if (localUser != null)
            {
                localUser.JoinedChannel -= this.LocalUser_JoinedChannel;
                localUser.LeftChannel -= this.LocalUser_LeftChannel;
                localUser.InviteReceived -= this.LocalUser_InviteReceived;
            }
        }

        #region UI-facing methods

        public void SendMessage(ChannelItem message)
        {
            if (message != null)
            {
                var summary = message.Summary;
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);

                if (localUser != null)
                {
                    // TODO: Recovery : Message not sent
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

                if (localUser != null)
                {
                    // TODO: Recovery : Message not sent
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

        private void LocalUser_InviteReceived(object sender, IrcChannelInvitationEventArgs e)
        {
            // TODO: UI setting for join on invite. Default to true
            var localUser = sender as IrcLocalUser;
            var isJoinOnInvite = true;
            // TODO: JoinOnInvite creates orphan records because no favorites are added
            if (isJoinOnInvite)
                localUser.Client.Channels.Join(e.Channel.Name);
        }

        private void Channel_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            var ircUser = e.Source as IrcUser;
            var messageText = e.Text;
            var messageType = MessageType.Theirs;

            // Mentions are a simple string parse
            var isMention = e.Text.Contains(channel.Client.LocalUser.NickName);
            AddMessage(channel, ircUser, messageText, messageType, isMention);
        }

        private void Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            var ircUser = e.Source as IrcUser;
            var messageText = e.Text;
            var messageType = MessageType.Theirs;

            // Mentions are a simple string parse
            var isMention = e.Text.Contains(channel.Client.LocalUser.NickName);
            AddMessage(channel, ircUser, messageText, messageType, isMention);
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

        private void AddMessage(IrcChannel channel, IrcUser user, string text, MessageType type, bool isMention)
        {
            // Channel comes first
            if (!isMention)
            {
                var summary = this.GetChannelSummary(channel);
                if (summary != null)
                {
                    var message = this.GetIrcMessage(summary, user.NickName, text, type);
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
            else
            {
                var summary = this.GetMentionSummary(channel, user);
                if (summary != null)
                {
                    var message = this.GetIrcMessage(summary, channel.Name, text, type);
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
        }

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

        private ChannelItem GetIrcMessage(Channel summary, string source, string text, MessageType type)
        {
            var result = new ChannelItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            result.Source = source;
            result.Text = text;
            result.Type = type;
            return result;
        }

        private MentionItem GetIrcMessage(Mention summary, string source, string text, MessageType type)
        {
            var result = new MentionItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            result.Source = source;
            result.Text = text;
            result.Type = type;
            return result;
        }

        private string GetMentionText(string name, string message)
        {
            var result = message;
            // TODO: UI setting for mention char string. Default to ":"
            var mentionChar = ": ";
            if (!string.IsNullOrEmpty(name))
                result = name + mentionChar + message;
            return result;
        }

        #endregion

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
