using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;

namespace derpirc.Core
{
    public class LocalUserSupervisor : IDisposable
    {
        private bool _isDisposed;

        private DataUnitOfWork _unitOfWork;

        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<MessageItemEventArgs> ChannelItemReceived;
        public event EventHandler<MessageItemEventArgs> MentionItemReceived;
        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        public LocalUserSupervisor(DataUnitOfWork unitOfWork)
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

                localUser.MessageReceived += this.LocalUser_MessageReceived;
                localUser.MessageSent += this.LocalUser_MessageSent;
                localUser.NoticeReceived += this.LocalUser_NoticeReceived;
                localUser.NoticeSent += this.LocalUser_NoticeSent;
                localUser.NickNameChanged += this.LocalUser_NickNameChanged;
            }
        }

        public void DetachLocalUser(IrcLocalUser localUser)
        {
            if (localUser != null)
            {
                localUser.JoinedChannel -= this.LocalUser_JoinedChannel;
                localUser.LeftChannel -= this.LocalUser_LeftChannel;
                localUser.InviteReceived -= this.LocalUser_InviteReceived;

                localUser.MessageReceived -= this.LocalUser_MessageReceived;
                localUser.MessageSent -= this.LocalUser_MessageSent;
                localUser.NoticeReceived -= this.LocalUser_NoticeReceived;
                localUser.NoticeSent -= this.LocalUser_NoticeSent;
                localUser.NickNameChanged -= this.LocalUser_NickNameChanged;
            }
        }

        #region UI-facing methods

        public void SendMessage(IMessage summary, string text)
        {
            var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);

            if (localUser != null)
            {
                // TODO: Recovery : Message not sent
                if (summary is Channel)
                {
                    localUser.SendMessage(summary.Name, text);
                    AddMessage(localUser.Client, summary.Name, localUser.NickName, text, Owner.Me);
                }
                if (summary is Mention)
                {
                    var concreteSummary = summary as Mention;
                    if (concreteSummary != null)
                    {
                        var channelName = concreteSummary.ChannelName;
                        var mentionText = GetMentionText(summary.Name, text);
                        localUser.SendMessage(channelName, mentionText);
                        AddMessage(localUser.Client, channelName, summary.Name, mentionText, Owner.Me);
                    }
                }
                if (summary is Message)
                {
                    localUser.SendMessage(summary.Name, text);
                    AddMessage(localUser.Client, null, summary.Name, text, Owner.Me);
                }
            }
        }

        public void SendNotice(IMessage summary, string text)
        {
            var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);

            if (localUser != null)
            {
                // TODO: Recovery : Notice not sent
                localUser.SendNotice(summary.Name, text);
                AddMessage(localUser.Client, null, localUser.NickName, text, Owner.Me);
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

        #region Subscriber Events

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
            var text = e.Text;
            var type = Owner.Them;

            AddMessage(channel.Client, channel.Name, ircUser.NickName, text, type);
        }

        private void Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            var ircUser = e.Source as IrcUser;
            var text = e.Text;
            var type = Owner.Them;

            AddMessage(channel.Client, channel.Name, ircUser.NickName, text, type);
        }

        private void Channel_TopicChanged(object sender, IrcUserEventArgs e)
        {
            var channel = sender as IrcChannel;
            UpdateTopic(channel);
        }

        private void Channel_ModesChanged(object sender, IrcUserEventArgs e)
        {
            var channel = sender as IrcChannel;
        }

        private void LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            var ircUser = e.Source as IrcUser;
            var text = e.Text;
            var type = Owner.Them;

            AddMessage(ircUser.Client, null, ircUser.NickName, text, type);
        }

        private void LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            var ircUser = e.Source as IrcUser;
            var text = e.Text;
            var type = Owner.Them;

            //AddMessage(ircUser.Client, null, ircUser.NickName, text, owner);
        }

        private void LocalUser_MessageSent(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            var targets = e.Targets;
            foreach (var item in targets)
            {
                var record = item;
            }
            //OnLocalUserMessageSent(localUser, e);
        }

        private void LocalUser_NoticeSent(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            //OnLocalUserNoticeSent(localUser, e);
        }

        private void LocalUser_NickNameChanged(object sender, EventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            //OnLocalUserNickNameChanged(localUser, e);
        }

        #endregion

        private void JoinChannel(IrcChannel channel)
        {
            var summary = this.GetChannel(channel.Client, channel.Name);
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
            var summary = this.GetChannel(channel.Client, channel.Name);
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

        private void AddMessage(IrcClient client, string channelName, string nickName, string text, Owner owner)
        {
            var isRead = owner == Owner.Me ? true : false;
            if (!string.IsNullOrEmpty(channelName))
            {
                if (text.Contains(client.LocalUser.NickName) || text.Contains(nickName))
                {
                    // Mention
                    var summary = this.GetMention(client, channelName, nickName);
                    if (summary != null)
                    {
                        var message = new MentionItem()
                        {
                            Source = channelName,
                            Text = text,
                            Owner = owner,
                            Timestamp = DateTime.Now,
                            IsRead = isRead,
                        };
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
                    // Channel
                    var summary = this.GetChannel(client, channelName);
                    if (summary != null)
                    {
                        var message = new ChannelItem()
                        {
                            Source = nickName,
                            Text = text,
                            Owner = owner,
                            Timestamp = DateTime.Now,
                            IsRead = isRead,
                        };
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
            else
            {
                // Message
                var summary = this.GetMessage(client, nickName);
                if (summary != null)
                {
                    var message = new MessageItem()
                    {
                        Source = nickName,
                        Text = text,
                        Owner = owner,
                        Timestamp = DateTime.Now,
                        IsRead = isRead,
                    };
                    summary.Messages.Add(message);
                    this._unitOfWork.Commit();

                    var eventArgs = new MessageItemEventArgs()
                    {
                        NetworkId = summary.NetworkId,
                        SummaryId = summary.Id,
                        MessageId = message.Id,
                    };
                    this.OnMessageItemReceived(eventArgs);
                }
            }
        }

        private void UpdateTopic(IrcChannel channel)
        {
            var summary = this.GetChannel(channel.Client, channel.Name);
            if (summary != null)
            {
                summary.Topic = channel.Topic;
                _unitOfWork.Commit();
            }
        }

        #region Lookup methods

        private Channel GetChannel(IrcClient client, string channelName)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(client);
            if (network != null)
            {
                var result = network.Channels.FirstOrDefault(x => x.Name == channelName.ToLower());
                if (result == null)
                {
                    result = new Channel() { Name = channelName };
                    network.Channels.Add(result);
                    this._unitOfWork.Commit();
                }
                return result;
            }
            return null;
        }

        private Mention GetMention(IrcClient client, string channelName, string nickName)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(client);
            if (network != null)
            {
                var result = network.Mentions.FirstOrDefault(x => x.Name == nickName.ToLower());
                if (result == null)
                {
                    result = new Mention() { Name = nickName, ChannelName = channelName };
                    network.Mentions.Add(result);
                    this._unitOfWork.Commit();
                }
                return result;
            }
            return null;
        }

        private Message GetMessage(IrcClient client, string nickName)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(client);
            if (network != null)
            {
                var result = network.Messages.FirstOrDefault(x => x.Name == nickName.ToLower());
                if (result == null)
                {
                    result = new Message() { Name = nickName };
                    network.Messages.Add(result);
                    this._unitOfWork.Commit();
                }
                return result;
            }
            return null;
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

        #region Publisher Events

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

        private void OnMessageItemReceived(MessageItemEventArgs e)
        {
            var handler = this.MessageItemReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        #endregion

        public void Dispose()
        {
            this.Dispose(true);
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
