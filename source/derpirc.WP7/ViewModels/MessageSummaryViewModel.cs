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

        private string _serverName;
        public string ServerName
        {
            get { return _serverName; }
            set
            {
                if (_serverName == value)
                    return;

                var oldValue = _serverName;
                _serverName = value;
                RaisePropertyChanged(() => ServerName);
            }
        }

        private ChannelMessage _lastMessage;
        public ChannelMessage LastMessage
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
            // HACK: Temporary
            model.Name = "w0rd-driven";
            model.Server = new Data.Settings.SessionServer()
            {
                DisplayName = "irc.efnet.org",
            };
            model.LastItem = new ChannelMessage()
            {
                //Summary = model,
                SummaryId = model.Id,
                IsRead = false,
                Source = "w0rd-driven",
                Text = "urmom!",
                TimeStamp = DateTime.Now,
            };
            model.UnreadCount = 4;

            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
            }
            else
            {
                // code runs "for real": connect to service, etc...
            }

            Model = model;
            NickName = Model.Name;
            ServerName = Model.Server.DisplayName;
            UnreadCount = Model.UnreadCount;
            LastMessage = Model.LastItem as ChannelMessage;
            MessageIsRead = LastMessage.IsRead;
            MessageSource = ServerName;
            MessageText = LastMessage.Text;
            MessageTimeStamp = LastMessage.TimeStamp;
        }

        public override void Cleanup()
        {
            // clean own resources if needed

            base.Cleanup();
        }
    }
}