﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;

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

        RelayCommand _navigatedFromCommand;
        public RelayCommand NavigatedFromCommand
        {
            get
            {
                return _navigatedFromCommand ?? (_navigatedFromCommand =
                    new RelayCommand(() => this.OnNavigatedFrom()));
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
                DeleteCommand.RaiseCanExecuteChanged();
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

        private string _hostName;
        public string HostName
        {
            get { return _hostName; }
            set
            {
                if (_hostName == value)
                    return;

                _hostName = value;
                RaisePropertyChanged(() => HostName);
            }
        }

        private string _ports;
        public string Ports
        {
            get { return _ports; }
            set
            {
                if (_ports == value)
                    return;

                _ports = value;
                RaisePropertyChanged(() => Ports);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value)
                    return;

                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsNetworkDetailViewModel class.
        /// </summary>
        public SettingsNetworkDetailViewModel()
        {
            _favoritesList = new ObservableCollection<Favorite>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                DisplayName = "clefnet 1";
                Name = "clefnet";
                HostName = "irc.efnet.org";
                Ports = "6667";
                Password = string.Empty;

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

        private void OnNavigatedFrom()
        {

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
                }
            }
        }

        private void UpdateViewModel(Network model)
        {
            DisplayName = model.DisplayName;
            Name = model.Name;
            if (model.Server != null)
            {
                HostName = model.Server.HostName;
                Ports = model.Server.Ports;
                Password = model.Server.Password;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _favoritesList.Clear();
                foreach (var item in model.Favorites)
                {
                    _favoritesList.Add(item);
                }
                if (_favoritesList.Count > 0)
                    CanClear = true;
            });
        }

        private void Add()
        {
            _favoritesList.Add(new Favorite()
            {
                Name = "#(new)",
                IsAutoConnect = true,
            });
            CanClear = true;
        }

        private void Delete(Favorite item)
        {
            CanDelete = false;
            if (_favoritesList.Contains(item))
                _favoritesList.Remove(item);
            if (_favoritesList.Count == 0)
                CanClear = false;
        }

        private void Clear()
        {
            _favoritesList.Clear();
            CanClear = false;
        }

        private void SelectItem()
        {
            if (SelectedItem != null)
            {
                CanDelete = true;
                CanClear = true;
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