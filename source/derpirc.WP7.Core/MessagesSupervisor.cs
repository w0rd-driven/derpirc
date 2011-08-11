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
    public class MessagesSupervisor
    {
        private DataUnitOfWork _unitOfWork;
        private Session _session;

        private ObservableCollection<IrcLocalUser> _localUsers;
        public ObservableCollection<IrcLocalUser> LocalUsers
        {
            get { return _localUsers; }
            private set { }
        }

        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        public MessagesSupervisor(DataUnitOfWork unitOfWork, Session session)
        {
            _unitOfWork = unitOfWork;
            _session = session;
            _localUsers = new FixupCollection<IrcLocalUser>();
            _localUsers.CollectionChanged += new NotifyCollectionChangedEventHandler(LocalUsers_CollectionChanged);
        }

        private void LocalUsers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    var newItem = item as IrcLocalUser;
                    newItem.NickNameChanged += LocalUser_NickNameChanged;
                    newItem.MessageReceived += LocalUser_MessageReceived;
                    //newItem.MessageSent += LocalUser_MessageSent;
                    //newItem.NoticeReceived += LocalUser_NoticeReceived;
                }
            }
        }

        private void LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            var messageType = MessageType.Theirs;
            if (e.Source is IrcUser)
            {
                messageType = MessageType.Theirs;
            }

            var summary = GetMessageSummary(e.Source as IrcUser);
            var message = GetIrcMessage(summary, e, messageType);
            summary.Messages.Add(message);
            _unitOfWork.Commit();
            var eventArgs = new MessageItemEventArgs()
            {
                User = summary,
                Message = message,
            };
            OnMessageItemReceived(eventArgs);
        }

        private void LocalUser_NickNameChanged(object sender, EventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            //OnLocalUserNickNameChanged(localUser, e);
        }

        private void LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            //OnLocalUserNoticeReceived(localUser, e);
        }

        private MessageSummary GetMessageSummary(IrcUser user)
        {
            var server = GetServerByName(user.Client.ServerName);
            var result = server.Messages.FirstOrDefault(x => x.ServerId == server.Id && x.Name == user.NickName.ToLower());
            if (result == null)
            {
                result = new MessageSummary() { Name = user.NickName.ToLower() };
                server.Messages.Add(result);
                _unitOfWork.Commit();
            }
            return result;
        }

        private MessageItem GetIrcMessage(MessageSummary summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new MessageItem();
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

        // TODO: static method
        private SessionServer GetServerByName(string serverName)
        {
            // TODO: Error handling make sure _session != null
            return _session.Servers.FirstOrDefault(x => x.HostName.ToLower() == serverName.ToLower());
        }

        private void OnMessageItemReceived(MessageItemEventArgs e)
        {
            var handler = MessageItemReceived;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
    }
}
