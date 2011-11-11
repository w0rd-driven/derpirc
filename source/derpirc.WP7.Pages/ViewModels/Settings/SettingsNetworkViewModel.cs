using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using derpirc.Core;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using derpirc.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace derpirc.ViewModels
{
    public class SettingsNetworkViewModelFactory : ViewModelFactory<SettingsNetworkViewModel, SettingsNetworkViewModel> { }

    public class SettingsNetworkViewModel : ViewModelBase
    {
        #region Commands

        private bool _canAdd;
        public bool CanAdd
        {
            get { return _canAdd; }
            set
            {
                if (_canAdd == value)
                    return;

                _canAdd = value;
                RaisePropertyChanged(() => CanAdd);
                this.MessengerInstance.Send<NotificationMessage<bool>>
                    (new NotificationMessage<bool>(this, _canAdd, "add"), "action");
            }
        }

        private bool _canClear;
        public bool CanClear
        {
            get { return _canClear; }
            set
            {
                if (_canClear == value)
                    return;

                _canClear = value;
                RaisePropertyChanged(() => CanClear);
                this.MessengerInstance.Send<NotificationMessage<bool>>
                    (new NotificationMessage<bool>(this, _canClear, "clear"), "action");
            }
        }

        #endregion

        #region Properties

        private readonly INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return this._navigationService; }
        }

        private ObservableCollection<Network> _networksList;
        public CollectionViewSource Networks { get; set; }

        private Network _selectedItem;
        public Network SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
                SelectItem();
            }
        }

        #endregion

        private Network _addingRecord;

        public SettingsNetworkViewModel() : this(null) { }

        public SettingsNetworkViewModel(INavigationService navigationService)
        {
            _networksList = new ObservableCollection<Network>();
            Networks = new CollectionViewSource() { Source = _networksList };

            CanAdd = true;

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                _navigationService = navigationService;

                this.MessengerInstance.Register<NotificationMessage<Network>>(this, "Network", message =>
                {
                    switch (message.Notification)
                    {
                        case "unselect":
                            this.UnselectItem();
                            break;
                        case "add":
                            this.Add();
                            break;
                        case "delete":
                            this.Delete(message.Content);
                            break;
                        case "clear":
                            this.Clear();
                            break;
                    }
                });

                this.MessengerInstance.Register<NotificationMessage<bool>>(this, "Save", message =>
                {
                    this.Save(message.Content);
                });

                Load();
            }
        }

        private void Load()
        {
            var networks = SettingsUnitOfWork.Default.Networks;
            _networksList.Clear();
            foreach (var item in networks)
            {
                _networksList.Add(item);
            }
            if (_networksList.Count > 0)
                CanClear = true;
        }

        private void Add()
        {
            var count = SettingsUnitOfWork.Default.Networks.Last().Id;
            // We're playing throw the record. THROW
            _addingRecord = new Network();
            _addingRecord.Id = count + 1;
            _addingRecord.Name = "New Network";
            _addingRecord.HostName = "newnetwork";
            _addingRecord.Ports = "6667";
            SettingsUnitOfWork.Default.Networks.Add(_addingRecord);
            CanAdd = false;
            ViewDetails(_addingRecord);
        }

        private void Delete(Network item)
        {
            if (_networksList.Contains(item))
                _networksList.Remove(item);
        }

        private void Clear()
        {
            _networksList.Clear();
        }

        private void SelectItem()
        {
            if (SelectedItem != null)
                ViewDetails(SelectedItem);
        }

        private void UnselectItem()
        {
            // We're playing throw the record. CATCH & SHIT ON IT
            if (_addingRecord != null)
            {
                SettingsUnitOfWork.Default.Networks.Remove(_addingRecord);
                _networksList.Add(_addingRecord);
                _addingRecord = null;
                CanAdd = true;
            }
            this.SelectedItem = null;
        }

        private void ViewDetails(Network item)
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (item != null)
                id = item.Id.ToString();
            if (!string.IsNullOrEmpty(id))
            {
                uriString = string.Format("/derpirc.Pages;component/Views/NetworkDetailView.xaml?id={0}", Uri.EscapeUriString(id));
                var uri = new Uri(uriString, UriKind.Relative);
                NavigationService.Navigate(uri);
            }
        }

        private void Save(bool commit)
        {
            SettingsUnitOfWork.Default.Networks = _networksList.ToList();
            if (commit)
                SupervisorFacade.Default.CommitSettings();
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}