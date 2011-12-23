using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using derpirc.Core;
using derpirc.Data;
using derpirc.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Phone.Controls;

namespace derpirc.ViewModels
{
    public class MainViewModelFactory : ViewModelFactory<MainViewModel, MainViewModel> { }

    public class MainViewModel : ViewModelBase
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

        RelayCommand<FrameworkElement> _layoutRootCommand;
        public RelayCommand<FrameworkElement> LayoutRootCommand
        {
            get
            {
                return _layoutRootCommand ?? (_layoutRootCommand =
                    new RelayCommand<FrameworkElement>(sender => this.RootLoaded(sender)));
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

        RelayCommand _unselectItemCommand;
        public RelayCommand UnselectItemCommand
        {
            get
            {
                return _unselectItemCommand ?? (_unselectItemCommand =
                    new RelayCommand(() => this.UnselectItem()));
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

                _canViewSettings = value;
                ViewSettingsCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => CanViewSettings);
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

                _canViewConnections = value;
                ViewConnectionsCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => CanViewConnections);
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

        private bool _canViewAbout;
        public bool CanViewAbout
        {
            get { return _canViewAbout; }
            set
            {
                if (_canViewAbout == value)
                    return;

                _canViewAbout = value;
                ViewAboutCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => CanViewAbout);
            }
        }

        RelayCommand _viewAboutCommand;
        public RelayCommand ViewAboutCommand
        {
            get
            {
                return _viewAboutCommand ?? (_viewAboutCommand =
                    new RelayCommand(() => this.ViewAbout()));
            }
        }

        #endregion

        #region Properties

        private readonly INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get { return this._navigationService; }
        }

        private FrameworkElement _layoutRoot;
        public FrameworkElement LayoutRoot
        {
            get { return _layoutRoot; }
            set
            {
                if (_layoutRoot == value)
                    return;

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

                _selectedChannel = value;
                RaisePropertyChanged(() => SelectedChannel);
                if (SelectedChannel != null)
                    SelectChannel();
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

                _selectedMention = value;
                RaisePropertyChanged(() => SelectedMention);
                if (SelectedMention != null)
                    SelectMention();
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

                _selectedMessage = value;
                RaisePropertyChanged(() => SelectedMessage);
                if (SelectedMessage != null)
                    SelectMessage();
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

                _progressText = value;
                RaisePropertyChanged(() => ProgressText);
            }
        }

        #endregion

        private object _threadLock = new object();
        private BackgroundWorker _worker;
        private SupervisorFacade _supervisor;

        private DateTime _lastRefreshChannels;
        private DateTime _lastRefreshMentions;
        private DateTime _lastRefreshMessages;

        public MainViewModel() : this(null) { }

        public MainViewModel(INavigationService navigationService)
        {
            _channelsList = new ObservableCollection<ChannelViewModel>();
            _mentionsList = new ObservableCollection<MentionViewModel>();
            _messagesList = new ObservableCollection<MessageViewModel>();
            Channels = new CollectionViewSource() { Source = _channelsList };
            Mentions = new CollectionViewSource() { Source = _mentionsList };
            Messages = new CollectionViewSource() { Source = _messagesList };
            _lastRefreshChannels = DateTime.MinValue;
            _lastRefreshMentions = DateTime.MinValue;
            _lastRefreshMessages = DateTime.MinValue;
            CanViewConnections = true;
            CanViewSettings = true;
            CanViewAbout = true;

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                _channelsList.Add(new ChannelViewModel());
                _channelsList.Add(new ChannelViewModel());
                _mentionsList.Add(new MentionViewModel());
                _mentionsList.Add(new MentionViewModel());
                _messagesList.Add(new MessageViewModel());
                _messagesList.Add(new MessageViewModel());
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                // HACK: Execution order: 1
                _navigationService = navigationService;

                _worker = new BackgroundWorker();
                _worker.DoWork += new DoWorkEventHandler(DeferStartupWork);
            }
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
                if (_supervisor == null)
                {
                    _supervisor = SupervisorFacade.Default;
                    _supervisor.StateChanged += this._supervisor_StateChanged;
                    _supervisor.MessageRemoved += this._supervisor_MessageRemoved;
                    _supervisor.ChannelJoined += this._supervisor_ChannelJoined;
                    _supervisor.ChannelLeft += this._supervisor_ChannelLeft;
                    _supervisor.ChannelItemReceived += this._supervisor_ChannelItemReceived;
                    _supervisor.MentionItemReceived += this._supervisor_MentionItemReceived;
                    _supervisor.MessageItemReceived += this._supervisor_MessageItemReceived;
                }
            }

            if (completed != null)
            {
                completed();
            }
        }

        #region Events

        private void _supervisor_StateChanged(object sender, ClientStatusEventArgs e)
        {
            if (e.Info.State == ClientState.Processed)
            {
                var foundChannels = _channelsList.Where(x => x.NetworkName.Equals(e.Info.NetworkName,
                    StringComparison.OrdinalIgnoreCase));
                foreach (var item in foundChannels)
                    item.IsConnected = true;

                var foundMentions = _mentionsList.Where(x => x.NetworkName.Equals(e.Info.NetworkName,
                    StringComparison.OrdinalIgnoreCase));
                foreach (var item in foundMentions)
                    item.IsConnected = true;

                var foundMessages = _messagesList.Where(x => x.NetworkName.Equals(e.Info.NetworkName,
                    StringComparison.OrdinalIgnoreCase));
                foreach (var item in foundMessages)
                    item.IsConnected = true;
            }
            else
            {
                var foundChannels = _channelsList.Where(x => x.NetworkName.Equals(e.Info.NetworkName,
                    StringComparison.OrdinalIgnoreCase));
                foreach (var item in foundChannels)
                    item.IsConnected = false;

                var foundMentions = _mentionsList.Where(x => x.NetworkName.Equals(e.Info.NetworkName,
                    StringComparison.OrdinalIgnoreCase));
                foreach (var item in foundMentions)
                    item.IsConnected = false;

                var foundMessages = _messagesList.Where(x => x.NetworkName.Equals(e.Info.NetworkName,
                    StringComparison.OrdinalIgnoreCase));
                foreach (var item in foundMessages)
                    item.IsConnected = false;
            }
        }

        private void _supervisor_MessageRemoved(object sender, MessageRemovedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.FavoriteName))
            {
                var favoriteChannels = _channelsList.Where(x => x.NetworkName == e.NetworkName && x.ChannelName == e.FavoriteName);
                if (favoriteChannels.Any())
                {
                    for (int index = 0; index < _channelsList.Count; index++)
                    {
                        var record = _channelsList[index];
                        if (favoriteChannels.Contains(record))
                            _channelsList.Remove(record);
                    }
                }
                var favoriteMentions = _mentionsList.Where(x => x.NetworkName == e.NetworkName && x.ChannelName == e.FavoriteName);
                if (favoriteMentions.Any())
                {
                    for (int index = 0; index < _mentionsList.Count; index++)
                    {
                        var record = _mentionsList[index];
                        if (favoriteMentions.Contains(record))
                            _mentionsList.Remove(record);
                    }
                }
            }
            else
            {
                var foundChannels = _channelsList.Where(x => x.NetworkName == e.NetworkName);
                if (foundChannels.Any())
                {
                    for (int index = 0; index < _channelsList.Count; index++)
                    {
                        var record = _channelsList[index];
                        if (foundChannels.Contains(record))
                            _channelsList.Remove(record);
                    }
                }
                var foundMentions = _mentionsList.Where(x => x.NetworkName == e.NetworkName);
                if (foundMentions.Any())
                {
                    for (int index = 0; index < _mentionsList.Count; index++)
                    {
                        var record = _mentionsList[index];
                        if (foundMentions.Contains(record))
                            _mentionsList.Remove(record);
                    }
                }
                var foundMessages = _messagesList.Where(x => x.NetworkName == e.NetworkName);
                if (foundMessages.Any())
                {
                    for (int index = 0; index < _messagesList.Count; index++)
                    {
                        var record = _messagesList[index];
                        if (foundMessages.Contains(record))
                            _messagesList.Remove(record);
                    }
                }
            }
        }

        private void _supervisor_ChannelJoined(object sender, ChannelStatusEventArgs e)
        {
            var foundChannel = _channelsList.Where(x => x.RecordId == e.SummaryId).FirstOrDefault();
            if (foundChannel == null)
            {
                if (_lastRefreshChannels > DateTime.MinValue)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var summary = new ChannelViewModel();
                        var isLoaded = summary.LoadById(e.SummaryId);
                        if (isLoaded)
                            _channelsList.Add(summary);
                    });
                }
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foundChannel.LoadById(e.SummaryId);
                    foundChannel.IsConnected = true;
                });
            }

            var foundMention = _mentionsList.Where(x => x.NetworkName == e.NetworkName &&
                x.ChannelName == e.ChannelName).FirstOrDefault();
            if (foundMention != null)
                foundMention.IsConnected = true;
        }

        private void _supervisor_ChannelLeft(object sender, ChannelStatusEventArgs e)
        {
            var foundChannel = _channelsList.Where(x => x.RecordId == e.SummaryId).FirstOrDefault();
            if (foundChannel != null)
                foundChannel.IsConnected = false;

            var foundMention = _mentionsList.Where(x => x.NetworkName == e.NetworkName &&
                x.ChannelName == e.ChannelName).FirstOrDefault();
            if (foundMention != null)
                foundMention.IsConnected = true;
        }

        private void _supervisor_ChannelItemReceived(object sender, MessageItemEventArgs e)
        {
            var foundItem = _channelsList.Where(x => x.RecordId == e.SummaryId).FirstOrDefault();
            if (foundItem == null)
            {
                if (_lastRefreshChannels > DateTime.MinValue)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var summary = new ChannelViewModel();
                        var isLoaded = summary.LoadById(e.SummaryId);
                        if (isLoaded)
                            _channelsList.Add(summary);
                    });
                }
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    // TODO: Handle Topic, UnreadCount, making subsequent LoadById's moot
                    foundItem.LoadById(e.SummaryId);
                    foundItem.LoadLastMessage(e.MessageId);
                });
            }
        }

        private void _supervisor_MentionItemReceived(object sender, MessageItemEventArgs e)
        {
            var foundItem = _mentionsList.Where(x => x.RecordId == e.SummaryId).FirstOrDefault();
            if (foundItem == null)
            {
                if (_lastRefreshMentions > DateTime.MinValue)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var summary = new MentionViewModel();
                        var isLoaded = summary.LoadById(e.SummaryId);
                        if (isLoaded)
                            _mentionsList.Add(summary);
                    });
                }
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    // TODO: Handle Topic, UnreadCount, making subsequent LoadById's moot
                    foundItem.LoadById(e.SummaryId);
                    foundItem.LoadLastMessage(e.MessageId);
                });
            }
        }

        private void _supervisor_MessageItemReceived(object sender, MessageItemEventArgs e)
        {
            var foundItem = _messagesList.Where(x => x.RecordId == e.SummaryId).FirstOrDefault();
            if (foundItem == null)
            {
                if (_lastRefreshMessages > DateTime.MinValue)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var summary = new MessageViewModel();
                        var isLoaded = summary.LoadById(e.SummaryId);
                        if (isLoaded)
                            _messagesList.Add(summary);
                    });
                }
            }
            else
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    // TODO: Handle Topic, UnreadCount, making subsequent LoadById's moot
                    foundItem.LoadById(e.SummaryId);
                    foundItem.LoadLastMessage(e.MessageId);
                });
            }
        }

        #endregion

        private void RootLoaded(FrameworkElement layoutRoot)
        {
            // HACK: Execution order: 2
            LayoutRoot = layoutRoot;
            DeferStartup(null);
        }

        private void PivotItemLoaded(PivotItemEventArgs eventArgs)
        {
            // HACK: Execution order: 3
            // You can use PivotItemLoaded or SelectedItem/Index binding.
            // This gets called every time the PivotItem shows so you need to track an IsVMLoaded.
            var pivotControl = eventArgs.Item.Parent as Pivot;
            if (pivotControl != null)
            {
                switch (pivotControl.SelectedIndex)
                {
                    // channels
                    case 0:
                        if (_lastRefreshChannels == DateTime.MinValue)
                        {
                            ProgressIndeterminate = true;
                            ProgressText = "Loading channels...";
                            using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString()))
                            {
                                var channels = unitOfWork.Channels.FindAll().ToList();
                                foreach (var item in channels)
                                {
                                    var itemSummary = new ChannelViewModel(item);
                                    _channelsList.Add(itemSummary);
                                }
                            }
                            _lastRefreshChannels = DateTime.Now;
                            ProgressIndeterminate = false;
                            ProgressText = string.Empty;
                        }
                        break;
                    // mentions
                    case 1:
                        if (_lastRefreshMentions == DateTime.MinValue)
                        {
                            ProgressIndeterminate = true;
                            ProgressText = "Loading mentions...";
                            using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString()))
                            {
                                var mentions = unitOfWork.Mentions.FindAll().ToList();
                                foreach (var item in mentions)
                                {
                                    var itemSummary = new MentionViewModel(item);
                                    _mentionsList.Add(itemSummary);
                                }
                            }
                            _lastRefreshMentions = DateTime.Now;
                            ProgressIndeterminate = false;
                            ProgressText = string.Empty;
                        }
                        break;
                    // messages
                    case 2:
                        if (_lastRefreshMessages == DateTime.MinValue)
                        {
                            using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString()))
                            {
                                var messages = unitOfWork.Messages.FindAll().ToList();
                                ProgressIndeterminate = true;
                                ProgressText = "Loading messages...";
                                foreach (var item in messages)
                                {
                                    var itemSummary = new MessageViewModel(item);
                                    _messagesList.Add(itemSummary);
                                }
                            }
                            _lastRefreshMessages = DateTime.Now;
                            ProgressIndeterminate = false;
                            ProgressText = string.Empty;
                        }
                        break;
                }
            }
         }

        private void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            if (eventArgs.IsNavigationInitiator && eventArgs.NavigationMode == NavigationMode.Back)
                UnselectItem();
            if (!eventArgs.IsNavigationInitiator && eventArgs.NavigationMode == NavigationMode.Back)
            {
                // This gets called wether resuming or first starting. Tread lightly
                if (_supervisor != null)
                    _supervisor.Reconnect(null, true, true);
            }
        }

        private void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
        }

        #region Child navigation

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

        private void ViewAbout()
        {
            var uriString = "/derpirc.Pages;component/Views/AboutView.xaml";
            var uri = new Uri(uriString, UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void SelectChannel()
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (SelectedChannel != null)
                id = SelectedChannel.RecordId.ToString();
            if (!string.IsNullOrEmpty(id))
            {
                uriString = string.Format("/derpirc.Pages;component/Views/ChannelDetailView.xaml?id={0}", Uri.EscapeUriString(id));
                var uri = new Uri(uriString, UriKind.Relative);
                NavigationService.Navigate(uri);
            }
        }

        private void SelectMention()
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (SelectedMention != null)
                id = SelectedMention.RecordId.ToString();
            if (!string.IsNullOrEmpty(id))
            {
                uriString = string.Format("/derpirc.Pages;component/Views/MentionDetailView.xaml?id={0}", Uri.EscapeUriString(id));
                var uri = new Uri(uriString, UriKind.Relative);
                NavigationService.Navigate(uri);
            }
        }

        private void SelectMessage()
        {
            var id = string.Empty;
            var uriString = string.Empty;
            if (SelectedMessage != null)
                id = SelectedMessage.RecordId.ToString();
            if (!string.IsNullOrEmpty(id))
            {
                uriString = string.Format("/derpirc.Pages;component/Views/MessageDetailView.xaml?id={0}", Uri.EscapeUriString(id));
                var uri = new Uri(uriString, UriKind.Relative);
                NavigationService.Navigate(uri);
            }
        }

        private void UnselectItem()
        {
            SelectedChannel = null;
            SelectedMention = null;
            SelectedMessage = null;
        }

        #endregion
    }
}
