using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc.Views
{
    /// <summary>
    /// Description for SettingsView.
    /// </summary>
    public partial class SettingsView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the SettingsView class.
        /// </summary>
        public SettingsView()
        {
            InitializeComponent();
        }

        SettingsViewModel viewModel
        {
            get
            {
                return this.DataContext as SettingsViewModel;
            }
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
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.Back)
                viewModel.UnselectItemCommand.Execute(null);
        }
    }
}
