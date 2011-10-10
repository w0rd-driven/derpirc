using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace derpirc.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Commands

        RelayCommand _navigatedToCommand;
        public RelayCommand NavigatedToCommand
        {
            get
            {
                return _navigatedToCommand ?? (_navigatedToCommand =
                    new RelayCommand(() => this.OnNavigatedTo()));
            }
        }

        RelayCommand<FrameworkElement> _layoutRootCommand;
        public RelayCommand<FrameworkElement> LayoutRootCommand
        {
            get
            {
                return _layoutRootCommand ?? (_layoutRootCommand =
                    new RelayCommand<FrameworkElement>(sender => this.LayoutRoot = sender));
            }
        }

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

        private FrameworkElement _layoutRoot;
        public FrameworkElement LayoutRoot
        {
            get { return _layoutRoot; }
            set
            {
                if (_layoutRoot == value)
                    return;

                _layoutRoot = value;
                RaisePropertyChanged(() => LayoutRoot);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsNetworkViewModel class.
        /// </summary>
        public SettingsViewModel()
        {

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
            }
        }

        private void OnNavigatedTo()
        {

        }

        private void OnNavigatedFrom()
        {

        }

        private void Add()
        {

        }

        private void Edit()
        {

        }

        private void Delete()
        {

        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}