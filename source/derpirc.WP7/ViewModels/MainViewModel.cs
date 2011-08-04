using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using derpirc.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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

        public MainViewModel()
        {
            navigationService = new ApplicationFrameNavigationService(((App)Application.Current).RootFrame);

            _channelsList = new ObservableCollection<ChannelSummaryViewModel>();
            var channel = new ChannelSummaryViewModel();
            _channelsList.Add(channel);
            Channels = new CollectionViewSource() { Source = _channelsList };

            _mentionsList = new ObservableCollection<MentionSummaryViewModel>();
            var mention = new MentionSummaryViewModel();
            _mentionsList.Add(mention);
            Mentions = new CollectionViewSource() { Source = _mentionsList };

            _messagesList = new ObservableCollection<MessageSummaryViewModel>();
            var message = new MessageSummaryViewModel();
            _messagesList.Add(message);
            Messages = new CollectionViewSource() { Source = _messagesList };

            //var unitOfWork = new Data.DataUnitOfWork();
            //unitOfWork.InitializeDatabase(true);
            //var sessionSupervisor = new Core.SessionSupervisor(unitOfWork);
        }

        private void SelectChannel()
        {
            NavigationService.Navigate(new Uri("/Views/ChannelDetailView.xaml", UriKind.Relative));
        }

        private void SelectMention()
        {
            NavigationService.Navigate(new Uri("/Views/MentionDetailView.xaml", UriKind.Relative));
        }

        private void SelectMessage()
        {
            NavigationService.Navigate(new Uri("/Views/MessageDetailView.xaml", UriKind.Relative));
        }

        public void LoadData()
        {
            this.IsDataLoaded = true;
        }
    }
}