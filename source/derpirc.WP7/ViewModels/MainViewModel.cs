using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace derpirc.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Commands
        RelayCommand<FrameworkElement> _layoutRootCommand;
        public RelayCommand<FrameworkElement> LayoutRootCommand
        {
            get
            {
                return _layoutRootCommand ?? (_layoutRootCommand =
                    new RelayCommand<FrameworkElement>(sender => this.LayoutRoot = sender));
            }
        }
        #endregion

        #region Properties
        public FrameworkElement LayoutRoot { get; set; }

        private ObservableCollection<ChannelsViewModel> _channelsList;
        public CollectionViewSource Channels { get; set; }

        private ObservableCollection<MentionsViewModel> _mentionsList;
        public CollectionViewSource Mentions { get; set; }

        private ObservableCollection<MessagesViewModel> _messagesList;
        public CollectionViewSource Messages { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        #endregion

        public MainViewModel()
        {
            _channelsList = new ObservableCollection<ChannelsViewModel>();
            var channel = new ChannelsViewModel();
            _channelsList.Add(channel);
            _channelsList.Add(channel);
            _channelsList.Add(channel);
            Channels = new CollectionViewSource() { Source = _channelsList };
        }

        public void LoadData()
        {
            this.IsDataLoaded = true;
        }
    }
}