//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models.Settings
{
    [global::System.Data.Linq.Mapping.TableAttribute()]
    public partial class User : IBaseModel, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private string _Name;

        private string _NickName;

        private string _NickNameAlternate;

        private string _FullName;

        private string _Username;

        private System.Nullable<bool> _IsInvisible;

        private string _QuitMessage;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Name", DbType = "NVarChar(128) NOT NULL", CanBeNull = false)]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                if ((this._Name != value))
                {
                    this.OnNameChanging(value);
                    this.SendPropertyChanging();
                    this._Name = value;
                    this.SendPropertyChanged("Name");
                    this.OnNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_NickName", DbType = "NVarChar(64) NOT NULL", CanBeNull = false)]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_NickNameAlternate", DbType = "NVarChar(64)")]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_FullName", DbType = "NVarChar(128)")]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Username", DbType = "NVarChar(64)")]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_IsInvisible", DbType = "Bit")]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_QuitMessage", DbType = "NVarChar(256)")]
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
