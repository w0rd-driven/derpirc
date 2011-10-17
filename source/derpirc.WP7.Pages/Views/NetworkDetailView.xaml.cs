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

        NetworkDetailViewModel viewModel
        {
            get { return this.DataContext as NetworkDetailViewModel; }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.NavigatedFromCommand.Execute(null);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.NavigatedToCommand.Execute(NavigationContext.QueryString);
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }
    }
}
