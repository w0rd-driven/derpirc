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
            _unitOfWork = unitOfWork;
        }

        public void AttachLocalUser(IrcLocalUser localUser)
        {
            localUser.JoinedChannel += LocalUser_JoinedChannel;
            localUser.LeftChannel += LocalUser_LeftChannel;
        }

        public void DetachLocalUser(IrcLocalUser localUser)
        {
            localUser.JoinedChannel -= LocalUser_JoinedChannel;
            localUser.LeftChannel -= LocalUser_LeftChannel;
        }

        public void SendMessage(ChannelItem message)
        {
            if (message != null)
            {
                var summary = message.Summary;
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);
                localUser.SendMessage(summary.Name, message.Text);

                // Add the source and MessageType at the last minute
                message.Source = localUser.NickName;
                message.Type = MessageType.Mine;

                summary.Messages.Add(message);
                DataUnitOfWork.Default.Commit();
                //_unitOfWork.Commit();
                var eventArgs = new MessageItemEventArgs()
                {
                    NetworkId = summary.NetworkId,
                    SummaryId = summary.Id,
                    MessageId = message.Id,
                };
                OnChannelItemReceived(eventArgs);
            }
        }

        public void SendMessage(MentionItem message)
        {
            if (message != null)
            {
                var summary = message.Summary;
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);
                //localUser.SendMessage(summary.ChannelName, message.Text);

                // Add the source and MessageType at the last minute
                message.Source = localUser.NickName;
                message.Type = MessageType.Mine;

                summary.Messages.Add(message);
                DataUnitOfWork.Default.Commit();
                //_unitOfWork.Commit();
                var eventArgs = new MessageItemEventArgs()
                {
                    NetworkId = summary.NetworkId,
                    SummaryId = summary.Id,
                    MessageId = message.Id,
                };
                OnMentionItemReceived(eventArgs);
            }
        }

        private void LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            e.Channel.MessageReceived += Channel_MessageReceived;
            e.Channel.NoticeReceived += Channel_NoticeReceived;
            JoinChannel(e.Channel);
        }

        private void LocalUser_LeftChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            e.Channel.MessageReceived -= Channel_MessageReceived;
            e.Channel.NoticeReceived -= Channel_NoticeReceived;
            LeaveChannel(e.Channel);
        }

        private void Channel_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            var messageType = MessageType.Theirs;
            if (e.Source is IrcUser)
            {
                messageType = MessageType.Theirs;
            }

            var isMention = e.Text.Contains(channel.Client.LocalUser.NickName);
            if (isMention)
            {
                var summary = GetMentionSummary(e.Source as IrcUser, channel);
                var message = GetIrcMessage(summary, e, messageType);
                summary.Messages.Add(message);
                DataUnitOfWork.Default.Commit();
                //_unitOfWork.Commit();
                var eventArgs = new MessageItemEventArgs()
                {
                    NetworkId = summary.NetworkId,
                    SummaryId = summary.Id,
                    MessageId = message.Id,
                };
                OnMentionItemReceived(eventArgs);
            }
            else
            {
                var summary = GetChannelSummary(channel);
                var message = GetIrcMessage(summary, e, messageType);
                summary.Messages.Add(message);
                DataUnitOfWork.Default.Commit();
                //_unitOfWork.Commit();
                var eventArgs = new MessageItemEventArgs()
                {
                    NetworkId = summary.NetworkId,
                    SummaryId = summary.Id,
                    MessageId = message.Id,
                };
                OnChannelItemReceived(eventArgs);
            }
        }

        private void Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            //OnChannelNoticeReceived(channel, e);
        }

        private void JoinChannel(IrcChannel channel)
        {
            var channelSummary = GetChannelSummary(channel);
            var eventArgs = new ChannelStatusEventArgs()
            {
                SummaryId = channelSummary.Id,
                Status = ChannelStatusType.Join,
            };
            OnChannelJoined(eventArgs);
        }

        private void LeaveChannel(IrcChannel channel)
        {
            var channelSummary = GetChannelSummary(channel);
            var eventArgs = new ChannelStatusEventArgs()
            {
                SummaryId = channelSummary.Id,
                Status = ChannelStatusType.Leave,
            };
            OnChannelLeft(eventArgs);
        }

        private Channel GetChannelSummary(IrcChannel channel)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(channel.Client);
            var result = network.Channels.FirstOrDefault(x => x.Name == channel.Name.ToLower());
            if (result == null)
            {
                result = new Channel() { Name = channel.Name.ToLower() };
                network.Channels.Add(result);
                DataUnitOfWork.Default.Commit();
                //_unitOfWork.Commit();
            }
            return result;
        }

        private Mention GetMentionSummary(IrcUser user, IrcChannel channel)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(user.Client);
            var result = network.Mentions.FirstOrDefault(x => x.Name == user.NickName.ToLower());
            if (result == null)
            {
                result = new Mention() { Name = user.NickName.ToLower(), ChannelName = channel.Name.ToLower() };
                network.Mentions.Add(result);
                DataUnitOfWork.Default.Commit();
                //_unitOfWork.Commit();
            }
            return result;
        }

        private ChannelItem GetIrcMessage(Channel summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new ChannelItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            result.Summary = summary;
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
            result.Summary = summary;
            if (eventArgs.Targets.Count >= 1)
                result.Source = eventArgs.Targets[0].Name;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
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
