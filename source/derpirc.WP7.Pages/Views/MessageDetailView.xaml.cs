using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
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

        MessageDetailViewModel ViewModel { get { return this.DataContext as MessageDetailViewModel; } }

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

        private void Send_KeyUp(object sender, KeyEventArgs e)
        {
            var control = sender as TextBox;
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(control.Text))
                if (ViewModel.SendCommand.CanExecute(null))
                    ViewModel.SendCommand.Execute(null);
        }
    }
}
