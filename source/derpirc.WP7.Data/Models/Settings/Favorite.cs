using System;
using System.ComponentModel;

namespace derpirc.Data.Models.Settings
{
    public partial class Favorite : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private string _Name;

        private bool _IsAutoConnect;

        private string _Password;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnCreated();
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        partial void OnIsAutoConnectChanging(System.Nullable<bool> value);
        partial void OnIsAutoConnectChanged();
        partial void OnPasswordChanging(string value);
        partial void OnPasswordChanged();
        #endregion

        public Favorite()
        {
            OnCreated();
        }

        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if ((this._Name != value.ToLower()))
                {
                    this.OnNameChanging(value.ToLower());
                    this.SendPropertyChanging();
                    this._Name = value.ToLower();
                    this.SendPropertyChanged("Name");
                    this.OnNameChanged();
                }
            }
        }

        public bool IsAutoConnect
        {
            get
            {
                return this._IsAutoConnect;
            }
            set
            {
                if ((this._IsAutoConnect != value))
                {
                    this.OnIsAutoConnectChanging(value);
                    this.SendPropertyChanging();
                    this._IsAutoConnect = value;
                    this.SendPropertyChanged("IsAutoConnect");
                    this.OnIsAutoConnectChanged();
                }
            }
        }

        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                if ((this._Password != value))
                {
                    this.OnPasswordChanging(value);
                    this.SendPropertyChanging();
                    this._Password = value;
                    this.SendPropertyChanged("Password");
                    this.OnPasswordChanged();
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
