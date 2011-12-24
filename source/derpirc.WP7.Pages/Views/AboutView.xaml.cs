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

        AboutViewModel ViewModel { get { return this.DataContext as AboutViewModel; } }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.NavigatedToCommand.Execute(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.NavigatedFromCommand.Execute(e);
        }
    }
}
