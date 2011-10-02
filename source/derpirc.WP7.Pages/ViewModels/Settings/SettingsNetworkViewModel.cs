using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace derpirc.ViewModels.Settings
{
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

                var oldValue = _canEdit;
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
                    new RelayCommand(() => this.Delete(), () => this.CanDelete));
            }
        }

        #endregion

        #region Properties

        private Session _model;
        public Session Model
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

                var oldValue = _selectedItem;
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
                SelectItem();
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsNetworkViewModel class.
        /// </summary>
        public SettingsNetworkViewModel()
        {

            _networksList = new ObservableCollection<Network>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                Load();
            }

            Networks = new CollectionViewSource() { Source = _networksList };
        }

        private void Load()
        {
            var model = SettingsUnitOfWork.Default.Sessions.FindBy(x => x.Name == "default").FirstOrDefault();
            if (model != null)
                Model = model;
        }

        private void UpdateViewModel(Session model)
        {
            _networksList.Clear();
            foreach (var item in model.Networks)
            {
                _networksList.Add(item);
            }
        }

        void Add()
        {

        }

        void Edit()
        {

        }

        void Delete()
        {

        }

        void SelectItem()
        {
            if (SelectedItem != null)
            {
                CanEdit = true;
                CanDelete = true;
            }
            else
            {
                CanEdit = false;
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