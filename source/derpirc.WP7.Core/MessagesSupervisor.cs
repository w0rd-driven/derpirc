using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;

namespace derpirc.Core
{
    public class MessagesSupervisor : IDisposable
    {
        private bool _isDisposed;
        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        public MessagesSupervisor()
        {
        }

        public void SendMessage(MessageItem message)
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
                OnMessageItemReceived(eventArgs);
            }
        }

        public void AttachLocalUser(IrcLocalUser localUser)
        {
            localUser.NickNameChanged += LocalUser_NickNameChanged;
            localUser.MessageReceived += LocalUser_MessageReceived;
            //localUser.MessageSent += LocalUser_MessageSent;
            //localUser.NoticeReceived += LocalUser_NoticeReceived;
        }

        public void DetachLocalUser(IrcLocalUser localUser)
        {
            localUser.NickNameChanged -= LocalUser_NickNameChanged;
            localUser.MessageReceived -= LocalUser_MessageReceived;
            //localUser.MessageSent -= LocalUser_MessageSent;
            //localUser.NoticeReceived -= LocalUser_NoticeReceived;
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

        private Message GetMessageSummary(IrcUser user)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(user.Client);
            var result = network.Messages.FirstOrDefault(x => x.Name == user.NickName.ToLower());
            if (result == null)
            {
                result = new Message() { Name = user.NickName.ToLower() };
                network.Messages.Add(result);
                //_unitOfWork.Commit();
                DataUnitOfWork.Default.Commit();
            }
            return result;
        }

        private MessageItem GetIrcMessage(Message summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new MessageItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            result.Summary = summary;
            //result.Source = eventArgs.Source.Name;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
        }

        #region Events

        private void OnMessageItemReceived(MessageItemEventArgs e)
        {
            var handler = MessageItemReceived;
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
