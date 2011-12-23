using System;
using System.Threading;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace derpirc.ViewModels
{
    public class SettingsStorageViewModelFactory : ViewModelFactory<SettingsStorageViewModel, SettingsStorageViewModel> { }

    public class SettingsStorageViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

        private Storage _model;
        public Storage Model
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

        private bool _isWipeSettings;
        public bool IsWipeSettings
        {
            get { return _isWipeSettings; }
            set
            {
                if (_isWipeSettings == value)
                    return;

                _isWipeSettings = value;
                RaisePropertyChanged(() => IsWipeSettings);
            }
        }

        private DateTime _settingsCreateDate;
        public DateTime SettingsCreateDate
        {
            get { return _settingsCreateDate; }
            set
            {
                if (_settingsCreateDate == value)
                    return;

                _settingsCreateDate = value;
                RaisePropertyChanged(() => SettingsCreateDate);
            }
        }

        private bool _isWipeData;
        public bool IsWipeData
        {
            get { return _isWipeData; }
            set
            {
                if (_isWipeData == value)
                    return;

                _isWipeData = value;
                RaisePropertyChanged(() => IsWipeData);
            }
        }

        private DateTime _dataCreateDate;
        public DateTime DataCreateDate
        {
            get { return _dataCreateDate; }
            set
            {
                if (_dataCreateDate == value)
                    return;

                _dataCreateDate = value;
                RaisePropertyChanged(() => DataCreateDate);
            }
        }

        private int _showMaxMessages;
        public int ShowMaxMessages
        {
            get { return _showMaxMessages; }
            set
            {
                if (_showMaxMessages == value)
                    return;

                _showMaxMessages = value;
                RaisePropertyChanged(() => ShowMaxMessages);
            }
        }

        private int _storeMaxMessageDays;
        public int StoreMaxMessageDays
        {
            get { return _storeMaxMessageDays; }
            set
            {
                if (_storeMaxMessageDays == value)
                    return;

                _storeMaxMessageDays = value;
                RaisePropertyChanged(() => StoreMaxMessageDays);
            }
        }

        private int _storeMaxMessages;
        public int StoreMaxMessages
        {
            get { return _storeMaxMessages; }
            set
            {
                if (_storeMaxMessages == value)
                    return;

                _storeMaxMessages = value;
                RaisePropertyChanged(() => StoreMaxMessages);
            }
        }

        private bool _isScheduledTaskEnabled;
        public bool IsScheduledTaskEnabled
        {
            get { return _isScheduledTaskEnabled; }
            set
            {
                if (_isScheduledTaskEnabled == value)
                    return;

                _isScheduledTaskEnabled = value;
                RaisePropertyChanged(() => IsScheduledTaskEnabled);
            }
        }

        #endregion

        public SettingsStorageViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                this.IsWipeSettings = true;
                this.SettingsCreateDate = DateTime.Now;
                this.IsWipeData = true;
                this.DataCreateDate = DateTime.Now;
                this.ShowMaxMessages = 50;
                this.StoreMaxMessageDays = 7;
                this.StoreMaxMessages = 1000;
                this.IsScheduledTaskEnabled = true;
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                this.MessengerInstance.Register<NotificationMessage<bool>>(this, "Save", message =>
                {
                    this.Save();
                });

                ThreadPool.QueueUserWorkItem((object userState) =>
                {
                    this.Load();
                });
            }
        }

        private void Load()
        {
            var model = SettingsUnitOfWork.Default.Storage;
            if (model != null)
                this.Model = model;
        }

        private void UpdateViewModel(Storage model)
        {
            this.IsWipeSettings = model.IsWipeSettings;
            this.SettingsCreateDate = model.SettingsCreateDate;
            this.IsWipeData = model.IsWipeData;
            this.DataCreateDate = model.DataCreateDate;
            this.ShowMaxMessages = model.ShowMaxMessages;
            this.StoreMaxMessageDays = model.StoreMaxMessageDays;
            this.StoreMaxMessages = model.StoreMaxMessages;
            this.IsScheduledTaskEnabled = model.IsScheduledTaskEnabled;
        }

        private void Save()
        {
            var newRecord = new Storage();
            newRecord.IsWipeSettings = this.IsWipeSettings;
            newRecord.SettingsCreateDate = this.SettingsCreateDate;
            newRecord.IsWipeData = this.IsWipeData;
            newRecord.DataCreateDate = this.DataCreateDate;
            newRecord.ShowMaxMessages = this.ShowMaxMessages;
            newRecord.StoreMaxMessageDays = this.StoreMaxMessageDays;
            newRecord.StoreMaxMessages = this.StoreMaxMessages;
            newRecord.IsScheduledTaskEnabled = this.IsScheduledTaskEnabled;
            newRecord.IsSyncRequired = true;
            SettingsUnitOfWork.Default.Storage = newRecord;
            newRecord = null;
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}