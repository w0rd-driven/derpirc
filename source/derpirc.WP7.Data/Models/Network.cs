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
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private int _SessionId;

        private int _ServerId;

        private string _Name;

        private System.Nullable<bool> _IsJoinEnabled;

        private string _JoinChannels;

        private EntitySet<Channel> _Channels;

        private EntitySet<Mention> _Mentions;

        private EntitySet<Message> _Messages;

        private EntityRef<Server> _Server;

        private EntityRef<Session> _Session;

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
        partial void OnNameChanging(string value);
        partial void OnNameChanged();
        partial void OnIsJoinEnabledChanging(System.Nullable<bool> value);
        partial void OnIsJoinEnabledChanged();
        partial void OnJoinChannelsChanging(string value);
        partial void OnJoinChannelsChanged();
        #endregion

        public Network()
        {
            this._Channels = new EntitySet<Channel>(new Action<Channel>(this.attach_Channels), new Action<Channel>(this.detach_Channels));
            this._Mentions = new EntitySet<Mention>(new Action<Mention>(this.attach_Mentions), new Action<Mention>(this.detach_Mentions));
            this._Messages = new EntitySet<Message>(new Action<Message>(this.attach_Messages), new Action<Message>(this.detach_Messages));
            this._Server = default(EntityRef<Server>);
            this._Session = default(EntityRef<Session>);
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ServerId", DbType = "Int NOT NULL")]
        public int ServerId
        {
            get
            {
                return this._ServerId;
            }
            set
            {
                if ((this._ServerId != value))
                {
                    if (this._Server.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.OnServerIdChanging(value);
                    this.SendPropertyChanging();
                    this._ServerId = value;
                    this.SendPropertyChanged("ServerId");
                    this.OnServerIdChanged();
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_IsJoinEnabled", DbType = "Bit")]
        public System.Nullable<bool> IsJoinEnabled
        {
            get
            {
                return this._IsJoinEnabled;
            }
            set
            {
                if ((this._IsJoinEnabled != value))
                {
                    this.OnIsJoinEnabledChanging(value);
                    this.SendPropertyChanging();
                    this._IsJoinEnabled = value;
                    this.SendPropertyChanged("IsJoinEnabled");
                    this.OnIsJoinEnabledChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_JoinChannels", DbType = "NVarChar(256)")]
        public string JoinChannels
        {
            get
            {
                return this._JoinChannels;
            }
            set
            {
                if ((this._JoinChannels != value))
                {
                    this.OnJoinChannelsChanging(value);
                    this.SendPropertyChanging();
                    this._JoinChannels = value;
                    this.SendPropertyChanged("JoinChannels");
                    this.OnJoinChannelsChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Channel", Storage = "_Channels", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
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

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Mention", Storage = "_Mentions", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
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

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Message", Storage = "_Messages", ThisKey = "Id", OtherKey = "NetworkId", DeleteRule = "NO ACTION")]
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

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Server", Storage = "_Server", ThisKey = "Id", OtherKey = "Id", IsUnique = true, IsForeignKey = false, DeleteRule = "NO ACTION")]
        public Server Server
        {
            get
            {
                return this._Server.Entity;
            }
            set
            {
                Server previousValue = this._Server.Entity;
                if (((previousValue != value)
                            || (this._Server.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if ((previousValue != null))
                    {
                        this._Server.Entity = null;
                        previousValue.Network = null;
                    }
                    this._Server.Entity = value;
                    if ((value != null))
                    {
                        value.Network = this;
                    }
                    this.SendPropertyChanged("Server");
                }
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
    }
}