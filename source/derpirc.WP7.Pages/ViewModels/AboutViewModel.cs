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

        private void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            if (!eventArgs.IsNavigationInitiator)
            {
                // Resuming...
                SupervisorFacade.Default.Reconnect(null, true);
            }
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