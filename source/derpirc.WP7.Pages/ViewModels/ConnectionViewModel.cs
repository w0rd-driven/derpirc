﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using derpirc.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace derpirc.ViewModels
{
    public class ConnectionViewModel : ViewModelBase
    {
        #region Commands

        RelayCommand<FrameworkElement> _layoutRootCommand;
        public RelayCommand<FrameworkElement> LayoutRootCommand
        {
            get
            {
                return _layoutRootCommand ?? (_layoutRootCommand =
                    new RelayCommand<FrameworkElement>(sender => this.LayoutRoot = sender));
            }
        }

        RelayCommand _navigatedToCommand;
        public RelayCommand NavigatedToCommand
        {
            get
            {
                return _navigatedToCommand ?? (_navigatedToCommand =
                    new RelayCommand(() => this.OnNavigatedTo()));
            }
        }

        RelayCommand _reconnectCommand;
        public RelayCommand ReconnectCommand
        {
            get
            {
                return _reconnectCommand ?? (_reconnectCommand =
                    new RelayCommand(() => this.Reconnect()));
            }
        }

        RelayCommand _disconnectCommand;
        public RelayCommand DisconnectCommand
        {
            get
            {
                return _disconnectCommand ?? (_disconnectCommand =
                    new RelayCommand(() => this.Disconnect()));
            }
        }

        #endregion

        #region Properties

        public FrameworkElement LayoutRoot { get; set; }

        private ObservableCollection<ClientInfo> _connectionsList;
        public CollectionViewSource Connections { get; set; }

        private ObservableCollection<ClientInfo> _selectedItems;
        public ObservableCollection<ClientInfo> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                if (_selectedItems == value)
                    return;

                _selectedItems = value;
                RaisePropertyChanged(() => SelectedItems);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the ConnectionViewModel class.
        /// </summary>
        public ConnectionViewModel()
        {
            _connectionsList = new ObservableCollection<ClientInfo>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                _connectionsList.Add(new ClientInfo()
                {
                    Id = 1,
                    NetworkName = "clefnet",
                    State = ClientState.Connected,
                });
                _connectionsList.Add(new ClientInfo()
                {
                    Id = 2,
                    NetworkName = "peenode",
                    State = ClientState.Processed,
                });
                _connectionsList.Add(new ClientInfo()
                {
                    Id = 3,
                    NetworkName = "palnet",
                    State = ClientState.Disconnected,
                });
                _connectionsList.Add(new ClientInfo()
                {
                    Id = 4,
                    NetworkName = "urmom",
                    State = ClientState.Intervention,
                });
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                Load();
            }

            Connections = new CollectionViewSource() { Source = _connectionsList };
        }

        private void OnNavigatedTo()
        {

        }

        private void OnNavigatedFrom()
        {

        }

        private void Load()
        {
            _connectionsList.Clear();
            foreach (var item in SupervisorFacade.Default.Clients)
            {
                _connectionsList.Add(item.Info);
            }

            SupervisorFacade.Default.ClientStatusChanged += (s, e) => 
            {
                var foundClient = _connectionsList.FirstOrDefault(x => x.Id == e.Info.Id);
                if (foundClient == null)
                    _connectionsList.Add(e.Info);
                else
                {
                    foundClient = e.Info;
                }
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Connections.View.Refresh();
                });
            };
        }

        private void Reconnect()
        {

        }

        private void Disconnect()
        {

        }
    }
}
