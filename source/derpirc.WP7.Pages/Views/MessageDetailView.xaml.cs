using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc.Views
{
    /// <summary>
    /// Description for MessageDetailView.
    /// </summary>
    public partial class MessageDetailView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the MessageDetailView class.
        /// </summary>
        public MessageDetailView()
        {
            InitializeComponent();
        }

        MessageDetailViewModel viewModel
        {
            get
            {
                return this.DataContext as MessageDetailViewModel;
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
