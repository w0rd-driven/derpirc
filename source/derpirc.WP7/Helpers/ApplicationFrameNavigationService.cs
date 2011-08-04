using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace derpirc.Helpers
{
    /// <summary>
    /// Application frame navigation service - navigates between view models.
    /// </summary>
    public class ApplicationFrameNavigationService : INavigationService
    {
        #region Constants and Fields

        private const string FailedToGoBack = "Failed to navigate back to previous view model.";
        private const string FailedToNavigate = "Failed to navigate to view model.";

        /// <summary>
        /// The phone application _mainFrame.
        /// </summary>
        private PhoneApplicationFrame _mainFrame;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFrameNavigationService"/> class.
        /// </summary>
        /// <param name="frame">
        /// The application frame.
        /// </param>
        public ApplicationFrameNavigationService(PhoneApplicationFrame frame)
        {
            this._mainFrame = frame;
            if (this._mainFrame == null)
                return;

            this._mainFrame.Navigating += (s, e) =>
            {
                if (this.Navigating != null)
                {
                    this.Navigating(s, e);
                }
            };
        }

        #endregion

        #region Events

        /// <summary>
        /// Publishes an event when navigating between view models is happening.
        /// </summary>
        public event NavigatingCancelEventHandler Navigating;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the one can go back from the current view model.
        /// </summary>
        public bool CanGoBack
        {
            get
            {
                return this._mainFrame.CanGoBack;
            }
        }

        /// <summary>
        /// Gets the current source of the application _mainFrame.
        /// </summary>
        public Uri CurrentSource
        {
            get
            {
                return this._mainFrame.CurrentSource;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region INavigationService

        /// <summary>
        /// Executes a navigation back to the previous view model.
        /// </summary>
        public void GoBack()
        {
            try
            {
                if (this.EnsureMainFrame() && this._mainFrame.CanGoBack)
                {
                    this._mainFrame.GoBack();
                }
            }
            catch (Exception exn)
            {
                throw new Exception(FailedToGoBack, exn);
            }
        }

        /// <summary>
        /// Navigates to the view model defined by the source url.
        /// </summary>
        /// <param name="source">
        /// The source url of the view model.
        /// </param>
        /// <returns>
        /// Returns true if the navigation was successful.
        /// </returns>
        public bool Navigate(Uri source)
        {
            try
            {
                return this.EnsureMainFrame() && this._mainFrame.Navigate(source);
            }
            catch (Exception exn)
            {
                throw new Exception(FailedToNavigate, exn);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Ensures this is the main _mainFrame.
        /// </summary>
        /// <returns>
        /// Returns true if the _mainFrame is the main _mainFrame.
        /// </returns>
        private bool EnsureMainFrame()
        {
            if (this._mainFrame != null)
            {
                return true;
            }

            _mainFrame = Application.Current.RootVisual as PhoneApplicationFrame;

            if (_mainFrame != null)
            {
                // Could be null if the app runs inside a design tool
                _mainFrame.Navigating += (s, e) =>
                {
                    if (Navigating != null)
                    {
                        Navigating(s, e);
                    }
                };

                return true;
            }

            return false;
        }

        #endregion
    }
}
