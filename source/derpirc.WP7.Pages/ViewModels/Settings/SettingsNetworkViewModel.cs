using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
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

        private bool _canEdit;
        public bool CanEdit
        {
            get { return _canEdit; }
            set
            {
                if (_canEdit == value)
                    return;

                _canEdit = value;
                RaisePropertyChanged(() => CanEdit);
                this.MessengerInstance.Send<NotificationMessage<bool>>(new NotificationMessage<bool>(this, "action", _canEdit, "edit"));
            }
        }

        private bool _canDelete;
        public bool CanDelete
        {
            get { return _canDelete; }
            set
            {
                if (_canDelete == value)
                    return;

                _canDelete = value;
                RaisePropertyChanged(() => CanDelete);
                this.MessengerInstance.Send<NotificationMessage<bool>>(new NotificationMessage<bool>(this, "action", _canDelete, "delete"));
            }
        }

        #endregion

        #region Properties

        private readonly INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get
            {
                return this._navigationService;
            }
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

        public SettingsNetworkViewModel() : this(null) { }

        public SettingsNetworkViewModel(INavigationService navigationService)
        {
            _networksList = new ObservableCollection<Network>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                _navigationService = navigationService;

                this.MessengerInstance.Register<NotificationMessage>(this, "Network", message =>
                {
                    var target = message.Target as string;
                    switch (message.Notification)
                    {
                        case "unselect":
                            this.UnselectItem();
                            break;
                        case "add":
                            this.Add();
                            break;
                        case "edit":
                            this.Edit();
                            break;
                        case "delete":
                            this.Delete(SelectedItem);
                            break;
                    }
                });

                this.MessengerInstance.Register<NotificationMessage>(this, "Save", message =>
                {
                    var target = message.Target as string;
                    this.Save();
                });

                Load();
            }

            Networks = new CollectionViewSource() { Source = _networksList };
        }

        private void Load()
        {
            var networks = SettingsUnitOfWork.Default.Networks;
            _networksList.Clear();
            foreach (var item in networks)
            {
                _networksList.Add(item);
            }
        }

        private void Edit()
        {
            ViewDetails(SelectedItem);
        }

        private void Add()
        {
            var item = new Network();
            _networksList.Add(item);
        }

        private void Delete(Network item)
        {
            if (_networksList.Contains(item))
                _networksList.Remove(item);
        }

        private void SelectItem()
        {
            if (SelectedItem != null)
            {
                CanEdit = true;
                CanDelete = true;
                ViewDetails(SelectedItem);
            }
            else
            {
                CanEdit = false;
                CanDelete = false;
            }
        }

        private void UnselectItem()
        {
            this.SelectedItem = null;
        }

        private void ViewDetails(Network item)
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (item != null)
                id = item.Id.ToString();
            uriString = string.Format("/derpirc.Pages;component/Views/NetworkDetailView.xaml?id={0}", Uri.EscapeUriString(id));
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void Save()
        {
            SettingsUnitOfWork.Default.Networks = _networksList.ToList();
            SettingsUnitOfWork.Default.Commit(CommitType.Session);
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}