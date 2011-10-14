using System;
using System.ComponentModel;

namespace derpirc.Data.Models.Settings
{
    public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private string _NickName;

        private string _NickNameAlternate;

        private string _FullName;

        private string _Username;

        private System.Nullable<bool> _IsInvisible;

        private string _QuitMessage;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnCreated();
        partial void OnNickNameChanging(string value);
        partial void OnNickNameChanged();
        partial void OnNickNameAlternateChanging(string value);
        partial void OnNickNameAlternateChanged();
        partial void OnFullNameChanging(string value);
        partial void OnFullNameChanged();
        partial void OnUsernameChanging(string value);
        partial void OnUsernameChanged();
        partial void OnIsInvisibleChanging(System.Nullable<bool> value);
        partial void OnIsInvisibleChanged();
        partial void OnQuitMessageChanging(string value);
        partial void OnQuitMessageChanged();
        #endregion

        public User()
        {
            OnCreated();
        }

        public string NickName
        {
            get
            {
                return this._NickName;
            }
            set
            {
                if ((this._NickName != value))
                {
                    this.OnNickNameChanging(value);
                    this.SendPropertyChanging();
                    this._NickName = value;
                    this.SendPropertyChanged("NickName");
                    this.OnNickNameChanged();
                }
            }
        }

        public string NickNameAlternate
        {
            get
            {
                return this._NickNameAlternate;
            }
            set
            {
                if ((this._NickNameAlternate != value))
                {
                    this.OnNickNameAlternateChanging(value);
                    this.SendPropertyChanging();
                    this._NickNameAlternate = value;
                    this.SendPropertyChanged("NickNameAlternate");
                    this.OnNickNameAlternateChanged();
                }
            }
        }

        public string FullName
        {
            get
            {
                return this._FullName;
            }
            set
            {
                if ((this._FullName != value))
                {
                    this.OnFullNameChanging(value);
                    this.SendPropertyChanging();
                    this._FullName = value;
                    this.SendPropertyChanged("FullName");
                    this.OnFullNameChanged();
                }
            }
        }

        public string Username
        {
            get
            {
                return this._Username;
            }
            set
            {
                if ((this._Username != value))
                {
                    this.OnUsernameChanging(value);
                    this.SendPropertyChanging();
                    this._Username = value;
                    this.SendPropertyChanged("Username");
                    this.OnUsernameChanged();
                }
            }
        }

        public System.Nullable<bool> IsInvisible
        {
            get
            {
                return this._IsInvisible;
            }
            set
            {
                if ((this._IsInvisible != value))
                {
                    this.OnIsInvisibleChanging(value);
                    this.SendPropertyChanging();
                    this._IsInvisible = value;
                    this.SendPropertyChanged("IsInvisible");
                    this.OnIsInvisibleChanged();
                }
            }
        }

        public string QuitMessage
        {
            get
            {
                return this._QuitMessage;
            }
            set
            {
                if ((this._QuitMessage != value))
                {
                    this.OnQuitMessageChanging(value);
                    this.SendPropertyChanging();
                    this._QuitMessage = value;
                    this.SendPropertyChanged("QuitMessage");
                    this.OnQuitMessageChanged();
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
