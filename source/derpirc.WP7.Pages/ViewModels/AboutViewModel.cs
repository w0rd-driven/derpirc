using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;

namespace derpirc.ViewModels
{
    public class AboutViewModel : ViewModelBase
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

        RelayCommand _navigatedFromCommand;
        public RelayCommand NavigatedFromCommand
        {
            get
            {
                return _navigatedToCommand ?? (_navigatedToCommand =
                    new RelayCommand(() => this.OnNavigatedFrom()));
            }
        }

        RelayCommand _websiteCommand;
        public RelayCommand WebsiteCommand
        {
            get
            {
                return _websiteCommand ?? (_websiteCommand =
                    new RelayCommand(() => this.Website()));
            }
        }

        RelayCommand _reviewCommand;
        public RelayCommand ReviewCommand
        {
            get
            {
                return _reviewCommand ?? (_reviewCommand =
                    new RelayCommand(() => this.Review()));
            }
        }

        RelayCommand _feedbackCommand;
        public RelayCommand FeedbackCommand
        {
            get
            {
                return _feedbackCommand ?? (_feedbackCommand =
                    new RelayCommand(() => this.Feedback()));
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

        #endregion

        /// <summary>
        /// Initializes a new instance of the AboutViewModel class.
        /// </summary>
        public AboutViewModel()
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

        private void Website()
        {
            var task = new WebBrowserTask();
            // TODO: Get from strings
            task.Uri = new Uri("http://braytonium.com");
            task.Show();
        }

        private void Review()
        {
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        private void Feedback()
        {
            var task = new EmailComposeTask();
            // TODO: Get from strings
            task.To = "durpirc@braytonium.com";
            task.Body = "";
            task.Subject = "Feedback: ";
            task.Show(); 
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}