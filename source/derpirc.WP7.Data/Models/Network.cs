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
    public partial class Network : IBaseModel, INotifyPropertyChanging, INotifyPropertyChanged
    {
        [Column(IsVersion = true)]
        private Binary _Version;

        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private int _SessionId;

        private string _DisplayName;

        private string _Name;

        private string _HostName;

        private string _ConnectedHostName;

        private string _Ports;

        private string _Password;

        private EntitySet<Channel> _Channels;

        private EntitySet<Mention> _Mentions;

        private EntitySet<Message> _Messages;

        private EntityRef<Session> _Session;

        private EntitySet<Favorite> _Favorites;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        partial void OnSessionIdChanging(int value);
        partial void OnSessionIdChanged();
        partial void OnServerIdChanging(int value);
        partial void OnServerIdChanged();
        partial void OnDisplayNameChanging(string value);
        partial void OnDisplayNameChanged();
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        partial void OnHostNameChanging(string value);
        partial void OnHostNameChanged();
        partial void OnConnectedHostNameChanging(string value);
        partial void OnConnectedHostNameChanged();
        partial void OnPortsChanging(string value);
        partial void OnPortsChanged();
        partial void OnPasswordChanging(string value);
        partial void OnPasswordChanged();
        partial void OnIsConnectedChanging(bool value);
        partial void OnIsConnectedChanged();
        #endregion

        public Network()
        {
            this._Channels = new EntitySet<Channel>(new Action<Channel>(this.attach_Channels), new Action<Channel>(this.detach_Channels));
            this._Mentions = new EntitySet<Mention>(new Action<Mention>(this.attach_Mentions), new Action<Mention>(this.detach_Mentions));
            this._Messages = new EntitySet<Message>(new Action<Message>(this.attach_Messages), new Action<Message>(this.detach_Messages));
            this._Session = default(EntityRef<Session>);
            this._Favorites = new EntitySet<Favorite>(new Action<Favorite>(this.attach_Favorites), new Action<Favorite>(this.detach_Favorites));
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_SessionId", DbType = "Int NOT NULL")]
        public int SessionId
        {
            get
            {
                return this._SessionId;
            }
            set
            {
                if ((this._SessionId != value))
                {
                    if (this._Session.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.OnSessionIdChanging(value);
                    this.SendPropertyChanging();
                    this._SessionId = value;
                    this.SendPropertyChanged("SessionId");
                    this.OnSessionIdChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_DisplayName", DbType = "NVarChar(128)")]
        public string DisplayName
        {
            get
            {
                return this._DisplayName;
            }
            set
            {
                if (string.Compare(this._DisplayName, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var newValue = (value ?? string.Empty).ToLowerInvariant();
                    //var newValue = !string.IsNullOrEmpty(value) ? value.ToLower() : null;
                    this.OnDisplayNameChanging(newValue);
                    this.SendPropertyChanging();
                    this._DisplayName = newValue;
                    this.SendPropertyChanged("DisplayName");
                    this.OnDisplayNameChanged();
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
                if (string.Compare(this._Name, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var newValue = (value ?? string.Empty).ToLowerInvariant();
                    this.OnNameChanging(newValue);
                    this.SendPropertyChanging();
                    this._Name = newValue;
                    this.SendPropertyChanged("Name");
                    this.OnNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_HostName", DbType = "NVarChar(128) NOT NULL", CanBeNull = false)]
        public string HostName
        {
            get
            {
                return this._HostName;
            }
            set
            {
                if (string.Compare(this._HostName, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var newValue = (value ?? string.Empty).ToLowerInvariant();
                    this.OnHostNameChanging(newValue);
                    this.SendPropertyChanging();
                    this._HostName = newValue;
                    this.SendPropertyChanged("HostName");
                    this.OnHostNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ConnectedHostName", DbType = "NVarChar(128) NOT NULL", CanBeNull = false)]
        public string ConnectedHostName
        {
            get
            {
                return this._ConnectedHostName;
            }
            set
            {
                if (string.Compare(this._ConnectedHostName, value, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    var newValue = (value ?? string.Empty).ToLowerInvariant();
                    this.OnConnectedHostNameChanging(newValue);
                    this.SendPropertyChanging();
                    this._ConnectedHostName = newValue;
                    this.SendPropertyChanged("ConnectedHostName");
                    this.OnConnectedHostNameChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Ports", DbType = "NVarChar(256)")]
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

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Channel", Storage = "_Channels", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "CASCADE")]
        public EntitySet<Channel> Channels
        {
            get
            {
                return this._Channels;
            }
            set
            {
                this._Channels.Assign(value);
            }
        }

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Mention", Storage = "_Mentions", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "CASCADE")]
        public EntitySet<Mention> Mentions
        {
            get
            {
                return this._Mentions;
            }
            set
            {
                this._Mentions.Assign(value);
            }
        }

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Message", Storage = "_Messages", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "CASCADE")]
        public EntitySet<Message> Messages
        {
            get
            {
                return this._Messages;
            }
            set
            {
                this._Messages.Assign(value);
            }
        }

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Session_Network", Storage = "_Session", ThisKey = "SessionId", OtherKey = "Id", IsForeignKey = true)]
        public Session Session
        {
            get
            {
                return this._Session.Entity;
            }
            set
            {
                Session previousValue = this._Session.Entity;
                if (((previousValue != value)
                            || (this._Session.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if ((previousValue != null))
                    {
                        this._Session.Entity = null;
                        previousValue.Networks.Remove(this);
                    }
                    this._Session.Entity = value;
                    if ((value != null))
                    {
                        value.Networks.Add(this);
                        this._SessionId = value.Id;
                    }
                    else
                    {
                        this._SessionId = default(int);
                    }
                    this.SendPropertyChanged("Session");
                }
            }
        }

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Favorite", Storage = "_Favorites", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "CASCADE")]
        public EntitySet<Favorite> Favorites
        {
            get
            {
                return this._Favorites;
            }
            set
            {
                this._Favorites.Assign(value);
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

        private void attach_Channels(Channel entity)
        {
            this.SendPropertyChanging();
            entity.Network = this;
        }

        private void detach_Channels(Channel entity)
        {
            this.SendPropertyChanging();
            entity.Network = null;
        }

        private void attach_Mentions(Mention entity)
        {
            this.SendPropertyChanging();
            entity.Network = this;
        }

        private void detach_Mentions(Mention entity)
        {
            this.SendPropertyChanging();
            entity.Network = null;
        }

        private void attach_Messages(Message entity)
        {
            this.SendPropertyChanging();
            entity.Network = this;
        }

        private void detach_Messages(Message entity)
        {
            this.SendPropertyChanging();
            entity.Network = null;
        }

        private void attach_Favorites(Favorite entity)
        {
            this.SendPropertyChanging();
            entity.Network = this;
        }

        private void detach_Favorites(Favorite entity)
        {
            this.SendPropertyChanging();
            entity.Network = null;
        }
    }
}
