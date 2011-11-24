using System;
using System.Windows.Navigation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Phone.Tasks;
using derpirc.Core;

namespace derpirc.ViewModels
{
    public class AboutViewModel : ViewModelBase
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

        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                if (_url == value)
                    return;

                _url = value;
                RaisePropertyChanged(() => Url);
            }
        }

        private string _feedbackUrl;
        public string FeedbackUrl
        {
            get { return _feedbackUrl; }
            set
            {
                if (_feedbackUrl == value)
                    return;

                _feedbackUrl = value;
                RaisePropertyChanged(() => FeedbackUrl);
            }
        }

        private string _forumUrl;
        public string ForumUrl
        {
            get { return _forumUrl; }
            set
            {
                if (_forumUrl == value)
                    return;

                _forumUrl = value;
                RaisePropertyChanged(() => ForumUrl);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the AboutViewModel class.
        /// </summary>
        public AboutViewModel()
        {
            Url = "braytonium.com/derpirc";
            FeedbackUrl = "derpirc@braytonium.com";

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
            }
        }

        private void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            if (!eventArgs.IsNavigationInitiator && eventArgs.NavigationMode == NavigationMode.Back)
                SupervisorFacade.Default.Reconnect(null, true, true);
        }

        private void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
            if (eventArgs.NavigationMode == NavigationMode.New)
            {
                // A task is being called...
            }
        }

        private void Website()
        {
            var task = new WebBrowserTask();
            task.Uri = new Uri("http://" + Url);
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
            task.To = FeedbackUrl;
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