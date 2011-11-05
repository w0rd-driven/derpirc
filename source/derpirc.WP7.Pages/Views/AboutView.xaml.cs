using System.Windows.Navigation;
using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc.Views
{
    /// <summary>
    /// Description for AboutView.
    /// </summary>
    public partial class AboutView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the AboutView class.
        /// </summary>
        public AboutView()
        {
            InitializeComponent();
        }

        AboutViewModel viewModel { get { return this.DataContext as AboutViewModel; } }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.NavigatedToCommand.Execute(NavigationContext.QueryString);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.NavigatedFromCommand.Execute(e);
        }
    }
}
