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
    public partial class ChannelItem : IBaseModel, IMessageItem, INotifyPropertyChanging, INotifyPropertyChanged
    {
        [Column(IsVersion = true)]
        private Binary _Version;

        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int _Id;

        private int _SummaryId;

        private string _Source;

        private string _Text;

        private Owner _Owner;

        private System.DateTime _Timestamp;

        private bool _IsRead;

        private EntityRef<Channel> _Summary;

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();
        partial void OnIdChanging(int value);
        partial void OnIdChanged();
        partial void OnSummaryIdChanging(int value);
        partial void OnSummaryIdChanged();
        partial void OnSourceChanging(string value);
        partial void OnSourceChanged();
        partial void OnTextChanging(string value);
        partial void OnTextChanged();
        partial void OnOwnerChanging(Owner value);
        partial void OnOwnerChanged();
        partial void OnTimestampChanging(System.DateTime value);
        partial void OnTimestampChanged();
        partial void OnIsReadChanging(bool value);
        partial void OnIsReadChanged();
        #endregion

        public ChannelItem()
        {
            this._Summary = default(EntityRef<Channel>);
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_SummaryId", DbType = "Int NOT NULL")]
        public int SummaryId
        {
            get
            {
                return this._SummaryId;
            }
            set
            {
                if ((this._SummaryId != value))
                {
                    if (this._Summary.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }
                    this.OnSummaryIdChanging(value);
                    this.SendPropertyChanging();
                    this._SummaryId = value;
                    this.SendPropertyChanged("SummaryId");
                    this.OnSummaryIdChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Source", DbType = "NVarChar(64)")]
        public string Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                if ((this._Source != value))
                {
                    this.OnSourceChanging(value);
                    this.SendPropertyChanging();
                    this._Source = value;
                    this.SendPropertyChanged("Source");
                    this.OnSourceChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Text", DbType = "NVarChar(256)")]
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                if ((this._Text != value))
                {
                    this.OnTextChanging(value);
                    this.SendPropertyChanging();
                    this._Text = value;
                    this.SendPropertyChanged("Text");
                    this.OnTextChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Owner", DbType = "Int NOT NULL")]
        public Owner Owner
        {
            get
            {
                return this._Owner;
            }
            set
            {
                if ((this._Owner != value))
                {
                    this.OnOwnerChanging(value);
                    this.SendPropertyChanging();
                    this._Owner = value;
                    this.SendPropertyChanged("Owner");
                    this.OnOwnerChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Timestamp", DbType = "DateTime NOT NULL")]
        public System.DateTime Timestamp
        {
            get
            {
                return this._Timestamp;
            }
            set
            {
                if ((this._Timestamp != value))
                {
                    this.OnTimestampChanging(value);
                    this.SendPropertyChanging();
                    this._Timestamp = value;
                    this.SendPropertyChanged("Timestamp");
                    this.OnTimestampChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_IsRead", DbType = "Bit")]
        public bool IsRead
        {
            get
            {
                return this._IsRead;
            }
            set
            {
                if ((this._IsRead != value))
                {
                    this.OnIsReadChanging(value);
                    this.SendPropertyChanging();
                    this._IsRead = value;
                    this.SendPropertyChanged("IsRead");
                    this.OnIsReadChanged();
                }
            }
        }

        [global::System.Data.Linq.Mapping.AssociationAttribute(Name = "FK_Channel_ChannelItem", Storage = "_Summary", ThisKey = "SummaryId", OtherKey = "Id", IsForeignKey = true)]
        public Channel Summary
        {
            get
            {
                return this._Summary.Entity;
            }
            set
            {
                Channel previousValue = this._Summary.Entity;
                if (((previousValue != value)
                            || (this._Summary.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if ((previousValue != null))
                    {
                        this._Summary.Entity = null;
                        previousValue.Messages.Remove(this);
                    }
                    this._Summary.Entity = value;
                    if ((value != null))
                    {
                        value.Messages.Add(this);
                        this._SummaryId = value.Id;
                    }
                    else
                    {
                        this._SummaryId = default(int);
                    }
                    this.SendPropertyChanged("Summary");
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
