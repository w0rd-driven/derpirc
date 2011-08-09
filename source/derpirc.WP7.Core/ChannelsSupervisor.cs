using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using derpirc.Data;
using IrcDotNet;
using derpirc.Data.Settings;

namespace derpirc.Core
{
    public class ChannelsSupervisor
    {
        private bool _isDisposed;
        private const int _quitTimeout = 1000;
        private string _quitMessage;
        private DataUnitOfWork _unitOfWork;
        private Session _session;

        // Regex for splitting space-separated list of command parts until first parameter that begins with '/'.
        private static readonly Regex commandPartsSplitRegex = new Regex("(?<! /.*) ", RegexOptions.Compiled);

        private ObservableCollection<IrcChannel> _channels;
        private ObservableCollection<ChannelSummary> _channelSummaries;

        private ObservableCollection<IrcLocalUser> _localUsers;
        public ObservableCollection<IrcLocalUser> LocalUsers
        {
            get { return _localUsers; }
            private set { }
        }

        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<ChannelItemEventArgs> MessageReceived;
        public event EventHandler<MentionItemEventArgs> MentionReceived;

        public ChannelsSupervisor(DataUnitOfWork unitOfWork, Session session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
            _localUsers = new ObservableCollection<IrcLocalUser>();
            _localUsers.CollectionChanged += new NotifyCollectionChangedEventHandler(LocalUsers_CollectionChanged);
            //_channels = new ObservableCollection<IrcChannel>();
            //_channelSummaries = new ObservableCollection<ChannelSummary>();
        }

        private void LocalUsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var newItem = item as IrcLocalUser;
                    newItem.JoinedChannel += LocalUser_JoinedChannel;
                    newItem.LeftChannel += LocalUser_LeftChannel;
                    //newItem.NickNameChanged += LocalUser_NickNameChanged;
                }
            }
        }
        
        private void LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            e.Channel.UserJoined += Channel_UserJoined;
            e.Channel.UserLeft += Channel_UserLeft;
            e.Channel.MessageReceived += Channel_MessageReceived;
            e.Channel.NoticeReceived += Channel_NoticeReceived;
            JoinChannel(e.Channel);
        }

        private void LocalUser_LeftChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            e.Channel.UserJoined -= Channel_UserJoined;
            e.Channel.UserLeft -= Channel_UserLeft;
            e.Channel.MessageReceived -= Channel_MessageReceived;
            e.Channel.NoticeReceived -= Channel_NoticeReceived;
            LeaveChannel(e.Channel);
        }

        private void Channel_UserJoined(object sender, IrcChannelUserEventArgs e)
        {
            var channel = sender as IrcChannel;
            // TODO: Update UserList
            //OnChannelUserLeft(channel, e);
        }

        private void Channel_UserLeft(object sender, IrcChannelUserEventArgs e)
        {
            var channel = sender as IrcChannel;
            // TODO: Update UserList
            //OnChannelUserJoined(channel, e);
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
                var mentionSummary = GetMentionSummary(channel);
                var mentionMessage = GetIrcMessage(mentionSummary, e, messageType);
                mentionSummary.Messages.Add(mentionMessage);
                _unitOfWork.Commit();
                var eventArgs = new MentionItemEventArgs()
                {
                    Mention = mentionSummary,
                    Message = mentionMessage,
                };
                OnMentionItemReceived(eventArgs);
            }
            else
            {
                var channelSummary = GetChannelSummary(channel);
                var channelMessage = GetIrcMessage(channelSummary, e, messageType);
                channelSummary.Messages.Add(channelMessage);
                _unitOfWork.Commit();
                var eventArgs = new ChannelItemEventArgs()
                {
                    Channel = channelSummary,
                    Message = channelMessage,
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
                Channel = channelSummary,
                Status = ChannelStatusTypeEnum.Join,
            };
            OnChannelJoined(eventArgs);
        }

        private void LeaveChannel(IrcChannel channel)
        {
            var channelSummary = GetChannelSummary(channel);
            var eventArgs = new ChannelStatusEventArgs()
            {
                Channel = channelSummary,
                Status = ChannelStatusTypeEnum.Leave,
            };
            OnChannelLeft(eventArgs);
        }

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

        private void OnChannelItemReceived(ChannelItemEventArgs e)
        {
            var handler = MessageReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        private void OnMentionItemReceived(MentionItemEventArgs e)
        {
            var handler = MentionReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        private ChannelSummary GetChannelSummary(IrcChannel channel)
        {
            var server = GetServerByName(channel.Client.ServerName);
            var result = _unitOfWork.Channels.FindBy(x => x.ServerId == server.Id && x.Name == channel.Name.ToLower()).FirstOrDefault();
            if (result == null)
            {
                result = new ChannelSummary();
                result.Name = channel.Name.ToLower();
                result.Server = server;
                result.ServerId = server.Id;

                _unitOfWork.Channels.Add(result);
                _unitOfWork.Commit();
            }
            return result;
        }

        private MentionSummary GetMentionSummary(IrcChannel channel)
        {
            var server = GetServerByName(channel.Client.ServerName);
            var result = _unitOfWork.Mentions.FindBy(x => x.ServerId == server.Id && x.Name == channel.Name.ToLower()).FirstOrDefault();
            if (result == null)
            {
                result = new MentionSummary();
                result.Name = channel.Name.ToLower();
                result.Server = server;
                result.ServerId = server.Id;

                _unitOfWork.Mentions.Add(result);
                _unitOfWork.Commit();
            }
            return result;
        }

        private SessionServer GetServerByName(string serverName)
        {
            // TODO: Error handling make sure _session != null
            return _session.Servers.FirstOrDefault(x => x.HostName == serverName);
        }

        private ChannelItem GetIrcMessage(ChannelSummary summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new ChannelItem();
            // Set values
            result.TimeStamp = DateTime.Now;
            result.IsRead = false;
            result.Summary = summary;
            result.SummaryId = summary.Id;
            result.Source = eventArgs.Source.Name;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
        }

        private MentionItem GetIrcMessage(MentionSummary summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new MentionItem();
            // Set values
            result.TimeStamp = DateTime.Now;
            result.IsRead = false;
            result.Summary = summary;
            result.SummaryId = summary.Id;
            result.Source = eventArgs.Source.Name;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
        }
    }
}
