using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models.Settings;
using GalaSoft.MvvmLight;

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

        private Nullable<bool> _isInvisible;
        public Nullable<bool> IsInvisible
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
                Load();
            }
        }

        private void Load()
        {
            var model = SettingsUnitOfWork.Default.User.FindBy(x => x.Name == "default").FirstOrDefault();
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

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}