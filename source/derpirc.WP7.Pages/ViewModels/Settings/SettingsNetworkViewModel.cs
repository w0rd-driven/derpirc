using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using derpirc.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace derpirc.ViewModels
{
    public class SettingsNetworkViewModelFactory : ViewModelFactory<SettingsNetworkViewModel, SettingsNetworkViewModel> { }

    public class SettingsNetworkViewModel : ViewModelBase
    {
        #region Commands

        RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand =
                    new RelayCommand(() => this.Add()));
            }
        }

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
                EditCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _editCommand;
        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand ?? (_editCommand =
                    new RelayCommand(() => this.Edit(), () => this.CanEdit));
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
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand =
                    new RelayCommand(() => this.Delete(SelectedItem), () => this.CanDelete));
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

                this.MessengerInstance.Register<NotificationMessage>(this, "SelectedNetwork", message =>
                {
                    var target = message.Target as string;
                    this.UnselectItem();
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
            //var session = 
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

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}