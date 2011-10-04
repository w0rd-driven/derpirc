using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc.Views
{
    /// <summary>
    /// Description for ConnectionView.
    /// </summary>
    public partial class ConnectionView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the ConnectionView class.
        /// </summary>
        public ConnectionView()
        {
            InitializeComponent();
        }

        ConnectionViewModel viewModel
        {
            get
            {
                return this.DataContext as ConnectionViewModel;
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