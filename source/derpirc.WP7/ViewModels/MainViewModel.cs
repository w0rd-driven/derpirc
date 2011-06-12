using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using System.Windows.Data;

namespace derpirc.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties
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