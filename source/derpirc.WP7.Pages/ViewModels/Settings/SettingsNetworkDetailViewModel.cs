using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace derpirc.ViewModels
{
    public class SettingsNetworkDetailViewModel : ViewModelBase
    {
        #region Commands

        RelayCommand<IDictionary<string, string>> _navigatedToCommand;
        public RelayCommand<IDictionary<string, string>> NavigatedToCommand
        {
            get
            {
                return _navigatedToCommand ?? (_navigatedToCommand =
                    new RelayCommand<IDictionary<string, string>>(item => this.OnNavigatedTo(item)));
            }
        }

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
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand ?? (_addCommand =
                    new RelayCommand(() => this.Add(), () => this.CanAdd));
            }
        }

        RelayCommand _editCommand;
        public RelayCommand EditCommand
        {
            get
            {
                return _editCommand ?? (_editCommand =
                    new RelayCommand(() => this.Edit(SelectedItem)));
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

                var oldValue = _canDelete;
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

        private Network _model;
        public Network Model
        {
            get { return _model; }
            set
            {
                if (value != null)
                    UpdateViewModel(value);
                if (_model == value)
                    return;

                _model = value;
                RaisePropertyChanged(() => Model);
            }
        }

        private ObservableCollection<Favorite> _favoritesList;
        public CollectionViewSource Favorites { get; set; }

        private Favorite _selectedItem;
        public Favorite SelectedItem
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

        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName == value)
                    return;

                _displayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsNetworkDetailViewModel class.
        /// </summary>
        public SettingsNetworkDetailViewModel()
        {
            _favoritesList = new ObservableCollection<Favorite>();

            CanAdd = true;

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                DisplayName = "clefnet 1";
                Name = "clefnet";

                //_favoritesList.Add(new Favorite()
                //{
                //    Name
                //});
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
            }

            Favorites = new CollectionViewSource() { Source = _favoritesList };
        }

        private void OnNavigatedTo(IDictionary<string, string> queryString)
        {
            //TODO: Link Model and VM via Events
            var id = string.Empty;
            queryString.TryGetValue("id", out id);
            var integerId = -1;
            int.TryParse(id, out integerId);
            var model = SettingsUnitOfWork.Default.Networks.FindBy(x => x.Id == integerId).FirstOrDefault();
            if (model != null)
                Model = model;
        }

        private void UpdateViewModel(Network model)
        {
            DisplayName = model.DisplayName;
            Name = model.Name;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _favoritesList.Clear();
                foreach (var item in model.Favorites)
                {
                    _favoritesList.Add(item);
                }
            });
        }

        private void Add()
        {
            _favoritesList.Add(new Favorite()
            {
                Name = "#(new)",
                IsAutoConnect = true,
            });
        }

        private void Edit(Favorite item)
        {

        }

        private void Delete(Favorite item)
        {
            if (_favoritesList.Contains(item))
                _favoritesList.Remove(item);
        }

        private void SelectItem()
        {
            if (SelectedItem != null)
            {
                CanDelete = true;
            }
            else
            {
                CanDelete = false;
            }
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}