using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        RelayCommand<IList> _selectionChangedToCommand;
        public RelayCommand<IList> SelectionChangedCommand
        {
            get
            {
                return _selectionChangedToCommand ?? (_selectionChangedToCommand =
                    new RelayCommand<IList>((items) => this.SelectionChanged(items)));
            }
        }

        RelayCommand _selectCommand;
        public RelayCommand SelectCommand
        {
            get
            {
                return _selectCommand ?? (_selectCommand =
                    new RelayCommand(() => this.Select()));
            }
        }

        RelayCommand _connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                return _connectCommand ?? (_connectCommand =
                    new RelayCommand(() => this.Connect(SelectedItems)));
            }
        }

        private bool _canDisconnect;
        public bool CanDisconnect
        {
            get { return _canDisconnect; }
            set
            {
                if (_canDisconnect == value)
                    return;

                _canDisconnect = value;
                DisconnectCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => CanDisconnect);
            }
        }

        RelayCommand _disconnectCommand;
        public RelayCommand DisconnectCommand
        {
            get
            {
                return _disconnectCommand ?? (_disconnectCommand =
                    new RelayCommand(() => this.Disconnect(SelectedItems)));
            }
        }

        private bool _canReconnect;
        public bool CanReconnect
        {
            get { return _canReconnect; }
            set
            {
                if (_canReconnect == value)
                    return;

                _canReconnect = value;
                ReconnectCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => CanReconnect);
            }
        }

        RelayCommand _reconnectCommand;
        public RelayCommand ReconnectCommand
        {
            get
            {
                return _reconnectCommand ?? (_reconnectCommand =
                    new RelayCommand(() => this.Reconnect(SelectedItems)));
            }
        }

        #endregion

        #region Properties

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                if (_pageTitle == value)
                    return;

                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        private string _pageSubTitle;
        public string PageSubTitle
        {
            get { return _pageSubTitle; }
            set
            {
                if (_pageSubTitle == value)
                    return;

                _pageSubTitle = value;
                RaisePropertyChanged(() => PageSubTitle);
            }
        }

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

        private ClientInfo _selectedItem;
        public ClientInfo SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        private bool _isSelectionEnabled;
        public bool IsSelectionEnabled
        {
            get { return _isSelectionEnabled; }
            set
            {
                if (_isSelectionEnabled == value)
                    return;

                _isSelectionEnabled = value;
                RaisePropertyChanged(() => IsSelectionEnabled);
            }
        }

        #endregion

        private object _threadLock = new object();
        private BackgroundWorker _worker;

        /// <summary>
        /// Initializes a new instance of the ConnectionViewModel class.
        /// </summary>
        public ConnectionViewModel()
        {
            // TODO: Localize
            PageTitle = "connections";

            _connectionsList = new ObservableCollection<ClientInfo>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                _connectionsList.Add(new ClientInfo()
                {
                    Id = 1,
                    NetworkName = "clefnet",
                    State = ClientState.Connected,
                    Error = new Exception("This is a test."),
                });
                _connectionsList.Add(new ClientInfo()
                {
                    Id = 2,
                    NetworkName = "peenode",
                    State = ClientState.Processed,
                    Error = new Exception("This is only a test."),
                });
                _connectionsList.Add(new ClientInfo()
                {
                    Id = 3,
                    NetworkName = "palnet",
                    State = ClientState.Disconnected,
                    Error = new Exception("If this were an actual emergency, you'd be given a bunch of instructions herpderp."),
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
                _worker = new BackgroundWorker();
                _worker.DoWork += new DoWorkEventHandler(DeferStartupWork);

                DeferStartup(null);
            }

            Connections = new CollectionViewSource() { Source = _connectionsList };
        }

        internal void DeferStartup(Action completed)
        {
            _worker.RunWorkerAsync(completed);
        }

        private void DeferStartupWork(object sender, DoWorkEventArgs e)
        {
            Action completed = e.Argument as Action;
            lock (_threadLock)
            {
                Load();
            }

            if (completed != null)
            {
                completed();
            }
        }

        private void OnNavigatedTo()
        {

        }

        private void OnNavigatedFrom()
        {

        }

        private void SelectionChanged(IList items)
        {
            // Set SelectedItems
            if (SelectedItems == null)
                SelectedItems = new ObservableCollection<ClientInfo>();
            SelectedItems.Clear();
            var list = items.Cast<ClientInfo>().ToList();
            if (list.Count > 0)
            {
                list.ForEach(item => SelectedItems.Add(item));
                // Set SelectedItem
                if (list[0] != null)
                    SelectedItem = list[0];
                CanDisconnect = true;
                CanReconnect = true;
            }
            else
            {
                CanDisconnect = false;
                CanReconnect = false;
            }
        }

        private void Load()
        {
            _connectionsList.Clear();
            foreach (var item in SupervisorFacade.Default.Clients)
            {
                _connectionsList.Add(item.Info);
            }

            SupervisorFacade.Default.ClientStatusChanged += new EventHandler<ClientStatusEventArgs>(Default_ClientStatusChanged);
        }

        private void Default_ClientStatusChanged(object sender, ClientStatusEventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var foundClient = _connectionsList.FirstOrDefault(x => x.Id == e.Info.Id);
                if (foundClient == null)
                {
                    // Wait for Collection/PropertyChanged event
                    _connectionsList.Add(e.Info);
                }
                else
                    foundClient = e.Info;

                Connections.View.Refresh();
            });
        }

        private void Select()
        {
            IsSelectionEnabled = !IsSelectionEnabled;
        }

        private void Connect(ObservableCollection<ClientInfo> clients)
        {
            SupervisorFacade.Default.Connect(clients);
        }

        private void Disconnect(ObservableCollection<ClientInfo> clients)
        {
            SupervisorFacade.Default.Disconnect(clients);
        }

        private void Reconnect(ObservableCollection<ClientInfo> clients)
        {
            SupervisorFacade.Default.Reconnect(clients);
        }
    }
}
