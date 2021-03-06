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
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models
{
    [global::System.Data.Linq.Mapping.TableAttribute()]
    public partial class Favorite : IBaseModel, INotifyPropertyChanging, INotifyPropertyChanged
    {
        [Column(IsVersion = true)]
        private Binary _Version;

        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private int _NetworkId;

        private string _Name;

        private bool _IsAutoConnect;

        private string _Password;

        private EntityRef<Network> _Network;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        partial void OnNetworkIdChanging(int value);
        partial void OnNetworkIdChanged();
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        partial void OnIsAutoConnectChanging(System.Nullable<bool> value);
        partial void OnIsAutoConnectChanged();
        partial void OnPasswordChanging(string value);
        partial void OnPasswordChanged();
        #endregion

        public Favorite()
        {
            this._Network = default(EntityRef<Network>);
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_NetworkId", DbType = "Int NOT NULL")]
        public int NetworkId
        {
            get
            {
                return this._NetworkId;
            }
            set
            {
                if ((this._NetworkId != value))
                {
                    this.OnNetworkIdChanging(value);
                    this.SendPropertyChanging();
                    this._NetworkId = value;
                    this.SendPropertyChanged("NetworkId");
                    this.OnNetworkIdChanged();
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_IsAutoConnect", DbType = "Bit")]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Password", DbType = "NVarChar(128)")]
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

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Favorite", Storage = "_Network", ThisKey = "NetworkId", OtherKey = "Id", IsForeignKey = true)]
        public Network Network
        {
            get
            {
                return this._Network.Entity;
            }
            set
            {
                Network previousValue = this._Network.Entity;
                if (((previousValue != value)
                            || (this._Network.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if ((previousValue != null))
                    {
                        this._Network.Entity = null;
                        previousValue.Favorites.Remove(this);
                    }
                    this._Network.Entity = value;
                    if ((value != null))
                    {
                        value.Favorites.Add(this);
                        this._NetworkId = value.Id;
                    }
                    else
                    {
                        this._NetworkId = default(int);
                    }
                    this.SendPropertyChanged("Network");
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
