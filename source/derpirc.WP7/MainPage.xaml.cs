using System.Windows.Navigation;
using derpirc.ViewModels;
using Microsoft.Phone.Controls;

namespace derpirc
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        MainViewModel viewModel { get { return this.DataContext as MainViewModel; } }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewModel.NavigatedToCommand.Execute(NavigationContext.QueryString);
            if (e.NavigationMode == NavigationMode.Back)
                viewModel.UnselectItemCommand.Execute(null);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            viewModel.NavigatedFromCommand.Execute(e);
        }
    }
}