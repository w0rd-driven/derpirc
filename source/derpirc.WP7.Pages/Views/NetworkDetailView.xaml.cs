using System.Windows.Navigation;
using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc.Views
{
    /// <summary>
    /// Description for SettingsNetworkView.
    /// </summary>
    public partial class SettingsNetworkView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the SettingsNetworkView class.
        /// </summary>
        public SettingsNetworkView()
        {
            InitializeComponent();
        }

        NetworkDetailViewModel ViewModel { get { return this.DataContext as NetworkDetailViewModel; } }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.NavigatedFromCommand.Execute(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.NavigatedToCommand.Execute(e);
        }
    }
}
