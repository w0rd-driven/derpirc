using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc.Views
{
    /// <summary>
    /// Description for ChannelDetailView.
    /// </summary>
    public partial class ChannelDetailView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the ChannelDetailView class.
        /// </summary>
        public ChannelDetailView()
        {
            InitializeComponent();
        }

        ChannelDetailViewModel viewModel
        {
            get
            {
                return this.DataContext as ChannelDetailViewModel;
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
