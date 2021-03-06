﻿using System.Windows.Navigation;
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

        ConnectionViewModel ViewModel { get { return this.DataContext as ConnectionViewModel; } }

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
