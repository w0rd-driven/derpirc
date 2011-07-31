using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class ViewModelLocator
    {
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
                _mainViewModel = new MainViewModel();
        }

        #endregion

        #region ChannelsViewModel

        private static ChannelsViewModel _channelsViewModel;

        /// <summary>
        /// Gets the ChannelsViewModel property.
        /// </summary>
        public static ChannelsViewModel ChannelsStatic
        {
            get
            {
                if (_channelsViewModel == null)
                    CreateChannels();

                return _channelsViewModel;
            }
        }

        /// <summary>
        /// Gets the ChannelsViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelsViewModel ChannelsViewModel
        {
            get { return ChannelsStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ChannelsViewModel property.
        /// </summary>
        public static void ClearChannels()
        {
            if (_channelsViewModel != null)
                _channelsViewModel.Cleanup();
            _channelsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ChannelsViewModel property.
        /// </summary>
        public static void CreateChannels()
        {
            if (_channelsViewModel == null)
                _channelsViewModel = new ChannelsViewModel();
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
                _channelDetailViewModel = new ChannelDetailViewModel();
        }

        #endregion

        #region MessagesViewModel

        private static MessagesViewModel _messagesViewModel;

        /// <summary>
        /// Gets the MessagesViewModel property.
        /// </summary>
        public static MessagesViewModel MessagesStatic
        {
            get
            {
                if (_messagesViewModel == null)
                    CreateMessages();

                return _messagesViewModel;
            }
        }

        /// <summary>
        /// Gets the MessagesViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MessagesViewModel MessagesViewModel
        {
            get { return MessagesStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MessagesViewModel property.
        /// </summary>
        public static void ClearMessages()
        {
            if (_messagesViewModel != null)
                _messagesViewModel.Cleanup();
            _messagesViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MessagesViewModel property.
        /// </summary>
        public static void CreateMessages()
        {
            if (_messagesViewModel == null)
                _messagesViewModel = new MessagesViewModel();
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
                _messageDetailViewModel = new MessageDetailViewModel();
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
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
            ClearChannels();
            ClearChannelDetail();
            ClearMessages();
            ClearMessageDetail();
        }
    }
}
