using System;
using System.Windows.Navigation;

namespace derpirc.Helpers
{
    /// <summary>
    /// Interface defining the navigation API.
    /// </summary>
    public interface INavigationService
    {
        #region Events

        /// <summary>
        /// Publishes an event when navigating between view models is happening.
        /// </summary>
        event NavigatingCancelEventHandler Navigating;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the one can go back from the current view model.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets the current source of the application _mainFrame.
        /// </summary>
        Uri CurrentSource { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes a navigation back to the previous view model.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Navigates to the view model defined by the source url.
        /// </summary>
        /// <param name="source">
        /// The source url of the view model.
        /// </param>
        /// <returns>
        /// Returns true if the navigation was successful.
        /// </returns>
        bool Navigate(Uri source);

        #endregion
    }
}
