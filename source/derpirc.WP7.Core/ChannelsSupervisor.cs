using System;
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

        public ChannelsSupervisor(DataUnitOfWork unitOfWork, Session session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
            _localUsers = new ObservableCollection<IrcLocalUser>();
            _localUsers.CollectionChanged += new NotifyCollectionChangedEventHandler(LocalUsers_CollectionChanged);
            _channels = new ObservableCollection<IrcChannel>();
            _channelSummaries = new ObservableCollection<ChannelSummary>();
        }

        void LocalUsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
            //OnLocalUserJoinedChannel(localUser, e);
        }

        private void LocalUser_LeftChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            e.Channel.UserJoined -= Channel_UserJoined;
            e.Channel.UserLeft -= Channel_UserLeft;
            e.Channel.MessageReceived -= Channel_MessageReceived;
            e.Channel.NoticeReceived -= Channel_NoticeReceived;
            //OnLocalUserJoinedChannel(localUser, e);
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
            if (e.Source is IrcUser)
            {
            }
            var channelSummary = GetChannelSummary(channel);
            var channelMessage = GetIrcMessage(channelSummary, e);
            channelSummary.Messages.Add(channelMessage);
            _unitOfWork.Commit();
            // TODO: Bubble up a UI event
            //OnChannelMessageReceived(channel, e);
        }

        private void Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = sender as IrcChannel;
            //OnChannelNoticeReceived(channel, e);
        }

        private ChannelSummary GetChannelSummary(IrcChannel channel)
        {
            var channelSummary = _unitOfWork.Channels.FindBy(x => x.Name == channel.Name.ToLower()).FirstOrDefault();
            if (channelSummary == null)
            {
                channelSummary = new ChannelSummary();
                channelSummary.Name = channel.Name.ToLower();
                // HACK: Get server from _session via dependency injection
                var clientId = -1;
                int.TryParse(channel.Client.ClientId, out clientId);
                var server = GetServer(clientId);
                channelSummary.Server = server;
                channelSummary.ServerId = server.Id;

                _unitOfWork.Channels.Add(channelSummary);
                _unitOfWork.Commit();
            }
            return channelSummary;
        }

        private SessionServer GetServer(int clientId)
        {
            // TODO: Error handling make sure _session != null
            return _session.Servers.FirstOrDefault(x => x.BasedOnId == clientId); ;
        }

        private ChannelMessage GetIrcMessage(ChannelSummary channelSummary, IrcMessageEventArgs eventArgs)
        {
            var result = new ChannelMessage();
            var line = eventArgs.Text;

            //if (line.Length > 1 && line.StartsWith("."))
            //{
            //    // Process command.
            //    var parts = commandPartsSplitRegex.Split(line.Substring(1)).Select(p => p.TrimStart('/')).ToArray();
            //    var command = parts.First();
            //    var parameters = parts.Skip(1).ToArray();

            //    //result.Parameters = parameters;
            //    //ReadChatCommand(client, eventArgs.Source, eventArgs.Targets, command, parameters);
            //}

            // Set values
            result.TimeStamp = DateTime.Now;
            result.IsRead = false;
            result.Summary = channelSummary;
            result.SummaryId = channelSummary.Id;
            result.Source = eventArgs.Source.Name;
            //result.Command = command;
            result.Text = eventArgs.Text;

            return result;
        }
    }
}
