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
        }
    }
}