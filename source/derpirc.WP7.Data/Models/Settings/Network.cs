using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace derpirc.Data.Models.Settings
{
    public partial class Network : IBaseModel, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private string _DisplayName;

        private string _Name;

        private string _HostName;

        private string _Ports;

        private string _Password;

        private List<Favorite> _Favorites;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnCreated();
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        partial void OnDisplayNameChanging(string value);
        partial void OnDisplayNameChanged();
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        partial void OnHostNameChanging(string value);
        partial void OnHostNameChanged();
        partial void OnPortsChanging(string value);
        partial void OnPortsChanged();
        partial void OnPasswordChanging(string value);
        partial void OnPasswordChanged();
        #endregion

        public Network()
        {
            this._Favorites = new List<Favorite>();
            OnCreated();
        }

        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if ((this._Id != value))
                {
                    this.OnIdChanging(value);
                    this.SendPropertyChanging();
                    this._Id = value;
                    this.SendPropertyChanged("Id");
                    this.OnIdChanged();
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return this._DisplayName;
            }
            set
            {
                if ((this._DisplayName != value.ToLower()))
                {
                    this.OnDisplayNameChanging(value.ToLower());
                    this.SendPropertyChanging();
                    this._DisplayName = value.ToLower();
                    this.SendPropertyChanged("DisplayName");
                    this.OnDisplayNameChanged();
                }
            }
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

        public string HostName
        {
            get
            {
                return this._HostName;
            }
            set
            {
                if ((this._HostName != value.ToLower()))
                {
                    this.OnHostNameChanging(value.ToLower());
                    this.SendPropertyChanging();
                    this._HostName = value.ToLower();
                    this.SendPropertyChanged("HostName");
                    this.OnHostNameChanged();
                }
            }
        }

        public string Ports
        {
            get
            {
                return this._Ports;
            }
            set
            {
                if ((this._Ports != value))
                {
                    this.OnPortsChanging(value);
                    this.SendPropertyChanging();
                    this._Ports = value;
                    this.SendPropertyChanged("Ports");
                    this.OnPortsChanged();
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

        public List<Favorite> Favorites
        {
            get
            {
                return _Favorites;
            }
            set
            {
                if (!ReferenceEquals(_Favorites, value))
                {
                    this.SendPropertyChanging();
                    this._Favorites = value;
                    this.SendPropertyChanged("Favorites");
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
