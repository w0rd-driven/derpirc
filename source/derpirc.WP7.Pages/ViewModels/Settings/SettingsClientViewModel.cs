using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace derpirc.ViewModels
{
    public class SettingsClientViewModelFactory : ViewModelFactory<SettingsClientViewModel, SettingsClientViewModel> { }

    public class SettingsClientViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

        private Client _model;
        public Client Model
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

        private bool _reconnectOnDisconnect;
        public bool IsReconnectOnDisconnect
        {
            get { return _reconnectOnDisconnect; }
            set
            {
                if (_reconnectOnDisconnect == value)
                    return;

                _reconnectOnDisconnect = value;
                RaisePropertyChanged(() => IsReconnectOnDisconnect);
            }
        }

        private bool _rejoinOnKick;
        public bool IsRejoinOnKick
        {
            get { return _rejoinOnKick; }
            set
            {
                if (_rejoinOnKick == value)
                    return;

                _rejoinOnKick = value;
                RaisePropertyChanged(() => IsRejoinOnKick);
            }
        }

        private bool _joinOnInvite;
        public bool IsJoinOnInvite
        {
            get { return _joinOnInvite; }
            set
            {
                if (_joinOnInvite == value)
                    return;

                _joinOnInvite = value;
                RaisePropertyChanged(() => IsJoinOnInvite);
            }
        }

        private int _disconnectRetries;
        public int DisconnectRetries
        {
            get { return _disconnectRetries; }
            set
            {
                if (_disconnectRetries == value)
                    return;

                _disconnectRetries = value;
                RaisePropertyChanged(() => DisconnectRetries);
            }
        }

        private int _disconnectRetryWait;
        public int DisconnectRetryWait
        {
            get { return _disconnectRetryWait; }
            set
            {
                if (_disconnectRetryWait == value)
                    return;

                _disconnectRetryWait = value;
                RaisePropertyChanged(() => DisconnectRetryWait);
            }
        }

        #endregion

        public SettingsClientViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                this.IsReconnectOnDisconnect = true;
                this.IsRejoinOnKick = true;
                this.IsJoinOnInvite = true;
                this.DisconnectRetries = 5;
                this.DisconnectRetryWait = 5;
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                this.MessengerInstance.Register<NotificationMessage>(this, "Save", message =>
                {
                    this.Save();
                });

                this.Load();
            }
        }

        private void Load()
        {
            var model = SettingsUnitOfWork.Default.Client;
            if (model != null)
                this.Model = model;
        }

        private void UpdateViewModel(Client model)
        {
            this.IsReconnectOnDisconnect = model.IsReconnectOnDisconnect;
            this.IsRejoinOnKick = model.IsRejoinOnKick;
            this.IsJoinOnInvite = model.IsJoinOnInvite;
            this.DisconnectRetries = model.DisconnectRetryWait;
            this.DisconnectRetryWait = model.DisconnectRetryWait;
        }

        private void Save()
        {
            var newRecord = new Client();
            newRecord.IsReconnectOnDisconnect = this.IsReconnectOnDisconnect;
            newRecord.IsRejoinOnKick = this.IsRejoinOnKick;
            newRecord.IsJoinOnInvite = this.IsJoinOnInvite;
            newRecord.DisconnectRetries = this.DisconnectRetries;
            newRecord.DisconnectRetryWait = this.DisconnectRetryWait;
            SettingsUnitOfWork.Default.Client = newRecord;
            newRecord = null;
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}