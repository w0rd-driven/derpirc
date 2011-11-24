using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using derpirc.Data;
using derpirc.Data.Models;
using IrcDotNet;
using IrcDotNet.Ctcp;

namespace derpirc.Core
{
    public class SupervisorFacade : IDisposable
    {
        #region Properties

        private ObservableCollection<ClientItem> _clients;
        public ObservableCollection<ClientItem> Clients
        {
            get { return _clients; }
            set { _clients = value; }
        }

        private List<ConnectionInfo> _connections;
        public List<ConnectionInfo> Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        #endregion

        private bool _isDisposed;
        private object _threadLock = new object();

        private IrcClientSupervisor _clientSupervisor;
        private LocalUserSupervisor _luserSupervisor;

        // HACK: UI facing
        public event EventHandler<MessageRemovedEventArgs> MessageRemoved;
        public event EventHandler<ClientStatusEventArgs> StateChanged;

        public event EventHandler<ChannelStatusEventArgs> ChannelJoined;
        public event EventHandler<ChannelStatusEventArgs> ChannelLeft;
        public event EventHandler<MessageItemEventArgs> ChannelItemReceived;
        public event EventHandler<MessageItemEventArgs> MentionItemReceived;
        public event EventHandler<MessageItemEventArgs> MessageItemReceived;

        #region Singleton Impl

        // Modified for http://www.yoda.arachsys.com/csharp/singleton.html #4. (Jon Skeet is a code machine)
        private static readonly SupervisorFacade defaultInstance = new SupervisorFacade();

        public static SupervisorFacade Default
        {
            get
            {
                return defaultInstance;
            }
        }

        static SupervisorFacade()
        {
        }

        #endregion

        private SupervisorFacade()
        {
            this._clients = new ObservableCollection<ClientItem>();
            this._connections = new List<ConnectionInfo>();
            this.Startup();
        }

        private void Startup()
        {
            lock (_threadLock)
            {
                this._clientSupervisor = new IrcClientSupervisor();
                this._clientSupervisor.StateChanged += this._clientSupervisor_StateChanged;
                this._clientSupervisor.MessageRemoved += this._clientSupervisor_MessageRemoved;
            }
        }

        private void _clientSupervisor_StateChanged(object sender, ClientStatusEventArgs e)
        {
            switch (e.Info.State)
            {
                case ClientState.Registered:
                    var attachClient = GetIrcClientByClientInfo(e.Info);
                    this.AttachLocalUser(attachClient.LocalUser);
                    break;
                case ClientState.Processed:
                    this.AddOrUpdateConnection(e.Info.NetworkName, true);
                    break;
                case ClientState.Disconnected:
                    var detachClient = GetClientByClientInfo(e.Info);
                    this.DetachLocalUser(detachClient.Client.LocalUser);
                    this._clientSupervisor.Disconnect(detachClient);
                    this.AddOrUpdateConnection(e.Info.NetworkName, false);
                    break;
            }
            this.OnClientStatusChanged(e);
        }

        private void _clientSupervisor_MessageRemoved(object sender, MessageRemovedEventArgs e)
        {
            this.OnMessageRemoved(e);
        }

        private void Shutdown()
        {
            this._clientSupervisor = null;
            this._luserSupervisor = null;
        }

        private void AddOrUpdateConnection(string networkName, bool isConnected, string channelName = null)
        {
            if (!string.IsNullOrEmpty(networkName))
            {
                var foundConnection = this.Connections.FirstOrDefault(x => x.NetworkName == networkName);
                if (foundConnection != null)
                {
                    foundConnection.IsConnected = isConnected;
                    if (!string.IsNullOrEmpty(channelName))
                        foundConnection.AddOrUpdateChannel(channelName, isConnected);
                }
                else
                {
                    this.Connections.Add(new ConnectionInfo()
                    {
                        NetworkName = networkName,
                        IsConnected = isConnected,
                    });
                }
            }
        }

        private void RemoveConnection(string networkName, bool isConnected, string channelName = null)
        {
            if (!string.IsNullOrEmpty(networkName))
            {
                var foundConnection = this.Connections.FirstOrDefault(x => x.NetworkName == networkName);
                if (foundConnection != null)
                {
                    if (string.IsNullOrEmpty(channelName))
                        foundConnection.RemoveChannel(channelName);
                    else
                        this.Connections.Remove(foundConnection);
                }
            }
        }

        #region UI-facing methods

        // SettingsView
        public void CommitSettings()
        {
            ThreadPool.QueueUserWorkItem((object userState) =>
            {
                var settingsSupervisorReference = new WeakReference(new SettingsSupervisor());
                var settingsSupervisor = settingsSupervisorReference.Target as SettingsSupervisor;
                settingsSupervisor.Commit(settingsToSync =>
                {
                    this._clientSupervisor.ReinitializeClients(settingsToSync);
                });
            });
        }

        // ConnectionView
        public void Disconnect(ObservableCollection<ClientInfo> clients)
        {
            if (_clientSupervisor != null)
                if (clients != null)
                    for (int index = 0; index < clients.Count; index++)
                    {
                        var client = GetClientByClientInfo(clients[index]);
                        this._clientSupervisor.Disconnect(client);
                    }
                else
                    this._clientSupervisor.Disconnect();
        }

        public void Reconnect(ObservableCollection<ClientInfo> clients, bool queue = false, bool force = false)
        {
            if (_clientSupervisor != null)
                if (clients != null)
                    for (int index = 0; index < clients.Count; index++)
                    {
                        var client = GetClientByClientInfo(clients[index]);
                        this._clientSupervisor.Reconnect(client);
                    }
                else
                    this._clientSupervisor.Reconnect(queue, force);
        }

        // *DetailsView
        public void SendMessage(ChannelItem message)
        {
            if (_luserSupervisor != null)
                this._luserSupervisor.SendMessage(message.Summary, message.Text);
        }

        public void SendMessage(MentionItem message)
        {
            if (_luserSupervisor != null)
                this._luserSupervisor.SendMessage(message.Summary, message.Text);
        }

        public void SendMessage(MessageItem message)
        {
            if (_luserSupervisor != null)
                this._luserSupervisor.SendMessage(message.Summary, message.Text);
        }

        public void SetNickName(IMessage target, string nickName)
        {
            if (_clientSupervisor != null)
                this._clientSupervisor.SetNickName(target, nickName);
        }

        public void SendAction(IMessage target, string message)
        {
            if (_clientSupervisor != null)
                this._clientSupervisor.SendAction(target, message);
        }

        public void SetTopic(IMessage target, string topic)
        {
            if (_luserSupervisor != null)
                this._luserSupervisor.SetTopic(target, topic);
        }

        public void SetModes(IMessage target, string modes)
        {
            if (_luserSupervisor != null)
                this._luserSupervisor.SetModes(target, modes);
        }

        #endregion

        #region Events

        private void OnClientStatusChanged(ClientStatusEventArgs eventArgs)
        {
            var handler = this.StateChanged;
            if (handler != null)
            {
                handler.Invoke(this, eventArgs);
            }
        }

        private void OnMessageRemoved(MessageRemovedEventArgs eventArgs)
        {
            this.AddOrUpdateConnection(eventArgs.NetworkName, false, eventArgs.FavoriteName);
            var handler = this.MessageRemoved;
            if (handler != null)
            {
                handler.Invoke(this, eventArgs);
            }
        }

        private void OnChannelSupervisor_ChannelJoined(object sender, ChannelStatusEventArgs eventArgs)
        {
            this.AddOrUpdateConnection(eventArgs.NetworkName, true, eventArgs.ChannelName);
            var handler = this.ChannelJoined;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        private void OnChannelSupervisor_ChannelLeft(object sender, ChannelStatusEventArgs eventArgs)
        {
            this.AddOrUpdateConnection(eventArgs.NetworkName, false, eventArgs.ChannelName);
            var handler = this.ChannelLeft;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        private void OnChannelSupervisor_MessageReceived(object sender, MessageItemEventArgs eventArgs)
        {
            var handler = this.ChannelItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        private void OnChannelSupervisor_MentionReceived(object sender, MessageItemEventArgs eventArgs)
        {
            var handler = this.MentionItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        private void OnMessageSupervisor_MessageReceived(object sender, MessageItemEventArgs eventArgs)
        {
            var handler = this.MessageItemReceived;
            if (handler != null)
            {
                handler.Invoke(sender, eventArgs);
            }
        }

        #endregion

        #region Lookup methods

        public IrcLocalUser GetLocalUserBySummary(IMessage summary)
        {
            IrcLocalUser result = null;
            result = (from client in this._clients
                      where client.Info.NetworkName == summary.Network.Name
                      select client.Client.LocalUser).FirstOrDefault();
            return result;
        }

        public IrcChannel GetIrcChannelBySummary(IMessage summary)
        {
            IrcChannel result = null;
            result = (from client in this._clients
                      from channel in client.Client.Channels
                      where client.Info.NetworkName == summary.Network.Name
                      where channel.Name == summary.Name
                      select channel).FirstOrDefault();
            return result;
        }

        public IrcClient GetIrcClientBySummary(IMessage summary)
        {
            IrcClient result = null;
            result = (from client in this._clients
                      where client.Info.NetworkName == summary.Network.Name
                      select client.Client).FirstOrDefault();
            return result;
        }

        public CtcpClient GetCtcpClientBySummary(IMessage summary)
        {
            CtcpClient result = null;
            result = (from client in this._clients
                      where client.Info.NetworkName == summary.Network.Name
                      select client.CtcpClient).FirstOrDefault();
            return result;
        }

        public ClientItem GetClientByClientInfo(ClientInfo summary)
        {
            ClientItem result = null;
            result = (from client in this._clients
                      where client.Info.NetworkName == summary.NetworkName
                      select client).FirstOrDefault();
            return result;
        }

        public IrcClient GetIrcClientByClientInfo(ClientInfo summary)
        {
            IrcClient result = null;
            result = (from client in this._clients
                      where client.Info.NetworkName == summary.NetworkName
                      select client.Client).FirstOrDefault();
            return result;
        }

        public ClientItem GetClientByIrcClient(IrcClient client)
        {
            ClientItem result = null;
            if (client != null)
            {
                result = (from clientItem in this._clients
                          where clientItem.Client == client
                          select clientItem).FirstOrDefault();
            }
            return result;
        }

        #endregion

        private void PrepareLocalUser()
        {
            if (this._luserSupervisor == null)
            {
                this._luserSupervisor = new LocalUserSupervisor();
                this._luserSupervisor.ChannelJoined += this.OnChannelSupervisor_ChannelJoined;
                this._luserSupervisor.ChannelLeft += this.OnChannelSupervisor_ChannelLeft;
                this._luserSupervisor.ChannelItemReceived += this.OnChannelSupervisor_MessageReceived;
                this._luserSupervisor.MentionItemReceived += this.OnChannelSupervisor_MentionReceived;
                this._luserSupervisor.MessageItemReceived += this.OnMessageSupervisor_MessageReceived;
            }
        }

        private void AttachLocalUser(IrcLocalUser localUser)
        {
            this.PrepareLocalUser();
            this._luserSupervisor.AttachLocalUser(localUser);
        }

        private void DetachLocalUser(IrcLocalUser localUser)
        {
            if (this._luserSupervisor != null)
                this._luserSupervisor.DetachLocalUser(localUser);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing)
                {
                    this.Shutdown();
                }
            }
            this._isDisposed = true;
        }
    }
}
