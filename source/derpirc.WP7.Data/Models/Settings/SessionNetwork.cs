﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace derpirc.Data.Models.Settings
{
    [Table]
    public partial class SessionNetwork : BaseNotify, IBaseModel
    {
        [Column(IsVersion = true)]
        private Binary version;
        private EntityRef<Session> _session;
        private EntityRef<Network> _network;
        private EntityRef<SessionServer> _server;

        #region Primitive Properties

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Name { get; set; }
        [Column(CanBeNull = true)]
        public bool IsJoinEnabled { get; set; }
        [Column(CanBeNull = true)]
        public string JoinChannels { get; set; }

        #endregion

        #region Navigation Properties

        [Column(CanBeNull = false)]
        public int SessionId { get; set; }
        [Association(Name = "Session_Item", ThisKey = "SessionId", OtherKey = "Id", IsForeignKey = true)]
        public Session Session
        {
            get { return _session.Entity; }
            set
            {
                Session previousValue = _session.Entity;
                if (previousValue != value || _session.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _session.Entity = null;
                    }
                    _session.Entity = value;
                    if ((value != null))
                    {
                        SessionId = value.Id;
                    }
                    else
                    {
                        SessionId = default(int);
                    }
                    this.RaisePropertyChanged(() => SessionId);
                    this.RaisePropertyChanged(() => Session);
                }
            }
        }

        [Column(CanBeNull = true)]
        public int BasedOnId { get; set; }
        [Association(Name = "Network_Item", ThisKey = "BasedOnId", OtherKey = "Id", IsForeignKey = true)]
        public Network Network
        {
            get
            {
                return this._network.Entity;
            }
            set
            {
                Network previousValue = this._network.Entity;
                if (previousValue != value || this._network.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        this._network.Entity = null;
                    }
                    this._network.Entity = value;
                    if ((value != null))
                    {
                        this.BasedOnId = value.Id;
                    }
                    else
                    {
                        this.BasedOnId = default(int);
                    }
                    this.RaisePropertyChanged(() => BasedOnId);
                    this.RaisePropertyChanged(() => Network);
                }
            }
        }

        [Column(CanBeNull = false)]
        public int ServerId { get; set; }
        [Association(Name = "Server_Item", ThisKey = "ServerId", OtherKey = "Id", IsForeignKey = true)]
        public SessionServer Server
        {
            get { return _server.Entity; }
            set
            {
                SessionServer previousValue = _server.Entity;
                if (previousValue != value || _server.HasLoadedOrAssignedValue == false)
                {
                    this.RaisePropertyChanged();
                    if ((previousValue != null))
                    {
                        _server.Entity = null;
                    }
                    _server.Entity = value;
                    if ((value != null))
                    {
                        ServerId = value.Id;
                    }
                    else
                    {
                        ServerId = default(int);
                    }
                    this.RaisePropertyChanged(() => ServerId);
                    this.RaisePropertyChanged(() => Server);
                }
            }
        }

        #endregion

        public SessionNetwork()
        {
            _session = default(EntityRef<Session>);
            _network = default(EntityRef<Network>);
            _server = default(EntityRef<SessionServer>);
        }
    }
}