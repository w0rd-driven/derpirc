using System.Windows;
using System.Windows.Navigation;
using derpirc.Core;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;

namespace derpirc.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Commands

        RelayCommand<NavigationEventArgs> _navigatedToCommand;
        public RelayCommand<NavigationEventArgs> NavigatedToCommand
        {
            get
            {
                return _navigatedToCommand ?? (_navigatedToCommand =
                    new RelayCommand<NavigationEventArgs>(eventArgs => this.OnNavigatedTo(eventArgs)));
            }
        }

        RelayCommand<NavigationEventArgs> _navigatedFromCommand;
        public RelayCommand<NavigationEventArgs> NavigatedFromCommand
        {
            get
            {
                return _navigatedFromCommand ?? (_navigatedFromCommand =
                    new RelayCommand<NavigationEventArgs>(eventArgs => this.OnNavigatedFrom(eventArgs)));
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

        RelayCommand<PivotItemEventArgs> _pivotItemLoadedCommand;
        public RelayCommand<PivotItemEventArgs> PivotItemLoadedCommand
        {
            get
            {
                return _pivotItemLoadedCommand ?? (_pivotItemLoadedCommand =
                    new RelayCommand<PivotItemEventArgs>(eventArgs => this.PivotItemLoaded(eventArgs)));
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
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    AddCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(() => CanAdd);
                });
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

        RelayCommand<Network> _deleteCommand;
        public RelayCommand<Network> DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand =
                    new RelayCommand<Network>(network => this.Delete(network)));
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
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ClearCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(() => CanClear);
                });
            }
        }

        RelayCommand _clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand =
                    new RelayCommand(() => this.Clear(), () => this.CanClear));
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

        private bool _isAppBarVisible;
        public bool IsAppBarVisible
        {
            get { return _isAppBarVisible; }
            set
            {
                if (_isAppBarVisible == value)
                    return;

                _isAppBarVisible = value;
                RaisePropertyChanged(() => IsAppBarVisible);
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

                this.MessengerInstance.Register<NotificationMessage<bool>>(this, "action", message =>
                {
                    switch (message.Notification)
                    {
                        case "add":
                            CanAdd = message.Content;
                            break;
                        case "clear":
                            CanClear = message.Content;
                            break;
                    }
                });
            }
        }

        private void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            // Resuming shows as a back mode so be careful it was navigation-driven. Could also inspect the Uri but ugh
            if (eventArgs.IsNavigationInitiator && eventArgs.NavigationMode == NavigationMode.Back)
            {
                IsAppBarVisible = true;
                UnselectItem();
            }
            if (!eventArgs.IsNavigationInitiator && eventArgs.NavigationMode == NavigationMode.Back)
                SupervisorFacade.Default.Reconnect(null, true, true);
        }

        private void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
            if (eventArgs.NavigationMode == NavigationMode.Back)
            {
                // HACK: Turn this off now as re-entry will display it for a split second
                IsAppBarVisible = false;
                Save(true);
            }
            if (!eventArgs.IsNavigationInitiator)
                Save(false);
        }

        private void PivotItemLoaded(PivotItemEventArgs eventArgs)
        {
            var pivotControl = eventArgs.Item.Parent as Pivot;
            if (pivotControl != null)
            {
                switch (pivotControl.SelectedIndex)
                {
                    // User
                    case 0:
                        IsAppBarVisible = false;
                        break;
                    // Network
                    case 1:
                        IsAppBarVisible = true;
                        break;
                    // Storage
                    case 2:
                        IsAppBarVisible = false;
                        break;
                }
            }
        }

        private void UnselectItem()
        {
            this.MessengerInstance.Send(new NotificationMessage<Network>(null, "unselect"), "Network");
        }

        private void Save(bool commit)
        {
            this.MessengerInstance.Send(new NotificationMessage<bool>(commit, "save"), "Save");
        }

        private void Add()
        {
            this.MessengerInstance.Send(new NotificationMessage<Network>(null, "add"), "Network");
        }

        private void Delete(Network item)
        {
            this.MessengerInstance.Send(new NotificationMessage<Network>(item, "delete"), "Network");
        }

        private void Clear()
        {
            this.MessengerInstance.Send(new NotificationMessage<Network>(null, "clear"), "Network");
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}