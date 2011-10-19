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
                localUser.NickNameChanged += this.LocalUser_NickNameChanged;
                localUser.MessageReceived += this.LocalUser_MessageReceived;
                localUser.NoticeReceived += this.LocalUser_NoticeReceived;
                //localUser.MessageSent += this.LocalUser_MessageSent;
            }
        }

        public void DetachLocalUser(IrcLocalUser localUser)
        {
            if (localUser != null)
            {
                localUser.NickNameChanged -= this.LocalUser_NickNameChanged;
                localUser.MessageReceived -= this.LocalUser_MessageReceived;
                localUser.NoticeReceived -= this.LocalUser_NoticeReceived;
                //localUser.MessageSent -= this.LocalUser_MessageSent;
            }
        }

        #region UI-facing methods

        public void SendMessage(MessageItem message)
        {
            if (message != null)
            {
                var summary = message.Summary;
                var localUser = SupervisorFacade.Default.GetLocalUserBySummary(summary);

                // TODO: Error check: If localUser == null, alert UI/internals
                if (localUser != null)
                {
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

        #endregion

        private void LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = sender as IrcLocalUser;
            var messageType = MessageType.Theirs;
            if (e.Source is IrcUser)
            {
                messageType = MessageType.Theirs;
            }

            // TODO: Error check: If summary == null, alert UI/internals
            var summary = this.GetMessageSummary(e.Source as IrcUser);
            if (summary != null)
            {
                // HACK: This should never be null but just in case, keep an eye on it
                var message = this.GetIrcMessage(summary, e, messageType);
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

        private MessageItem GetIrcMessage(Message summary, IrcMessageEventArgs eventArgs, MessageType messageType)
        {
            var result = new MessageItem();
            // Set values
            result.Timestamp = DateTime.Now;
            result.IsRead = false;
            result.Summary = summary;
            result.Text = eventArgs.Text;
            result.Type = messageType;
            return result;
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
