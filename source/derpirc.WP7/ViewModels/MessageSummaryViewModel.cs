using System;
using derpirc.Data;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    /// <summary>
    /// List-based ItemViewModel
    /// </summary>
    public class MessageSummaryViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

        private MessageSummary _model;
        public MessageSummary Model
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

        private string _nickName;
        public string NickName
        {
            get { return _nickName; }
            set
            {
                if (_nickName == value)
                    return;

                var oldValue = _nickName;
                _nickName = value;
                RaisePropertyChanged(() => NickName);
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

        private MessageItem _lastMessage;
        public MessageItem LastMessage
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

        private DateTime _messageTimeStamp;
        public DateTime MessageTimeStamp
        {
            get { return _messageTimeStamp; }
            set
            {
                if (_messageTimeStamp == value)
                    return;

                var oldValue = _messageTimeStamp;
                _messageTimeStamp = value;
                RaisePropertyChanged(() => MessageTimeStamp);
            }
        }

        private int _unreadCount;
        public int UnreadCount
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

        /// <summary>
        /// Initializes a new instance of the MessageSummaryViewModel class.
        /// </summary>
        public MessageSummaryViewModel() : this(new MessageSummary()) { }

        /// <summary>
        /// Initializes a new instance of the MessageSummaryViewModel class.
        /// </summary>
        public MessageSummaryViewModel(MessageSummary model)
        {
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
                model.Name = "w0rd-driven";
                var network = new Data.Settings.SessionNetwork()
                {
                    Name = "EFNet",
                };
                model.Server = new Data.Settings.SessionServer()
                {
                    HostName = "irc.efnet.org",
                    Network = network,
                };
                model.LastItem = new ChannelItem()
                {
                    //Summary = model,
                    SummaryId = model.Id,
                    IsRead = false,
                    Source = "w0rd-driven",
                    Text = "urmom!",
                    TimeStamp = DateTime.Now,
                };
                model.UnreadCount = 4;
            }
            else
            {
                // code runs "for real": connect to service, etc...
            }

            Model = model;
        }

        private void UpdateViewModel(MessageSummary model)
        {
            NickName = model.Name;
            if (model.Server.Network != null)
                NetworkName = model.Server.Network.Name;
            UnreadCount = model.UnreadCount;
            if (model.LastItem != null)
            {
                LastMessage = model.LastItem as MessageItem;
                MessageIsRead = LastMessage.IsRead;
                MessageSource = NetworkName;
                MessageText = LastMessage.Text;
                MessageTimeStamp = LastMessage.TimeStamp;
            }
        }

        public override void Cleanup()
        {
            // clean own resources if needed

            base.Cleanup();
        }
    }
}