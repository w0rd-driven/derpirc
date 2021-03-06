﻿using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class ViewModelLocator
    {
        private readonly BootStrapper bootStrapper;

        #region MainViewModel

        private static MainViewModel _mainViewModel;

        /// <summary>
        /// Gets the MainViewModel property.
        /// </summary>
        public static MainViewModel MainStatic
        {
            get
            {
                if (_mainViewModel == null)
                    CreateMain();

                return _mainViewModel;
            }
        }

        /// <summary>
        /// Gets the MainViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel MainViewModel
        {
            get { return MainStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MainViewModel property.
        /// </summary>
        public static void ClearMain()
        {
            if (_mainViewModel != null)
                _mainViewModel.Cleanup();
            _mainViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MainViewModel property.
        /// </summary>
        public static void CreateMain()
        {
            if (_mainViewModel == null)
                _mainViewModel = new MainViewModelFactory().ViewModel;
        }

        #endregion

        #region ChannelViewModel

        private static ChannelViewModel _channelsViewModel;

        /// <summary>
        /// Gets the ChannelViewModel property.
        /// </summary>
        public static ChannelViewModel ChannelsStatic
        {
            get
            {
                if (_channelsViewModel == null)
                    CreateChannel();

                return _channelsViewModel;
            }
        }

        /// <summary>
        /// Gets the ChannelViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelViewModel ChannelViewModel
        {
            get { return ChannelsStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ChannelViewModel property.
        /// </summary>
        public static void ClearChannel()
        {
            if (_channelsViewModel != null)
                _channelsViewModel.Cleanup();
            _channelsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ChannelViewModel property.
        /// </summary>
        public static void CreateChannel()
        {
            if (_channelsViewModel == null)
                _channelsViewModel = new ChannelViewModelFactory().ViewModel;
        }

        #endregion

        #region ChannelDetailViewModel

        private static ChannelDetailViewModel _channelDetailViewModel;

        /// <summary>
        /// Gets the ChannelDetailViewModel property.
        /// </summary>
        public static ChannelDetailViewModel ChannelDetailStatic
        {
            get
            {
                if (_channelDetailViewModel == null)
                    CreateChannelDetail();

                return _channelDetailViewModel;
            }
        }

        /// <summary>
        /// Gets the ChannelDetailViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelDetailViewModel ChannelDetailViewModel
        {
            get { return ChannelDetailStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ChannelDetailViewModel property.
        /// </summary>
        public static void ClearChannelDetail()
        {
            if (_channelDetailViewModel != null)
                _channelDetailViewModel.Cleanup();
            _channelDetailViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ChannelDetailViewModel property.
        /// </summary>
        public static void CreateChannelDetail()
        {
            if (_channelDetailViewModel == null)
                _channelDetailViewModel = new ChannelDetailViewModelFactory().ViewModel;
        }

        #endregion

        #region MentionViewModel

        private static MentionViewModel _mentionsViewModel;

        /// <summary>
        /// Gets the MentionViewModel property.
        /// </summary>
        public static MentionViewModel MentionsStatic
        {
            get
            {
                if (_mentionsViewModel == null)
                    CreateMention();

                return _mentionsViewModel;
            }
        }

        /// <summary>
        /// Gets the MentionViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MentionViewModel MentionViewModel
        {
            get { return MentionsStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MentionViewModel property.
        /// </summary>
        public static void ClearMention()
        {
            if (_mentionsViewModel != null)
                _mentionsViewModel.Cleanup();
            _mentionsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MentionViewModel property.
        /// </summary>
        public static void CreateMention()
        {
            if (_mentionsViewModel == null)
                _mentionsViewModel = new MentionViewModelFactory().ViewModel;
        }

        #endregion

        #region MentionDetailViewModel

        private static MentionDetailViewModel _mentionDetailViewModel;

        /// <summary>
        /// Gets the MentionDetailViewModel property.
        /// </summary>
        public static MentionDetailViewModel MentionDetailStatic
        {
            get
            {
                if (_mentionDetailViewModel == null)
                    CreateMentionDetail();

                return _mentionDetailViewModel;
            }
        }

        /// <summary>
        /// Gets the MentionDetailViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MentionDetailViewModel MentionDetailViewModel
        {
            get { return MentionDetailStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MentionDetailViewModel property.
        /// </summary>
        public static void ClearMentionDetail()
        {
            if (_mentionDetailViewModel != null)
                _mentionDetailViewModel.Cleanup();
            _mentionDetailViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MentionDetailViewModel property.
        /// </summary>
        public static void CreateMentionDetail()
        {
            if (_mentionDetailViewModel == null)
                _mentionDetailViewModel = new MentionDetailViewModelFactory().ViewModel;
        }

        #endregion

        #region MessageViewModel

        private static MessageViewModel _messagesViewModel;

        /// <summary>
        /// Gets the MessageViewModel property.
        /// </summary>
        public static MessageViewModel MessagesStatic
        {
            get
            {
                if (_messagesViewModel == null)
                    CreateMessage();

                return _messagesViewModel;
            }
        }

        /// <summary>
        /// Gets the MessageViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MessageViewModel MessageViewModel
        {
            get { return MessagesStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MessageViewModel property.
        /// </summary>
        public static void ClearMessage()
        {
            if (_messagesViewModel != null)
                _messagesViewModel.Cleanup();
            _messagesViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MessageViewModel property.
        /// </summary>
        public static void CreateMessage()
        {
            if (_messagesViewModel == null)
                _messagesViewModel = new MessageViewModelFactory().ViewModel;
        }

        #endregion

        #region MessageDetailViewModel

        private static MessageDetailViewModel _messageDetailViewModel;

        /// <summary>
        /// Gets the MessageDetailViewModel property.
        /// </summary>
        public static MessageDetailViewModel MessageDetailStatic
        {
            get
            {
                if (_messageDetailViewModel == null)
                    CreateMessageDetail();

                return _messageDetailViewModel;
            }
        }

        /// <summary>
        /// Gets the MessageDetailViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MessageDetailViewModel MessageDetailViewModel
        {
            get { return MessageDetailStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MessageDetailViewModel property.
        /// </summary>
        public static void ClearMessageDetail()
        {
            if (_messageDetailViewModel != null)
                _messageDetailViewModel.Cleanup();
            _messageDetailViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MessageDetailViewModel property.
        /// </summary>
        public static void CreateMessageDetail()
        {
            if (_messageDetailViewModel == null)
                _messageDetailViewModel = new MessageDetailViewModelFactory().ViewModel;
        }

        #endregion

        #region SettingsViewModel

        private static SettingsViewModel _settingsViewModel;

        /// <summary>
        /// Gets the SettingsViewModel property.
        /// </summary>
        public static SettingsViewModel SettingsStatic
        {
            get
            {
                if (_settingsViewModel == null)
                    CreateSettings();

                return _settingsViewModel;
            }
        }

        /// <summary>
        /// Gets the SettingsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel SettingsViewModel
        {
            get { return SettingsStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SettingsViewModel property.
        /// </summary>
        public static void ClearSettings()
        {
            if (_settingsViewModel != null)
                _settingsViewModel.Cleanup();
            _settingsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SettingsViewModel property.
        /// </summary>
        public static void CreateSettings()
        {
            if (_settingsViewModel == null)
                _settingsViewModel = new SettingsViewModel();
        }

        #endregion

        #region SettingsUserViewModel

        private static SettingsUserViewModel _settingsUserViewModel;

        /// <summary>
        /// Gets the SettingsUserViewModel property.
        /// </summary>
        public static SettingsUserViewModel SettingsUserStatic
        {
            get
            {
                if (_settingsUserViewModel == null)
                    CreateSettingsUser();

                return _settingsUserViewModel;
            }
        }

        /// <summary>
        /// Gets the SettingsUserViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsUserViewModel SettingsUserViewModel
        {
            get { return SettingsUserStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SettingsUserViewModel property.
        /// </summary>
        public static void ClearSettingsUser()
        {
            if (_settingsUserViewModel != null)
                _settingsUserViewModel.Cleanup();
            _settingsUserViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SettingsUserViewModel property.
        /// </summary>
        public static void CreateSettingsUser()
        {
            if (_settingsUserViewModel == null)
                _settingsUserViewModel = new SettingsUserViewModel();
        }

        #endregion

        #region SettingsNetworkViewModel

        private static SettingsNetworkViewModel _settingsNetworkViewModel;

        /// <summary>
        /// Gets the SettingsNetworkViewModel property.
        /// </summary>
        public static SettingsNetworkViewModel SettingsNetworkStatic
        {
            get
            {
                if (_settingsNetworkViewModel == null)
                    CreateSettingsNetwork();

                return _settingsNetworkViewModel;
            }
        }

        /// <summary>
        /// Gets the SettingsNetworkViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsNetworkViewModel SettingsNetworkViewModel
        {
            get { return SettingsNetworkStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SettingsNetworkViewModel property.
        /// </summary>
        public static void ClearSettingsNetwork()
        {
            if (_settingsNetworkViewModel != null)
                _settingsNetworkViewModel.Cleanup();
            _settingsNetworkViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SettingsNetworkViewModel property.
        /// </summary>
        public static void CreateSettingsNetwork()
        {
            if (_settingsNetworkViewModel == null)
                _settingsNetworkViewModel = new SettingsNetworkViewModelFactory().ViewModel;
        }

        #endregion

        #region SettingsClientViewModel

        private static SettingsClientViewModel _settingsClientViewModel;

        /// <summary>
        /// Gets the SettingsClientViewModel property.
        /// </summary>
        public static SettingsClientViewModel SettingsClientStatic
        {
            get
            {
                if (_settingsClientViewModel == null)
                    CreateSettingsClient();

                return _settingsClientViewModel;
            }
        }

        /// <summary>
        /// Gets the SettingsClientViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsClientViewModel SettingsClientViewModel
        {
            get { return SettingsClientStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SettingsClientViewModel property.
        /// </summary>
        public static void ClearSettingsClient()
        {
            if (_settingsClientViewModel != null)
                _settingsClientViewModel.Cleanup();
            _settingsClientViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SettingsClientViewModel property.
        /// </summary>
        public static void CreateSettingsClient()
        {
            if (_settingsClientViewModel == null)
                _settingsClientViewModel = new SettingsClientViewModelFactory().ViewModel;
        }

        #endregion

        #region SettingsStorageViewModel

        private static SettingsStorageViewModel _settingsStorageViewModel;

        /// <summary>
        /// Gets the SettingsStorageViewModel property.
        /// </summary>
        public static SettingsStorageViewModel SettingsStorageStatic
        {
            get
            {
                if (_settingsStorageViewModel == null)
                    CreateSettingsStorage();

                return _settingsStorageViewModel;
            }
        }

        /// <summary>
        /// Gets the SettingsStorageViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsStorageViewModel SettingsStorageViewModel
        {
            get { return SettingsStorageStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the SettingsStorageViewModel property.
        /// </summary>
        public static void ClearSettingsStorage()
        {
            if (_settingsStorageViewModel != null)
                _settingsStorageViewModel.Cleanup();
            _settingsStorageViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the SettingsStorageViewModel property.
        /// </summary>
        public static void CreateSettingsStorage()
        {
            if (_settingsStorageViewModel == null)
                _settingsStorageViewModel = new SettingsStorageViewModelFactory().ViewModel;
        }

        #endregion

        #region NetworkDetailViewModel

        private static NetworkDetailViewModel _networkDetailViewModel;

        /// <summary>
        /// Gets the NetworkDetailViewModel property.
        /// </summary>
        public static NetworkDetailViewModel NetworkDetailStatic
        {
            get
            {
                if (_networkDetailViewModel == null)
                    CreateNetworkDetail();

                return _networkDetailViewModel;
            }
        }

        /// <summary>
        /// Gets the NetworkDetailViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public NetworkDetailViewModel NetworkDetailViewModel
        {
            get { return NetworkDetailStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the NetworkDetailViewModel property.
        /// </summary>
        public static void ClearNetworkDetail()
        {
            if (_networkDetailViewModel != null)
                _networkDetailViewModel.Cleanup();
            _networkDetailViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the NetworkDetailViewModel property.
        /// </summary>
        public static void CreateNetworkDetail()
        {
            if (_networkDetailViewModel == null)
                _networkDetailViewModel = new NetworkDetailViewModel();
        }

        #endregion

        #region ConnectionViewModel

        private static ConnectionViewModel _connectionViewModel;

        /// <summary>
        /// Gets the ConnectionViewModel property.
        /// </summary>
        public static ConnectionViewModel ConnectionStatic
        {
            get
            {
                if (_connectionViewModel == null)
                    CreateConnection();

                return _connectionViewModel;
            }
        }

        /// <summary>
        /// Gets the ConnectionViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ConnectionViewModel ConnectionViewModel
        {
            get { return ConnectionStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ConnectionViewModel property.
        /// </summary>
        public static void ClearConnection()
        {
            if (_connectionViewModel != null)
                _connectionViewModel.Cleanup();
            _connectionViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ConnectionViewModel property.
        /// </summary>
        public static void CreateConnection()
        {
            if (_connectionViewModel == null)
                _connectionViewModel = new ConnectionViewModel();
        }

        #endregion

        #region AboutViewModel

        private static AboutViewModel _aboutViewModel;

        /// <summary>
        /// Gets the AboutViewModel property.
        /// </summary>
        public static AboutViewModel AboutStatic
        {
            get
            {
                if (_aboutViewModel == null)
                    CreateAbout();

                return _aboutViewModel;
            }
        }

        /// <summary>
        /// Gets the AboutViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public AboutViewModel AboutViewModel
        {
            get { return AboutStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the AboutViewModel property.
        /// </summary>
        public static void ClearAbout()
        {
            if (_aboutViewModel != null)
                _aboutViewModel.Cleanup();
            _aboutViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the AboutViewModel property.
        /// </summary>
        public static void CreateAbout()
        {
            if (_aboutViewModel == null)
                _aboutViewModel = new AboutViewModel();
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (this.bootStrapper == null)
            {
                this.bootStrapper = new BootStrapper();
            }

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view models
            }
            else
            {
                // Create run time view models
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            ClearMain();
            ClearChannel();
            ClearChannelDetail();
            ClearMention();
            ClearMentionDetail();
            ClearMessage();
            ClearMessageDetail();
            ClearSettings();
            ClearSettingsUser();
            ClearSettingsNetwork();
            ClearSettingsClient();
            ClearSettingsStorage();
            ClearNetworkDetail();
            ClearConnection();
            ClearAbout();
        }
    }
}
