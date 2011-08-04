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

        #region ChannelSummaryViewModel

        private static ChannelSummaryViewModel _channelsViewModel;

        /// <summary>
        /// Gets the ChannelSummaryViewModel property.
        /// </summary>
        public static ChannelSummaryViewModel ChannelsStatic
        {
            get
            {
                if (_channelsViewModel == null)
                    CreateChannelSummary();

                return _channelsViewModel;
            }
        }

        /// <summary>
        /// Gets the ChannelSummaryViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ChannelSummaryViewModel ChannelSummaryViewModel
        {
            get { return ChannelsStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the ChannelSummaryViewModel property.
        /// </summary>
        public static void ClearChannelSummary()
        {
            if (_channelsViewModel != null)
                _channelsViewModel.Cleanup();
            _channelsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the ChannelSummaryViewModel property.
        /// </summary>
        public static void CreateChannelSummary()
        {
            if (_channelsViewModel == null)
                _channelsViewModel = new ChannelSummaryViewModel();
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

        #region MentionSummaryViewModel

        private static MentionSummaryViewModel _mentionsViewModel;

        /// <summary>
        /// Gets the MentionSummaryViewModel property.
        /// </summary>
        public static MentionSummaryViewModel MentionsStatic
        {
            get
            {
                if (_mentionsViewModel == null)
                    CreateMentionSummary();

                return _mentionsViewModel;
            }
        }

        /// <summary>
        /// Gets the MentionSummaryViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MentionSummaryViewModel MentionSummaryViewModel
        {
            get { return MentionsStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MentionSummaryViewModel property.
        /// </summary>
        public static void ClearMentionSummary()
        {
            if (_mentionsViewModel != null)
                _mentionsViewModel.Cleanup();
            _mentionsViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MentionSummaryViewModel property.
        /// </summary>
        public static void CreateMentionSummary()
        {
            if (_mentionsViewModel == null)
                _mentionsViewModel = new MentionSummaryViewModel();
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
                _mentionDetailViewModel = new MentionDetailViewModel();
        }

        #endregion

        #region MessageSummaryViewModel

        private static MessageSummaryViewModel _messagesViewModel;

        /// <summary>
        /// Gets the MessageSummaryViewModel property.
        /// </summary>
        public static MessageSummaryViewModel MessagesStatic
        {
            get
            {
                if (_messagesViewModel == null)
                    CreateMessageSummary();

                return _messagesViewModel;
            }
        }

        /// <summary>
        /// Gets the MessageSummaryViewModel property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MessageSummaryViewModel MessageSummaryViewModel
        {
            get { return MessagesStatic; }
        }

        /// <summary>
        /// Provides a deterministic way to delete the MessageSummaryViewModel property.
        /// </summary>
        public static void ClearMessageSummary()
        {
            if (_messagesViewModel != null)
                _messagesViewModel.Cleanup();
            _messagesViewModel = null;
        }

        /// <summary>
        /// Provides a deterministic way to create the MessageSummaryViewModel property.
        /// </summary>
        public static void CreateMessageSummary()
        {
            if (_messagesViewModel == null)
                _messagesViewModel = new MessageSummaryViewModel();
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
            ClearChannelSummary();
            ClearMentionSummary();
            ClearMentionDetail();
            ClearChannelDetail();
            ClearMessageSummary();
            ClearMessageDetail();
        }
    }
}
