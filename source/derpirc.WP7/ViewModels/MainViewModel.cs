using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Data.Models;
using derpirc.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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

        private bool _canViewSettings;
        public bool CanViewSettings
        {
            get { return _canViewSettings; }
            set
            {
                if (_canViewSettings == value)
                    return;

                var oldValue = _canViewSettings;
                _canViewSettings = value;
                RaisePropertyChanged(() => CanViewSettings);
                ViewSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _viewSettingsCommand;
        public RelayCommand ViewSettingsCommand
        {
            get
            {
                return _viewSettingsCommand ?? (_viewSettingsCommand =
                    new RelayCommand(() => this.ViewSettings()));
            }
        }

        private bool _canViewConnections;
        public bool CanViewConnections
        {
            get { return _canViewConnections; }
            set
            {
                if (_canViewConnections == value)
                    return;

                var oldValue = _canViewConnections;
                _canViewConnections = value;
                RaisePropertyChanged(() => CanViewConnections);
                ViewConnectionsCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _viewConnectionsCommand;
        public RelayCommand ViewConnectionsCommand
        {
            get
            {
                return _viewConnectionsCommand ?? (_viewConnectionsCommand =
                    new RelayCommand(() => this.ViewConnections()));
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

        private ChannelViewModel _selectedChannel;
        public ChannelViewModel SelectedChannel
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

        private ObservableCollection<ChannelViewModel> _channelsList;
        public CollectionViewSource Channels  { get; set; }

        private MentionViewModel _selectedMention;
        public MentionViewModel SelectedMention
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

        private ObservableCollection<MentionViewModel> _mentionsList;
        public CollectionViewSource Mentions { get; set; }

        private MessageViewModel _selectedMessage;
        public MessageViewModel SelectedMessage
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

        private ObservableCollection<MessageViewModel> _messagesList;
        public CollectionViewSource Messages { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        private bool _progressIndeterminate;
        public bool ProgressIndeterminate
        {
            get { return _progressIndeterminate; }
            set
            {
                if (_progressIndeterminate == value)
                    return;

                var oldValue = _progressIndeterminate;
                _progressIndeterminate = value;
                RaisePropertyChanged(() => ProgressIndeterminate);
            }
        }

        private string _progressText;
        public string ProgressText
        {
            get { return _progressText; }
            set
            {
                if (_progressText == value)
                    return;

                var oldValue = _progressText;
                _progressText = value;
                RaisePropertyChanged(() => ProgressText);
            }
        }

        #endregion

        private object _threadLock = new object();
        private DataUnitOfWork _unitOfWork;
        private BackgroundWorker _worker;
        private Core.SupervisorFacade _supervisor;

        private DateTime _lastRefreshChannels;
        private DateTime _lastRefreshMentions;
        private DateTime _lastRefreshMessages;

        public MainViewModel()
        {
            _channelsList = new ObservableCollection<ChannelViewModel>();
            _mentionsList = new ObservableCollection<MentionViewModel>();
            _messagesList = new ObservableCollection<MessageViewModel>();

            CanViewSettings = true;

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real": Connect to service, etc...

                // TODO: For demo purposes, place initialization of everything here and in design mode. Otherwise, use state to determine what to construct.
                // HACK: Execution order: 1
                navigationService = new ApplicationFrameNavigationService(((App)Application.Current).RootFrame);

                _worker = new BackgroundWorker();
                _worker.DoWork += new DoWorkEventHandler(DeferStartupWork);

                DeferStartup(null);
            }

            Channels = new CollectionViewSource() { Source = _channelsList };
            Mentions = new CollectionViewSource() { Source = _mentionsList };
            Messages = new CollectionViewSource() { Source = _messagesList };
        }

        internal void DeferStartup(Action completed)
        {
            _worker.RunWorkerAsync(completed);
        }

        private void DeferStartupWork(object sender, DoWorkEventArgs e)
        {
            Action completed = e.Argument as Action;
            lock (_threadLock)
            {
                //    ApplicationState.AppLaunchInitialization();

                //    this.SetDefaultCategoryAndUnits();
                //    ApplicationState.Favorites =
                //        FavoriteCollection.LoadFromFile() ?? new FavoriteCollection();

                _supervisor = Core.SupervisorFacade.Default;
                _supervisor.ChannelJoined += new EventHandler<Core.ChannelStatusEventArgs>(_sessionSupervisor_ChannelJoined);
                _supervisor.ChannelLeft += new EventHandler<Core.ChannelStatusEventArgs>(_sessionSupervisor_ChannelLeft);
                _supervisor.ChannelItemReceived += new EventHandler<Core.MessageItemEventArgs>(_sessionSupervisor_ChannelItemReceived);
                _supervisor.MentionItemReceived += new EventHandler<Core.MessageItemEventArgs>(_sessionSupervisor_MentionItemReceived);
                _supervisor.MessageItemReceived += new EventHandler<Core.MessageItemEventArgs>(_sessionSupervisor_MessageItemReceived);

                this.MessengerInstance.Register<GenericMessage<ChannelItem>>(this, message =>
                {
                    var target = message.Target as string;
                    if (target == "out")
                        Send(message.Content);
                });
                this.MessengerInstance.Register<GenericMessage<MentionItem>>(this, message =>
                {
                    var target = message.Target as string;
                    if (target == "out")
                        Send(message.Content);
                });
                this.MessengerInstance.Register<GenericMessage<MessageItem>>(this, message =>
                {
                    var target = message.Target as string;
                    if (target == "out")
                        Send(message.Content);
                });

                LoadInitialView();
            }

            if (completed != null)
            {
                completed();
            }
        }

        #region Events

        void _sessionSupervisor_ChannelJoined(object sender, Core.ChannelStatusEventArgs e)
        {
            var foundItem = _channelsList.Where(x => x.Model.Id == e.SummaryId).FirstOrDefault();
            if (foundItem == null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var channelSummary = new ChannelViewModel();
                    channelSummary.LoadById(e.SummaryId);
                    _channelsList.Add(channelSummary);
                });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundItem.LoadById(e.SummaryId);
                });
            }
        }

        void _sessionSupervisor_ChannelLeft(object sender, Core.ChannelStatusEventArgs e)
        {
        }

        void _sessionSupervisor_ChannelItemReceived(object sender, Core.MessageItemEventArgs e)
        {
            var foundItem = _channelsList.Where(x => x.Model.Id == e.SummaryId).FirstOrDefault();
            if (foundItem == null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var channelSummary = new ChannelViewModel();
                    channelSummary.LoadById(e.SummaryId);
                    _channelsList.Add(channelSummary);
                });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundItem.LoadById(e.SummaryId);
                    var newMessage = foundItem.Model.Messages.FirstOrDefault(x => x.Id == e.MessageId);
                    if (newMessage != null)
                        this.MessengerInstance.Send(new GenericMessage<ChannelItem>(this, "in", newMessage));
                });
            }
        }

        void _sessionSupervisor_MentionItemReceived(object sender, Core.MessageItemEventArgs e)
        {
            var foundItem = _mentionsList.Where(x => x.Model.Id == e.SummaryId).FirstOrDefault();
            if (foundItem == null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var mentionSummary = new MentionViewModel();
                    mentionSummary.LoadById(e.SummaryId);
                    _mentionsList.Add(mentionSummary);
                });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundItem.LoadById(e.SummaryId);
                    var newMessage = foundItem.Model.Messages.FirstOrDefault(x => x.Id == e.MessageId);
                    if (newMessage != null)
                        this.MessengerInstance.Send(new GenericMessage<MentionItem>(this, "in", newMessage));
                });
            }
        }

        void _sessionSupervisor_MessageItemReceived(object sender, Core.MessageItemEventArgs e)
        {
            var foundItem = _messagesList.Where(x => x.Model.Id == e.SummaryId).FirstOrDefault();
            if (foundItem == null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var messageSummary = new MessageViewModel();
                    messageSummary.LoadById(e.SummaryId);
                    _messagesList.Add(messageSummary);
                });
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundItem.LoadById(e.SummaryId);
                    var newMessage = foundItem.Model.Messages.FirstOrDefault(x => x.Id == e.MessageId);
                    if (newMessage != null)
                        this.MessengerInstance.Send(new GenericMessage<MessageItem>(this, "in", newMessage));
                });
            }
        }

        #endregion

        private void LoadInitialView()
        {
            //_unitOfWork = new DataUnitOfWork();
            _unitOfWork = DataUnitOfWork.Default;
            // HACK: Test First Init
            _unitOfWork.WipeDatabase();
            _unitOfWork.InitializeDatabase(false);
            var isExisting = _unitOfWork.DatabaseExists;
            if (isExisting)
            {
                var channels = _unitOfWork.Channels.FindAll().ToList();
                var mentions = _unitOfWork.Mentions.FindAll().ToList();
                var messages = _unitOfWork.Messages.FindAll().ToList();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ProgressIndeterminate = true;
                    ProgressText = "Loading from database";

                    foreach (var item in channels)
                    {
                        var itemSummary = new ChannelViewModel(item);
                        _channelsList.Add(itemSummary);
                    }
                    _lastRefreshChannels = DateTime.Now;

                    foreach (var item in mentions)
                    {
                        var itemSummary = new MentionViewModel(item);
                        _mentionsList.Add(itemSummary);
                    }
                    _lastRefreshMentions = DateTime.Now;

                    foreach (var item in messages)
                    {
                        var itemSummary = new MessageViewModel(item);
                        _messagesList.Add(itemSummary);
                    }
                    _lastRefreshMessages = DateTime.Now;

                    ProgressIndeterminate = false;
                    ProgressText = string.Empty;
                });
            }
        }

        private void RootLoaded(FrameworkElement sender)
        {
            // HACK: Execution order: 2
            LayoutRoot = sender;
        }

        private void PivotItemLoaded(PivotItemEventArgs eventArgs)
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

        private void OnNavigatedFrom()
        {
            //Save required state in either the Phone Application service or Page Application service depending on the structure of your application.
            //Clear the flag indicating that the page constructor has been called.
        }

        public void LoadData()
        {
            this.IsDataLoaded = true;
        }

        private void ViewSettings()
        {
            var uriString = "/derpirc.Pages;component/Views/SettingsView.xaml";
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void ViewConnections()
        {
            var uriString = "/derpirc.Pages;component/Views/ConnectionView.xaml";
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void SelectChannel()
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (SelectedChannel != null)
            {
                id = SelectedChannel.Model.Id.ToString();
            }
            else
            {
                id = "1";
            }
            uriString = string.Format("/derpirc.Pages;component/Views/ChannelDetailView.xaml?id={0}", Uri.EscapeUriString(id));
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void SelectMention()
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (SelectedMention != null)
            {
                id = SelectedMention.Model.Id.ToString();
            }
            else
            {
                id = "1";
            }
            uriString = string.Format("/derpirc.Pages;component/Views/MentionDetailView.xaml?id={0}", Uri.EscapeUriString(id));
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void SelectMessage()
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (SelectedMessage != null)
            {
                id = SelectedMessage.Model.Id.ToString();
            }
            else
            {
                id = "1";
            }
            uriString = string.Format("/derpirc.Pages;component/Views/MessageDetailView.xaml?id={0}", Uri.EscapeUriString(id));
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        public void Send(ChannelItem message)
        {
            _supervisor.SendMessage(message);
        }

        public void Send(MentionItem message)
        {
            _supervisor.SendMessage(message);
        }

        public void Send(MessageItem message)
        {
            _supervisor.SendMessage(message);
        }
    }
}
