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

        private DataUnitOfWork _unitOfWork;

        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        public MessagesSupervisor(DataUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public void AttachLocalUser(IrcLocalUser localUser)
        {
            if (localUser != null)
            {
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
                localUser.MessageReceived -= this.LocalUser_MessageReceived;
                localUser.MessageSent -= this.LocalUser_MessageSent;
                localUser.NoticeReceived -= this.LocalUser_NoticeReceived;
                localUser.NoticeSent -= this.LocalUser_NoticeSent;
                localUser.NickNameChanged -= this.LocalUser_NickNameChanged;
            }
        }

        #region UI-facing methods

        public void SendMessage(MessageItem message)
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
                    this.OnMessageItemReceived(eventArgs);
                }
            }
        }

        public void SendNotice(MessageItem message)
        {
            if (message != null)
            {
                var summary = message.Summary;
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);

                if (localUser != null)
                {
                    // TODO: Recovery : Notice not sent
                    localUser.SendNotice(summary.Name, message.Text);

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
                    this.OnMessageItemReceived(eventArgs);
                }
            }
        }

        #endregion

        private void LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            var ircUser = e.Source as IrcUser;
            var messageText = e.Text;
            var messageType = MessageType.Theirs;

            AddMessage(ircUser, messageText, messageType);
        }

        private void LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            var ircUser = e.Source as IrcUser;
            var messageText = e.Text;
            var messageType = MessageType.Theirs;

            AddMessage(ircUser, messageText, messageType);
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

        private void AddMessage(IrcUser ircUser, string messageText, MessageType messageType)
        {
            var summary = this.GetMessageSummary(ircUser);
            if (summary != null)
            {
                var message = this.GetIrcMessage(summary, messageText, messageType);
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

        #region Lookup methods

        private Message GetMessageSummary(IrcUser user)
        {
            var network = SupervisorFacade.Default.GetNetworkByClient(user.Client);
            if (network != null)
            {
                var result = network.Messages.FirstOrDefault(x => x.Name == user.NickName.ToLower());
                if (result == null)
                {
                    result = new Message() { Name = user.NickName };
                    network.Messages.Add(result);
                    this._unitOfWork.Commit();
                }
                return result;
            }
            return null;
        }

        private MessageItem GetIrcMessage(Message summary, string text, MessageType type)
        {
            var result = new MessageItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            result.Text = text;
            result.Type = type;
            return result;
        }

        #endregion

        #region Events

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
