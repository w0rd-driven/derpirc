using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;

namespace derpirc.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Commands

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
                    new RelayCommand<PivotItemEventArgs>(eventArgs => PivotItemLoaded(eventArgs)));
            }
        }

        RelayCommand _selectChannelCommand;
        public RelayCommand SelectChannelCommand
        {
            get
            {
                return _selectChannelCommand ?? (_selectChannelCommand =
                    new RelayCommand(() => this.SelectChannel()));
            }
        }

        RelayCommand _selectMentionCommand;
        public RelayCommand SelectMentionCommand
        {
            get
            {
                return _selectMentionCommand ?? (_selectMentionCommand =
                    new RelayCommand(() => this.SelectMention()));
            }
        }

        RelayCommand _selectMessageCommand;
        public RelayCommand SelectMessageCommand
        {
            get
            {
                return _selectMessageCommand ?? (_selectMessageCommand =
                    new RelayCommand(() => this.SelectMessage()));
            }
        }

        #endregion

        #region Properties

        private readonly INavigationService navigationService;
        public INavigationService NavigationService
        {
            get
            {
                return this.navigationService;
            }
        }

        private FrameworkElement _layoutRoot;
        public FrameworkElement LayoutRoot
        {
            get { return _layoutRoot; }
            set
            {
                if (_layoutRoot == value)
                    return;

                var oldValue = _layoutRoot;
                _layoutRoot = value;
                RaisePropertyChanged(() => LayoutRoot);
            }
        }

        private ChannelSummaryViewModel _selectedChannel;
        public ChannelSummaryViewModel SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                if (_selectedChannel == value)
                    return;

                var oldValue = _selectedChannel;
                _selectedChannel = value;
                RaisePropertyChanged(() => SelectedChannel);
            }
        }

        private ObservableCollection<ChannelSummaryViewModel> _channelsList;
        private CollectionViewSource _channels;
        public CollectionViewSource Channels
        {
            get { return _channels; }
            set
            {
                if (_channels == value)
                    return;

                var oldValue = _channels;
                _channels = value;
                RaisePropertyChanged(() => Channels);
            }
        }

        private MentionSummaryViewModel _selectedMention;
        public MentionSummaryViewModel SelectedMention
        {
            get { return _selectedMention; }
            set
            {
                if (_selectedMention == value)
                    return;

                var oldValue = _selectedMention;
                _selectedMention = value;
                RaisePropertyChanged(() => SelectedMention);
            }
        }

        private ObservableCollection<MentionSummaryViewModel> _mentionsList;
        private CollectionViewSource _mentions;
        public CollectionViewSource Mentions
        {
            get { return _mentions; }
            set
            {
                if (_mentions == value)
                    return;

                var oldValue = _mentions;
                _mentions = value;
                RaisePropertyChanged(() => Mentions);
            }
        }

        private MessageSummaryViewModel _selectedMessage;
        public MessageSummaryViewModel SelectedMessage
        {
            get { return _selectedMessage; }
            set
            {
                if (_selectedMessage == value)
                    return;

                var oldValue = _selectedMessage;
                _selectedMessage = value;
                RaisePropertyChanged(() => SelectedMessage);
            }
        }

        private ObservableCollection<MessageSummaryViewModel> _messagesList;
        private CollectionViewSource _messages;
        public CollectionViewSource Messages
        {
            get { return _messages; }
            set
            {
                if (_messages == value)
                    return;

                var oldValue = _messages;
                _messages = value;
                RaisePropertyChanged(() => Messages);
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        #endregion

        private BackgroundWorker _worker;
        private Data.DataUnitOfWork _unitOfWork;
        private Core.SessionSupervisor _sessionSupervisor;

        private DateTime _lastRefreshChannels;
        private DateTime _lastRefreshMentions;
        private DateTime _lastRefreshMessages;

        public MainViewModel()
        {
            // TODO: For demo purposes, place initialization of everything here and in design mode. Otherwise, use state to determine what to construct.
            // HACK: Execution order: 1
            navigationService = new ApplicationFrameNavigationService(((App)Application.Current).RootFrame);

            _channelsList = new ObservableCollection<ChannelSummaryViewModel>();
            Channels = new CollectionViewSource() { Source = _channelsList };
            _mentionsList = new ObservableCollection<MentionSummaryViewModel>();
            Mentions = new CollectionViewSource() { Source = _mentionsList };
            _messagesList = new ObservableCollection<MessageSummaryViewModel>();
            Messages = new CollectionViewSource() { Source = _messagesList };

            _worker = new BackgroundWorker();
            _worker.DoWork += new DoWorkEventHandler(DeferStartupWork);

            _unitOfWork = new Data.DataUnitOfWork();
            _sessionSupervisor = new Core.SessionSupervisor(_unitOfWork);
            _sessionSupervisor.ChannelJoined += new EventHandler<Core.ChannelStatusEventArgs>(_sessionSupervisor_ChannelJoined);
            _sessionSupervisor.ChannelLeft += new EventHandler<Core.ChannelStatusEventArgs>(_sessionSupervisor_ChannelLeft);
            _sessionSupervisor.MessageReceived += new EventHandler<Core.ChannelItemEventArgs>(_sessionSupervisor_MessageReceived);
            _sessionSupervisor.MentionReceived += new EventHandler<Core.MentionItemEventArgs>(_sessionSupervisor_MentionReceived);
        }

        void _sessionSupervisor_ChannelJoined(object sender, Core.ChannelStatusEventArgs e)
        {
            var foundChannel = _channelsList.Where(x => x.ChannelName == e.Channel.Name && x.Model.ServerId == e.Channel.ServerId).FirstOrDefault();
            if (foundChannel == null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var channelSummary = new ChannelSummaryViewModel(e.Channel);
                    _channelsList.Add(channelSummary);
                    Channels.View.Refresh();
                });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundChannel.Model = e.Channel;
                    Channels.View.Refresh();
                });
            }
        }

        void _sessionSupervisor_ChannelLeft(object sender, Core.ChannelStatusEventArgs e)
        {
        }

        void _sessionSupervisor_MessageReceived(object sender, Core.ChannelItemEventArgs e)
        {
            var foundChannel = _channelsList.Where(x => x.ChannelName == e.Channel.Name && x.Model.ServerId == e.Channel.ServerId).FirstOrDefault();
            if (foundChannel == null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var channelSummary = new ChannelSummaryViewModel(e.Channel);
                    _channelsList.Add(channelSummary);
                    Channels.View.Refresh();
                });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundChannel.Model = e.Channel;
                    Channels.View.Refresh();
                });
            }
        }

        void _sessionSupervisor_MentionReceived(object sender, Core.MentionItemEventArgs e)
        {
            var foundChannel = _mentionsList.Where(x => x.NickName == e.Mention.Name && x.Model.ServerId == e.Mention.ServerId).FirstOrDefault();
            if (foundChannel == null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var mentionSummary = new MentionSummaryViewModel(e.Mention);
                    _mentionsList.Add(mentionSummary);
                    Mentions.View.Refresh();
                });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundChannel.Model = e.Mention;
                    Mentions.View.Refresh();
                });
            }
        }

        internal void DeferStartup(Action completed)
        {
            _worker.RunWorkerAsync(completed);
        }

        private void DeferStartupWork(object sender, DoWorkEventArgs e)
        {
            Action completed = e.Argument as Action;
            //lock (threadLock)
            //{
            //    ApplicationState.AppLaunchInitialization();

            //    this.SetDefaultCategoryAndUnits();
            //    ApplicationState.Favorites =
            //        FavoriteCollection.LoadFromFile() ?? new FavoriteCollection();
            //}

            if (completed != null)
            {
                completed();
            }
        }

        void RootLoaded(FrameworkElement sender)
        {
            // HACK: Execution order: 2
            LayoutRoot = sender;
        }

        void PivotItemLoaded(PivotItemEventArgs eventArgs)
        {
            // HACK: Execution order: 3
            // You can use PivotItemLoaded or SelectedItem/Index binding. This gets called every time the PivotItem shows so you need to track an IsVMLoaded
            if (eventArgs.Item.Header.ToString() == "Channels")
            {
                //var channel = new ChannelSummaryViewModel();
                //_channelsList.Add(channel);
                //Channels.View.Refresh();
                _lastRefreshChannels = DateTime.Now;
                return;
            }
            if (eventArgs.Item.Header.ToString() == "Mentions")
            {
                //var mention = new MentionSummaryViewModel();
                //_mentionsList.Add(mention);
                //Mentions.View.Refresh();
                _lastRefreshMentions = DateTime.Now;
                return;
            }
            if (eventArgs.Item.Header.ToString() == "Messages")
            {
                //var message = new MessageSummaryViewModel();
                //_messagesList.Add(message);
                //Messages.View.Refresh();
                _lastRefreshMessages = DateTime.Now;
                return;
            }
         }

        private void SelectChannel()
        {
            var id = string.Empty;
            var name = string.Empty;
            var uriString = string.Empty;

            if (SelectedChannel != null)
            {
                id = SelectedChannel.Model.Id.ToString();
                name = SelectedChannel.Model.Name.Substring(1);
            }
            else
            {
                id = "1";
                name = "wp7";
            }
            uriString = string.Format("/Views/ChannelDetailView.xaml?id={0}&name={1}", 
                Uri.EscapeUriString(id), Uri.EscapeUriString(name));
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void SelectMention()
        {
            var id = string.Empty;
            var name = string.Empty;
            var uriString = string.Empty;

            if (SelectedMention != null)
            {
                id = SelectedMention.Model.Id.ToString();
                name = SelectedMention.Model.Name.Substring(1);
            }
            else
            {
                id = "1";
                name = "w0rd-driven";
            }
            uriString = string.Format("/Views/MentionDetailView.xaml?id={0}&name={1}", 
                Uri.EscapeUriString(id), Uri.EscapeUriString(name));
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void SelectMessage()
        {
            var id = string.Empty;
            var name = string.Empty;
            var uriString = string.Empty;

            if (SelectedMessage != null)
            {
                id = SelectedMessage.Model.Id.ToString();
                name = SelectedMessage.Model.Name.Substring(1);
            }
            else
            {
                id = "1";
                name = "w0rd-driven";
            }
            uriString = string.Format("/Views/MessageDetailView.xaml?id={0}&name={1}",
                Uri.EscapeUriString(id), Uri.EscapeUriString(name));
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void OnNavigatedTo()
        {
            //Application launch
            //Application Activation when the app was tombstoned.
            //  Retrieve the application state from either the PhoneApplicationService or PageApplicationService objects.
            //Application Activation when the app was not tombstoned.
            //  This case is essentially a No Op.
            //Application Activation when the application state was not fully saved before exiting. 
            //  This can occur if the user presses the Start button before you application completes its first time initialization.
            //Special cases
            //  The application is tombstoned before it fully initializes on Launch
            //  The application is activated but the application was not tombstoned

            //if (ApplicationState.ApplicationStartup == AppOpenState.Launching)
            //{
            //    // Initial application startup.
            //    this.AllowNavigation = false;
            //    this.SetDefaults();
            //    // Initialization of the app deferred until the page has rendered. See
            //    // the MainPage_LayoutUpdated handler.
            //    return;
            //}
            //if (ApplicationState.ApplicationStartup == AppOpenState.Activated &&
            //     !isPageActivated)
            //{
            //    // We are returning to the application, but we were not tombstoned.
            //    ApplicationState.ApplicationStartup = AppOpenState.None;
            //    return;
            //}
            //if (ApplicationState.RetrieveAppObjects(false))
            //{
            //    this.RefreshStateFromAppState();
            //    this.AllowNavigation = true;
            //}
            //else
            //{
            //    this.AllowNavigation = false;
            //    // Slight possibility that we did not complete 1st time init because of 
            //    // of a app deactivate immediately after start. Protect against this and
            //    // perform application 1st time startup sequence.
            //    this.SetDefaults();
            //    this.DeferStartup(notifyOfLoadCompleted);
            //}
            //ApplicationState.ApplicationStartup = AppOpenState.None;
        }

        void OnNavigatedFrom()
        {
            //Save required state in either the Phone Application service or Page Application service depending on the structure of your application.
            //Clear the flag indicating that the page constructor has been called.
        }

        public void LoadData()
        {
            this.IsDataLoaded = true;
        }

        public void Send(ChannelSummary channel, string message)
        {
            _sessionSupervisor.SendMessage(channel, message);
        }
    }
}
