using System;
using System.ComponentModel;

namespace derpirc.Data.Models.Settings
{
    public partial class Storage : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private bool _IsWipeSettings;

        private DateTime _SettingsCreateDate;

        private bool _IsWipeData;

        private DateTime _DataCreateDate;

        private int _ShowMaxMessages;

        private int _StoreMaxMessages;

        private int _StoreMaxMessageDays;

        private bool _IsScheduledTaskEnabled;

        private bool _IsSyncRequired;

        private DateTime _SyncDate;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnCreated();
        partial void OnIsWipeOnRestartChanging(bool value);
        partial void OnIsWipeOnRestartChanged();
        partial void OnIsWipeSettingsChanging(bool value);
        partial void OnIsWipeSettingsChanged();
        partial void OnSettingsCreateDateChanging(DateTime value);
        partial void OnSettingsCreateDateChanged();
        partial void OnIsWipeDataChanging(bool value);
        partial void OnIsWipeDataChanged();
        partial void OnDataCreateDateChanging(DateTime value);
        partial void OnDataCreateDateChanged();
        partial void OnShowMaxMessagesChanging(int value);
        partial void OnShowMaxMessagesChanged();
        partial void OnIsScheduledTaskEnabledChanging(bool value);
        partial void OnIsScheduledTaskEnabledChanged();
        partial void OnStoreMaxMessagesChanging(int value);
        partial void OnStoreMaxMessagesChanged();
        partial void OnStoreMaxMessageDaysChanging(int value);
        partial void OnStoreMaxMessageDaysChanged();
        partial void OnIsSyncRequiredChanging(bool value);
        partial void OnIsSyncRequiredChanged();
        partial void OnSyncDateChanging(DateTime value);
        partial void OnSyncDateChanged();
        #endregion

        public Storage()
        {
            OnCreated();
        }

        public bool IsWipeSettings
        {
            get
            {
                return this._IsWipeSettings;
            }
            set
            {
                if ((this._IsWipeSettings != value))
                {
                    this.OnIsWipeSettingsChanging(value);
                    this.SendPropertyChanging();
                    this._IsWipeSettings = value;
                    this.SendPropertyChanged("IsWipeSettings");
                    this.OnIsWipeSettingsChanged();
                }
            }
        }

        public DateTime SettingsCreateDate
        {
            get
            {
                return this._SettingsCreateDate;
            }
            set
            {
                if ((this._SettingsCreateDate != value))
                {
                    this.OnSettingsCreateDateChanging(value);
                    this.SendPropertyChanging();
                    this._SettingsCreateDate = value;
                    this.SendPropertyChanged("SettingsCreateDate");
                    this.OnSettingsCreateDateChanged();
                }
            }
        }

        public bool IsWipeData
        {
            get
            {
                return this._IsWipeData;
            }
            set
            {
                if ((this._IsWipeData != value))
                {
                    this.OnIsWipeDataChanging(value);
                    this.SendPropertyChanging();
                    this._IsWipeData = value;
                    this.SendPropertyChanged("IsWipeData");
                    this.OnIsWipeDataChanged();
                }
            }
        }

        public DateTime DataCreateDate
        {
            get
            {
                return this._DataCreateDate;
            }
            set
            {
                if ((this._DataCreateDate != value))
                {
                    this.OnDataCreateDateChanging(value);
                    this.SendPropertyChanging();
                    this._DataCreateDate = value;
                    this.SendPropertyChanged("DataCreateDate");
                    this.OnDataCreateDateChanged();
                }
            }
        }

        public int ShowMaxMessages
        {
            get
            {
                return this._ShowMaxMessages;
            }
            set
            {
                if ((this._ShowMaxMessages != value))
                {
                    this.OnShowMaxMessagesChanging(value);
                    this.SendPropertyChanging();
                    this._ShowMaxMessages = value;
                    this.SendPropertyChanged("ShowMaxMessages");
                    this.OnShowMaxMessagesChanged();
                }
            }
        }

        public int StoreMaxMessages
        {
            get
            {
                return this._StoreMaxMessages;
            }
            set
            {
                if ((this._StoreMaxMessages != value))
                {
                    this.OnStoreMaxMessagesChanging(value);
                    this.SendPropertyChanging();
                    this._StoreMaxMessages = value;
                    this.SendPropertyChanged("StoreMaxMessages");
                    this.OnStoreMaxMessagesChanged();
                }
            }
        }

        public int StoreMaxMessageDays
        {
            get
            {
                return this._StoreMaxMessageDays;
            }
            set
            {
                if ((this._StoreMaxMessageDays != value))
                {
                    this.OnStoreMaxMessageDaysChanging(value);
                    this.SendPropertyChanging();
                    this._StoreMaxMessageDays = value;
                    this.SendPropertyChanged("StoreMaxMessageDays");
                    this.OnStoreMaxMessageDaysChanged();
                }
            }
        }

        public bool IsScheduledTaskEnabled
        {
            get
            {
                return this._IsScheduledTaskEnabled;
            }
            set
            {
                if ((this._IsScheduledTaskEnabled != value))
                {
                    this.OnIsScheduledTaskEnabledChanging(value);
                    this.SendPropertyChanging();
                    this._IsScheduledTaskEnabled = value;
                    this.SendPropertyChanged("IsScheduledTaskEnabled");
                    this.OnIsScheduledTaskEnabledChanged();
                }
            }
        }

        public bool IsSyncRequired
        {
            get
            {
                return this._IsSyncRequired;
            }
            set
            {
                if ((this._IsSyncRequired != value))
                {
                    this.OnIsSyncRequiredChanging(value);
                    this.SendPropertyChanging();
                    this._IsSyncRequired = value;
                    this.SendPropertyChanged("IsSyncRequired");
                    this.OnIsSyncRequiredChanged();
                }
            }
        }

        public DateTime SyncDate
        {
            get
            {
                return this._SyncDate;
            }
            set
            {
                if ((this._SyncDate != value))
                {
                    this.OnDataCreateDateChanging(value);
                    this.SendPropertyChanging();
                    this._SyncDate = value;
                    this.SendPropertyChanged("SyncDate");
                    this.OnDataCreateDateChanged();
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
