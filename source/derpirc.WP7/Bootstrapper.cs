using Funq;
using derpirc.ViewModels;
using derpirc.Helpers;
using System;
using System.Windows;

namespace derpirc
{
    /// <summary>
    /// The boot strapper.
    /// </summary>
    public class BootStrapper : IDisposable
    {
        #region Constants and Fields

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BootStrapper"/> class.
        /// </summary>
        public BootStrapper()
        {
            this.Container = new Container();

            this.Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets Container.
        /// </summary>
        public Container Container { get; private set; }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            if (disposing)
            {
                this.Container.Dispose();
                this.Container = null;
            }
            this.disposed = true;
        }

        #endregion

        #endregion

        #region Methods

        private void Initialize()
        {
            // Initialize the ViewModelFactory with the Funq ViewModelResolver.
            ViewModelFactory.InitializeResolver(new FunqViewModelResolver(Container));

            //Container.Register<IEventAggregator>(new EventAggregator());
            //Container.Register<IDatabase>(new Database());

            Container.Register<INavigationService>(
                c => new ApplicationFrameNavigationService(((App)Application.Current).RootFrame));

            // ViewModel
            Container.Register<MainViewModel>(c => new MainViewModel(c.Resolve<INavigationService>()));
            Container.Register<ChannelViewModel>(c => new ChannelViewModel());
            Container.Register<MentionViewModel>(c => new MentionViewModel());
            Container.Register<MessageViewModel>(c => new MessageViewModel());

            // ViewModel Pages
            Container.Register<ChannelDetailViewModel>(c => new ChannelDetailViewModel());
            Container.Register<MentionDetailViewModel>(c => new MentionDetailViewModel());
            Container.Register<MessageDetailViewModel>(c => new MessageDetailViewModel());

            Container.Register<AboutViewModel>(c => new AboutViewModel());
            Container.Register<ConnectionViewModel>(c => new ConnectionViewModel());

            Container.Register<SettingsViewModel>(c => new SettingsViewModel());
            Container.Register<SettingsUserViewModel>(c => new SettingsUserViewModel());
            Container.Register<SettingsNetworkViewModel>(c => new SettingsNetworkViewModel(c.Resolve<INavigationService>()));
            Container.Register<SettingsNetworkDetailViewModel>(c => new SettingsNetworkDetailViewModel());

            // Model dependencies
        }

        #endregion
    }
}
