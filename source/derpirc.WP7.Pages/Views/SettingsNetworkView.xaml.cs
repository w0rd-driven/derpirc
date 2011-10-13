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

        SettingsNetworkDetailViewModel viewModel
        {
            get
            {
                return this.DataContext as SettingsNetworkDetailViewModel;
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.NavigatedToCommand.Execute(NavigationContext.QueryString);
        }
    }
}
