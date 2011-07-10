﻿using System.Data.Linq;

namespace derpirc.Data
{
    public class DataUnitOfWork : IUnitOfWork
    {
        readonly DataContext _context;

        SqlRepository<ChannelSummary> _channels = null;
        public IRepository<ChannelSummary> Channels
        {
            get
            {
                if (_channels == null)
                {
                    _channels = new SqlRepository<ChannelSummary>(_context);
                }
                return _channels;
            }
        }

        SqlRepository<ChannelDetail> _channelDetails = null;
        public IRepository<ChannelDetail> ChannelDetails
        {
            get
            {
                if (_channelDetails == null)
                {
                    _channelDetails = new SqlRepository<ChannelDetail>(_context);
                }
                return _channelDetails;
            }
        }

        SqlRepository<ChannelMessage> _channelMessages = null;
        public IRepository<ChannelMessage> ChannelMessages
        {
            get
            {
                if (_channelMessages == null)
                {
                    _channelMessages = new SqlRepository<ChannelMessage>(_context);
                }
                return _channelMessages;
            }
        }

        SqlRepository<MentionSummary> _mentions = null;
        public IRepository<MentionSummary> Mentions
        {
            get
            {
                if (_mentions == null)
                {
                    _mentions = new SqlRepository<MentionSummary>(_context);
                }
                return _mentions;
            }
        }

        SqlRepository<MessageSummary> _messages = null;
        public IRepository<MessageSummary> Messages
        {
            get
            {
                if (_messages == null)
                {
                    _messages = new SqlRepository<MessageSummary>(_context);
                }
                return _messages;
            }
        }

        SqlRepository<Settings.Client> _client = null;
        public IRepository<Settings.Client> Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new SqlRepository<Settings.Client>(_context);
                }
                return _client;
            }
        }

        SqlRepository<Settings.User> _user = null;
        public IRepository<Settings.User> User
        {
            get
            {
                if (_user == null)
                {
                    _user = new SqlRepository<Settings.User>(_context);
                }
                return _user;
            }
        }

        SqlRepository<Settings.Server> _servers = null;
        public IRepository<Settings.Server> Servers
        {
            get
            {
                if (_servers == null)
                {
                    _servers = new SqlRepository<Settings.Server>(_context);
                }
                return _servers;
            }
        }

        SqlRepository<Settings.Network> _networks = null;
        public IRepository<Settings.Network> Networks
        {
            get
            {
                if (_networks == null)
                {
                    _networks = new SqlRepository<Settings.Network>(_context);
                }
                return _networks;
            }
        }

        SqlRepository<Settings.Session> _sessions = null;
        public IRepository<Settings.Session> Sessions
        {
            get
            {
                if (_sessions == null)
                {
                    _sessions = new SqlRepository<Settings.Session>(_context);
                }
                return _sessions;
            }
        }

        public DataUnitOfWork()
        {
            _context = new DataModelContainer();
        }

        public bool InitializeDatabase()
        {
            var context = (_context as DataModelContainer);
            // HACK: Wipe database for testing only. REMEMBER TO REMOVE THIS
            context.InitializeDatabase(true);
            return context.DatabaseExists();
        }

        public void Commit()
        {
            _context.SubmitChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}