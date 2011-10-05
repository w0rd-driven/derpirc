using System;
using GalaSoft.MvvmLight;
using IrcDotNet;

namespace derpirc.Core
{
    public class ClientItem : ObservableObject
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id == value)
                    return;

                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private string _networkName;
        public string NetworkName
        {
            get { return _networkName; }
            set
            {
                if (_networkName == value)
                    return;

                _networkName = value;
                RaisePropertyChanged(() => NetworkName);
            }
        }

        private IrcClient _client;
        public IrcClient Client
        {
            get { return _client; }
            set
            {
                if (_client == value)
                    return;

                _client = value;
                RaisePropertyChanged(() => Client);
            }
        }

        private ClientState _state;
        public ClientState State
        {
            get { return _state; }
            set
            {
                if (_state == value)
                    return;

                _state = value;
                RaisePropertyChanged(() => State);
            }
        }

        private Exception _error;
        public Exception Error
        {
            get { return _error; }
            set
            {
                if (_error == value)
                    return;

                _error = value;
                RaisePropertyChanged(() => Error);
            }
        }

        public ClientItem()
        {
            Client = new IrcClient();
            State = ClientState.Inconceivable;
        }
    }
}
