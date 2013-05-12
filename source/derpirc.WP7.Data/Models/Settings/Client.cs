using System;
using System.ComponentModel;

namespace derpirc.Data.Models.Settings
{
    public partial class Client : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private bool _IsUserLockAware;

        private bool _IsRunUnderLock;

        private bool _IsNotifyMentions;

        private bool _IsNotifyMessages;

        private bool _IsReconnectOnDisconnect;

        private bool _IsRejoinOnKick;

        private bool _IsJoinOnInvite;

        private int _DisconnectRetries;

        private int _DisconnectRetryWait;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnCreated();
        partial void OnIsUserLockAwareChanging(bool value);
        partial void OnIsUserLockAwareChanged();
        partial void OnIsRunUnderLockChanging(bool value);
        partial void OnIsRunUnderLockChanged();
        partial void OnIsNotifyMentionsChanging(bool value);
        partial void OnIsNotifyMentionsChanged();
        partial void OnIsNotifyMessagesChanging(bool value);
        partial void OnIsNotifyMessagesChanged();
        partial void OnReconnectOnDisconnectChanging(bool value);
        partial void OnReconnectOnDisconnectChanged();
        partial void OnRejoinOnKickChanging(bool value);
        partial void OnRejoinOnKickChanged();
        partial void OnJoinOnInviteChanging(bool value);
        partial void OnJoinOnInviteChanged();
        partial void OnDisconnectRetriesChanging(int value);
        partial void OnDisconnectRetriesChanged();
        partial void OnDisconnectRetryWaitChanging(int value);
        partial void OnDisconnectRetryWaitChanged();
        #endregion

        public Client()
        {
            OnCreated();
        }

        /// <summary>
        /// Future - Has the user been notified of running under lock?
        /// </summary>
        public bool IsUserLockAware
        {
            get
            {
                return this._IsUserLockAware;
            }
            set
            {
                if ((this._IsUserLockAware != value))
                {
                    this.OnIsUserLockAwareChanging(value);
                    this.SendPropertyChanging();
                    this._IsUserLockAware = value;
                    this.SendPropertyChanged("IsUserLockAware");
                    this.OnIsUserLockAwareChanged();
                }
            }
        }

        /// <summary>
        /// Future - Run under lock screen
        /// </summary>
        public bool IsRunUnderLock
        {
            get
            {
                return this._IsRunUnderLock;
            }
            set
            {
                if ((this._IsRunUnderLock != value))
                {
                    this.OnIsRunUnderLockChanging(value);
                    this.SendPropertyChanging();
                    this._IsRunUnderLock = value;
                    this.SendPropertyChanged("IsRunUnderLock");
                    this.OnIsRunUnderLockChanged();
                }
            }
        }

        /// <summary>
        /// Future - Show toast on new mentions
        /// </summary>
        public bool IsNotifyMentions
        {
            get
            {
                return this._IsNotifyMentions;
            }
            set
            {
                if ((this._IsNotifyMentions != value))
                {
                    this.OnIsNotifyMentionsChanging(value);
                    this.SendPropertyChanging();
                    this._IsNotifyMentions = value;
                    this.SendPropertyChanged("IsNotifyMentions");
                    this.OnIsNotifyMentionsChanged();
                }
            }
        }

        /// <summary>
        /// Future - Show toast on new messages
        /// </summary>
        public bool IsNotifyMessages
        {
            get
            {
                return this._IsNotifyMessages;
            }
            set
            {
                if ((this._IsNotifyMessages != value))
                {
                    this.OnIsNotifyMessagesChanging(value);
                    this.SendPropertyChanging();
                    this._IsNotifyMessages = value;
                    this.SendPropertyChanged("IsNotifyMessages");
                    this.OnIsNotifyMessagesChanged();
                }
            }
        }

        /// <summary>
        /// Reconnect on connection disconnect
        /// </summary>
        public bool IsReconnectOnDisconnect
        {
            get
            {
                return this._IsReconnectOnDisconnect;
            }
            set
            {
                if ((this._IsReconnectOnDisconnect != value))
                {
                    this.OnReconnectOnDisconnectChanging(value);
                    this.SendPropertyChanging();
                    this._IsReconnectOnDisconnect = value;
                    this.SendPropertyChanged("IsReconnectOnDisconnect");
                    this.OnReconnectOnDisconnectChanged();
                }
            }
        }

        /// <summary>
        /// Rejoin channel on kick
        /// </summary>
        public bool IsRejoinOnKick
        {
            get
            {
                return this._IsRejoinOnKick;
            }
            set
            {
                if ((this._IsRejoinOnKick != value))
                {
                    this.OnRejoinOnKickChanging(value);
                    this.SendPropertyChanging();
                    this._IsRejoinOnKick = value;
                    this.SendPropertyChanged("IsRejoinOnKick");
                    this.OnRejoinOnKickChanged();
                }
            }
        }

        /// <summary>
        /// Join channel on invite
        /// </summary>
        public bool IsJoinOnInvite
        {
            get
            {
                return this._IsJoinOnInvite;
            }
            set
            {
                if ((this._IsJoinOnInvite != value))
                {
                    this.OnJoinOnInviteChanging(value);
                    this.SendPropertyChanging();
                    this._IsJoinOnInvite = value;
                    this.SendPropertyChanged("IsJoinOnInvite");
                    this.OnJoinOnInviteChanged();
                }
            }
        }

        /// <summary>
        /// Number of times to retry reconnection on disconnect
        /// </summary>
        public int DisconnectRetries
        {
            get
            {
                return this._DisconnectRetries;
            }
            set
            {
                if ((this._DisconnectRetries != value))
                {
                    this.OnDisconnectRetriesChanging(value);
                    this.SendPropertyChanging();
                    this._DisconnectRetries = value;
                    this.SendPropertyChanged("DisconnectRetries");
                    this.OnDisconnectRetriesChanged();
                }
            }
        }

        /// <summary>
        /// Wait time in seconds before reconnect
        /// </summary>
        public int DisconnectRetryWait
        {
            get
            {
                return this._DisconnectRetryWait;
            }
            set
            {
                if ((this._DisconnectRetryWait != value))
                {
                    this.OnDisconnectRetryWaitChanging(value);
                    this.SendPropertyChanging();
                    this._DisconnectRetryWait = value;
                    this.SendPropertyChanged("DisconnectRetryWait");
                    this.OnDisconnectRetryWaitChanged();
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
