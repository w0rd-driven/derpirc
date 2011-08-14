using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc.Views
{
    /// <summary>
    /// Description for MentionDetailView.
    /// </summary>
    public partial class MentionDetailView : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the MentionDetailView class.
        /// </summary>
        public MentionDetailView()
        {
            InitializeComponent();
        }

        MentionDetailViewModel viewModel
        {
            get
            {
                return this.DataContext as MentionDetailViewModel;
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
