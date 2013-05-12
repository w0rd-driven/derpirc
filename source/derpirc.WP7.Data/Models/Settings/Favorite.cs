using System;
using System.ComponentModel;

namespace derpirc.Data.Models.Settings
{
    public partial class Favorite : IBaseModel, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private string _Name;

        private bool _IsAutoConnect;

        private string _Password;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnCreated();
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
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

        /// <summary>
        /// Identifier
        /// </summary>
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

        /// <summary>
        /// Channel favorite name
        /// </summary>
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

        /// <summary>
        /// Auto connect on server join
        /// </summary>
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

        /// <summary>
        /// Channel key
        /// </summary>
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
