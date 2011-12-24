using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using derpirc.Data.Models.Settings;
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

        SettingsViewModel ViewModel { get { return this.DataContext as SettingsViewModel; } }

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

        private void DeleteNetworkItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (sender as MenuItem);
            string header = menuItem.Header.ToString();

            var listBoxItem = this.NetworksList.ItemContainerGenerator.ContainerFromItem(menuItem.DataContext) as ListBoxItem;
            if (listBoxItem == null)
                return;

            var dataContext = listBoxItem.DataContext as Network;
            if (dataContext != null)
                ViewModel.DeleteCommand.Execute(dataContext);
        }
    }
}
