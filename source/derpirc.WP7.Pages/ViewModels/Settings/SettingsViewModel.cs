﻿using System.Windows;
using System.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;

namespace derpirc.ViewModels
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

        RelayCommand _unselectItemCommand;
        public RelayCommand UnselectItemCommand
        {
            get
            {
                return _unselectItemCommand ?? (_unselectItemCommand =
                    new RelayCommand(() => this.UnselectItem()));
            }
        }

        RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand =
                    new RelayCommand(() => this.Save()));
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

                _canEdit = value;
                EditCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => CanEdit);
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
                DeleteCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => CanDelete);
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

                this.MessengerInstance.Register<NotificationMessage<bool>>(this, message =>
                {
                    var target = message.Target as string;
                    if (target == "action")
                    {
                        switch (message.Notification)
                        {
                            case "edit":
                                CanEdit = message.Content;
                                break;
                            case "delete":
                                CanDelete = message.Content;
                                break;
                            default:
                                break;
                        }
                    }
                });
            }
        }

        private void OnNavigatedTo()
        {
            IsAppBarVisible = true;
        }

        private void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
            if (eventArgs.NavigationMode == NavigationMode.New)
                IsAppBarVisible = false;
            if (eventArgs.NavigationMode == NavigationMode.Back)
                Save();
        }

        private void PivotItemLoaded(PivotItemEventArgs eventArgs)
        {
            var pivotControl = eventArgs.Item.Parent as Pivot;
            if (pivotControl != null)
            {
                switch (pivotControl.SelectedIndex)
                {
                    case 0:
                        IsAppBarVisible = false;
                        break;
                    case 1:
                        IsAppBarVisible = true;
                        break;
                    case 2:
                        IsAppBarVisible = false;
                        break;
                }
            }
        }

        private void UnselectItem()
        {
            this.MessengerInstance.Send(new NotificationMessage("unselect"), "Network");
        }

        private void Save()
        {
            this.MessengerInstance.Send(new NotificationMessage("save"), "Save");
        }

        private void Add()
        {
            this.MessengerInstance.Send(new NotificationMessage("add"), "Network");
        }

        private void Edit()
        {
            this.MessengerInstance.Send(new NotificationMessage("edit"), "Network");
        }

        private void Delete()
        {
            this.MessengerInstance.Send(new NotificationMessage("delete"), "Network");
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}