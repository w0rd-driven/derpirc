using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace derpirc.ViewModels
{
    public class SettingsUserViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

        private User _model;
        public User Model
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

        private string _nickName;
        public string NickName
        {
            get { return _nickName; }
            set
            {
                if (_nickName == value)
                    return;

                _nickName = value;
                RaisePropertyChanged(() => NickName);
            }
        }

        private string _nickNameAlt;
        public string NickNameAlt
        {
            get { return _nickNameAlt; }
            set
            {
                if (_nickNameAlt == value)
                    return;

                _nickNameAlt = value;
                RaisePropertyChanged(() => NickNameAlt);
            }
        }

        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName == value)
                    return;

                _fullName = value;
                RaisePropertyChanged(() => FullName);
            }
        }

        private string _userName;
        public string Username
        {
            get { return _userName; }
            set
            {
                if (_userName == value)
                    return;

                _userName = value;
                RaisePropertyChanged(() => Username);
            }
        }

        private bool _isInvisible;
        public bool IsInvisible
        {
            get { return _isInvisible; }
            set
            {
                if (_isInvisible == value)
                    return;

                _isInvisible = value;
                RaisePropertyChanged(() => IsInvisible);
            }
        }

        private string _quitMessage;
        public string QuitMessage
        {
            get { return _quitMessage; }
            set
            {
                if (_quitMessage == value)
                    return;

                _quitMessage = value;
                RaisePropertyChanged(() => QuitMessage);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsUserViewModel class.
        /// </summary>
        public SettingsUserViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                NickName = "derpirc";
                NickNameAlt = "durpirc";
                FullName = "derpirc WP7 IRC Client";
                Username = "derpirc";
                IsInvisible = true;
                QuitMessage = "I am a pretty pretty butterfly.";
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                this.MessengerInstance.Register<NotificationMessage>(this, "Save", message =>
                {
                    var target = message.Target as string;
                    this.Save();
                });

                Load();
            }
        }

        private void Load()
        {
            var model = SettingsUnitOfWork.Default.User;
            if (model != null)
                Model = model;
        }

        private void UpdateViewModel(User model)
        {
            NickName = model.NickName;
            NickNameAlt = model.NickNameAlternate;
            FullName = model.FullName;
            Username = model.Username;
            IsInvisible = model.IsInvisible;
            QuitMessage = model.QuitMessage;
        }

        private void Save()
        {
            var newRecord = new User();
            newRecord.NickName = this.NickName;
            newRecord.NickNameAlternate = this.NickNameAlt;
            newRecord.FullName = this.FullName;
            newRecord.Username = this.Username;
            newRecord.IsInvisible = this.IsInvisible;
            newRecord.QuitMessage = this.QuitMessage;
            SettingsUnitOfWork.Default.User = newRecord;
            newRecord = null;
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}