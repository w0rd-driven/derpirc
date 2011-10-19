using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class ChannelViewModelFactory : ViewModelFactory<ChannelViewModel, ChannelViewModel> { }

    public class ChannelViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

        private Channel _model;
        public Channel Model
        {
            get { return _model; }
            set
            {
                if (value != null)
                    UpdateViewModel(value);
                if (_model == value)
                    return;

                var oldValue = _model;
                _model = value;
                RaisePropertyChanged(() => Model);
            }
        }

        private string _channelName;
        public string ChannelName
        {
            get { return _channelName; }
            set
            {
                if (_channelName == value)
                    return;

                var oldValue = _channelName;
                _channelName = value;
                RaisePropertyChanged(() => ChannelName);
            }
        }

        private string _networkName;
        public string NetworkName
        {
            get { return _networkName; }
            set
            {
                if (_networkName == value)
                    return;

                var oldValue = _networkName;
                _networkName = value;
                RaisePropertyChanged(() => NetworkName);
            }
        }

        private string _channelTopic;
        public string ChannelTopic
        {
            get { return _channelTopic; }
            set
            {
                if (_channelTopic == value)
                    return;

                var oldValue = _channelTopic;
                _channelTopic = value;
                RaisePropertyChanged(() => ChannelTopic);
            }
        }

        private ChannelItem _lastMessage;
        public ChannelItem LastMessage
        {
            get { return _lastMessage; }
            set
            {
                if (_lastMessage == value)
                    return;

                var oldValue = _lastMessage;
                _lastMessage = value;
                RaisePropertyChanged(() => LastMessage);
            }
        }

        private bool _messageIsRead;
        public bool MessageIsRead
        {
            get { return _messageIsRead; }
            set
            {
                if (_messageIsRead == value)
                    return;

                var oldValue = _messageIsRead;
                _messageIsRead = value;
                RaisePropertyChanged(() => MessageIsRead);
            }
        }

        private string _messageSource;
        public string MessageSource
        {
            get { return _messageSource; }
            set
            {
                if (_messageSource == value)
                    return;

                var oldValue = _messageSource;
                _messageSource = value;
                RaisePropertyChanged(() => MessageSource);
            }
        }

        private string _messageText;
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                if (_messageText == value)
                    return;

                var oldValue = _messageText;
                _messageText = value;
                RaisePropertyChanged(() => MessageText);
            }
        }

        private DateTime _messageTimestamp;
        public DateTime MessageTimestamp
        {
            get { return _messageTimestamp; }
            set
            {
                if (_messageTimestamp == value)
                    return;

                var oldValue = _messageTimestamp;
                _messageTimestamp = value;
                RaisePropertyChanged(() => MessageTimestamp);
            }
        }

        private int? _unreadCount;
        public int? UnreadCount
        {
            get { return _unreadCount; }
            set
            {
                if (_unreadCount == value)
                    return;

                var oldValue = _unreadCount;
                _unreadCount = value;
                RaisePropertyChanged(() => UnreadCount);
            }
        }

        #endregion

        public ChannelViewModel() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the ChannelSummaryViewModel class.
        /// </summary>
        public ChannelViewModel(Channel model)
        {
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
                ChannelName = "#test";
                NetworkName = "clefnet";
                ChannelTopic = "This is a test topic";
                UnreadCount = 20;

                MessageIsRead = false;
                MessageSource = "w0rd-driven" + " on " + "clefnet";
                MessageText = "urmom!";
                MessageTimestamp = DateTime.Now;
            }
            else
            {
                // code runs "for real": connect to service, etc...
            }

            Model = model;
        }

        public void LoadById(int summaryId)
        {
            var model = DataUnitOfWork.Default.Channels.FindBy(x => x.Id == summaryId).FirstOrDefault();
            if (model != null)
                Model = model;
        }

        private void UpdateViewModel(Channel model)
        {
            ChannelName = model.Name;
            if (model.Network != null)
                NetworkName = model.Network.Name;
            ChannelTopic = model.Topic;
            UnreadCount = model.UnreadCount;
            if (model.LastItem != null)
            {
                LastMessage = model.LastItem as ChannelItem;
                MessageIsRead = LastMessage.IsRead;
                MessageSource = LastMessage.Source + " on " + NetworkName;
                MessageText = LastMessage.Text;
                MessageTimestamp = LastMessage.Timestamp;
            }
            else
            {
                MessageSource = NetworkName;
                MessageTimestamp = DateTime.Now;
            }
        }

        public override void Cleanup()
        {
            // clean own resources if needed

            base.Cleanup();
        }
    }
}