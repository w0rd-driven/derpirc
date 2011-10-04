﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using derpirc.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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

        #endregion

        #region Properties

        public FrameworkElement LayoutRoot { get; set; }

        private ObservableCollection<ClientItem> _connectionsList;
        public CollectionViewSource Connections { get; set; }

        private ObservableCollection<ClientItem> _selectedItems;
        public ObservableCollection<ClientItem> SelectedItems
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
            _connectionsList = new ObservableCollection<ClientItem>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                _connectionsList.Add(new ClientItem()
                {
                    Id = 1,
                    NetworkName = "clefnet",
                    State = ClientState.Connected,
                });
                _connectionsList.Add(new ClientItem()
                {
                    Id = 2,
                    NetworkName = "peenode",
                    State = ClientState.Processed,
                });
                _connectionsList.Add(new ClientItem()
                {
                    Id = 3,
                    NetworkName = "palnet",
                    State = ClientState.Disconnected,
                });
                _connectionsList.Add(new ClientItem()
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
                _connectionsList.Add(item);
            }
        }
    }
}