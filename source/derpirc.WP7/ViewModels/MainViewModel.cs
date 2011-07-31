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

        private ObservableCollection<ChannelSummaryViewModel> _channelsList;
        public CollectionViewSource Channels { get; set; }

        private ObservableCollection<MentionSummaryViewModel> _mentionsList;
        public CollectionViewSource Mentions { get; set; }

        private ObservableCollection<MessageSummaryViewModel> _messagesList;
        public CollectionViewSource Messages { get; set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        #endregion

        public MainViewModel()
        {
            _channelsList = new ObservableCollection<ChannelSummaryViewModel>();
            var channel = new ChannelSummaryViewModel();
            _channelsList.Add(channel);
            _channelsList.Add(channel);
            _channelsList.Add(channel);
            Channels = new CollectionViewSource() { Source = _channelsList };

            _mentionsList = new ObservableCollection<MentionSummaryViewModel>();
            var mention = new MentionSummaryViewModel();
            _mentionsList.Add(mention);
            _mentionsList.Add(mention);
            _mentionsList.Add(mention);
            Mentions = new CollectionViewSource() { Source = _mentionsList };

            _messagesList = new ObservableCollection<MessageSummaryViewModel>();
            var message = new MessageSummaryViewModel();
            _messagesList.Add(message);
            _messagesList.Add(message);
            _messagesList.Add(message);
            Messages = new CollectionViewSource() { Source = _messagesList };

            //var unitOfWork = new Data.DataUnitOfWork();
            //unitOfWork.InitializeDatabase(true);
            //var sessionSupervisor = new Core.SessionSupervisor(unitOfWork);
        }

        public void LoadData()
        {
            this.IsDataLoaded = true;
        }
    }
}