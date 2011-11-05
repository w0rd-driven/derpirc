using System.Windows.Navigation;
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

        SettingsViewModel viewModel { get { return this.DataContext as SettingsViewModel; } }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.NavigatedToCommand.Execute(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.NavigatedFromCommand.Execute(e);
        }
    }
}
