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
using System.Linq;

namespace derpirc.Data.Models
{
    [global::System.Data.Linq.Mapping.TableAttribute()]
    public partial class Message : IBaseModel, IMessage, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private int _NetworkId;

        private string _Name;

        private System.Nullable<int> _LastItemId;

        private System.Nullable<int> _Count;

        private System.Nullable<int> _UnreadCount;

        private EntitySet<MessageItem> _Messages;

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
        partial void OnLastItemIdChanging(System.Nullable<int> value);
        partial void OnLastItemIdChanged();
        partial void OnCountChanging(System.Nullable<int> value);
        partial void OnCountChanged();
        partial void OnUnreadCountChanging(System.Nullable<int> value);
        partial void OnUnreadCountChanged();
        #endregion

        public Message()
        {
            this._Messages = new EntitySet<MessageItem>(new Action<MessageItem>(this.attach_MessageItems), new Action<MessageItem>(this.detach_MessageItems));
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
                    if (this._Network.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.OnNetworkIdChanging(value);
                    this.SendPropertyChanging();
                    this._NetworkId = value;
                    this.SendPropertyChanged("NetworkId");
                    this.OnNetworkIdChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Name", DbType = "NVarChar(64) NOT NULL", CanBeNull = false)]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LastItemId", DbType = "Int")]
        public System.Nullable<int> LastItemId
        {
            get
            {
                return this._LastItemId;
            }
            set
            {
                if ((this._LastItemId != value))
                {
                    this.OnLastItemIdChanging(value);
                    this.SendPropertyChanging();
                    this._LastItemId = value;
                    this.SendPropertyChanged("LastItemId");
                    this.OnLastItemIdChanged();
                }
            }
        }

        public IMessageItem LastItem { get; set; }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Count", DbType = "Int")]
        public System.Nullable<int> Count
        {
            get
            {
                return this._Count;
            }
            set
            {
                if ((this._Count != value))
                {
                    this.OnCountChanging(value);
                    this.SendPropertyChanging();
                    this._Count = value;
                    this.SendPropertyChanged("Count");
                    this.OnCountChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_UnreadCount", DbType = "Int")]
        public System.Nullable<int> UnreadCount
        {
            get
            {
                return this._UnreadCount;
            }
            set
            {
                if ((this._UnreadCount != value))
                {
                    this.OnUnreadCountChanging(value);
                    this.SendPropertyChanging();
                    this._UnreadCount = value;
                    this.SendPropertyChanged("UnreadCount");
                    this.OnUnreadCountChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Message_MessageItem", Storage = "_Messages", ThisKey = "Id", OtherKey = "SummaryId", DeleteRule = "NO ACTION")]
        public EntitySet<MessageItem> Messages
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

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Network_Message", Storage = "_Network", ThisKey = "NetworkId", OtherKey = "Id", IsForeignKey = true)]
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
                        previousValue.Messages.Remove(this);
                    }
                    this._Network.Entity = value;
                    if ((value != null))
                    {
                        value.Messages.Add(this);
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

        private void attach_MessageItems(MessageItem entity)
        {
            this.SendPropertyChanging();
            entity.Summary = this;
            InsertMessageCount(entity);
        }

        private void detach_MessageItems(MessageItem entity)
        {
            this.SendPropertyChanging();
            entity.Summary = null;
            RemoveMessageCount(entity);
        }

        private void InsertMessageCount(MessageItem entity)
        {
            LastItem = entity;
            // Id is 0 here so inflate counts blindly
            LastItemId = _Messages.Count + 1;
            Count = _Messages.Count + 1;
            UnreadCount = _Messages.Count(x => x.IsRead == false) + 1;
        }

        private void RemoveMessageCount(MessageItem entity)
        {
            LastItem = entity;
            // Id is 0 here so inflate counts blindly
            LastItemId = _Messages.Count - 1;
            Count = _Messages.Count - 1;
            UnreadCount = _Messages.Count(x => x.IsRead == false) - 1;
        }
    }
}
