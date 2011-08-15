using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Settings;
using IrcDotNet;

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
            DataUnitOfWork.Default.Commit();
            //_unitOfWork.Commit();
            var eventArgs = new MessageItemEventArgs()
            {
                NetworkId = summary.NetworkId,
                SummaryId = summary.Id,
                MessageId = message.Id,
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
            var network = GetNetworkByClientId(user.Client.ClientId);
            var result = network.Messages.FirstOrDefault(x => x.Name == user.NickName.ToLower());
            if (result == null)
            {
                result = new MessageSummary() { Name = user.NickName.ToLower() };
                network.Messages.Add(result);
                //_unitOfWork.Commit();
                DataUnitOfWork.Default.Commit();
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
            //result.Source = eventArgs.Source.Name;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
        }

        private SessionNetwork GetNetworkByClientId(string clientId)
        {
            var integerId = -1;
            int.TryParse(clientId, out integerId);
            // TODO: Error handling make sure _session != null
            return _session.Networks.FirstOrDefault(x => x.Id == integerId);
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
