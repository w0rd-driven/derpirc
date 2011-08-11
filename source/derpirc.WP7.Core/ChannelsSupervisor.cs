﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using derpirc.Data;
using derpirc.Data.Settings;
using IrcDotNet;

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
        public event EventHandler<ChannelItemEventArgs> ChannelItemReceived;
        public event EventHandler<MentionItemEventArgs> MentionItemReceived;

        public ChannelsSupervisor(DataUnitOfWork unitOfWork, Session session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
            _localUsers = new FixupCollection<IrcLocalUser>();
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
                var summary = GetMentionSummary(e.Source as IrcUser);
                var message = GetIrcMessage(summary, e, messageType);
                summary.Messages.Add(message);
                //_unitOfWork.Commit();
                DataUnitOfWork.Default.Commit();
                var eventArgs = new MentionItemEventArgs()
                {
                    User = summary,
                    Message = message,
                };
                OnMentionItemReceived(eventArgs);
            }
            else
            {
                var summary = GetChannelSummary(channel);
                var message = GetIrcMessage(summary, e, messageType);
                summary.Messages.Add(message);
                //_unitOfWork.Commit();
                DataUnitOfWork.Default.Commit();
                var eventArgs = new ChannelItemEventArgs()
                {
                    Channel = summary,
                    Message = message,
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
                Status = ChannelStatusType.Join,
            };
            OnChannelJoined(eventArgs);
        }

        private void LeaveChannel(IrcChannel channel)
        {
            var channelSummary = GetChannelSummary(channel);
            var eventArgs = new ChannelStatusEventArgs()
            {
                Channel = channelSummary,
                Status = ChannelStatusType.Leave,
            };
            OnChannelLeft(eventArgs);
        }

        private ChannelSummary GetChannelSummary(IrcChannel channel)
        {
            var server = GetServerByName(channel.Client.ServerName);
            var result = server.Channels.FirstOrDefault(x => x.ServerId == server.Id && x.Name == channel.Name.ToLower());
            if (result == null)
            {
                result = new ChannelSummary() { Name = channel.Name.ToLower() };
                server.Channels.Add(result);
                //_unitOfWork.Commit();
                DataUnitOfWork.Default.Commit();
            }
            return result;
        }

        private MentionSummary GetMentionSummary(IrcUser user)
        {
            var server = GetServerByName(user.Client.ServerName);
            var result = server.Mentions.FirstOrDefault(x => x.ServerId == server.Id && x.Name == user.NickName.ToLower());
            if (result == null)
            {
                result = new MentionSummary() { Name = user.NickName.ToLower() };
                server.Mentions.Add(result);
                //_unitOfWork.Commit();
                DataUnitOfWork.Default.Commit();
            }
            return result;
        }

        // TODO: static method
        private SessionServer GetServerByName(string serverName)
        {
            // TODO: Error handling make sure _session != null
            return _session.Servers.FirstOrDefault(x => x.HostName.ToLower() == serverName.ToLower());
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
            var handler = ChannelItemReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        private void OnMentionItemReceived(MentionItemEventArgs e)
        {
            var handler = MentionItemReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
    }
}
