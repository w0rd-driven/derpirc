using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using derpirc.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace derpirc.ViewModels
{
    public class ChannelDetailViewModel : ViewModelBase
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

        private bool _canSend;
        public bool CanSend
        {
            get { return _canSend; }
            set
            {
                if (_canSend == value)
                    return;

                var oldValue = _canSend;
                _canSend = value;
                RaisePropertyChanged(() => CanSend);
                SendCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _sendCommand;
        public RelayCommand SendCommand
        {
            get
            {
                return _sendCommand ?? (_sendCommand =
                    new RelayCommand(() => this.Send(), () => this.CanSend));
            }
        }

        private bool _canSwitch;
        public bool CanSwitch
        {
            get { return _canSwitch; }
            set
            {
                if (_canSwitch == value)
                    return;

                var oldValue = _canSwitch;
                _canSwitch = value;
                RaisePropertyChanged(() => CanSwitch);
                SwitchCommand.RaiseCanExecuteChanged();
            }
        }

        RelayCommand _switchCommand;
        public RelayCommand SwitchCommand
        {
            get
            {
                return _switchCommand ?? (_switchCommand =
                    new RelayCommand(() => this.Switch(), () => this.CanSwitch));
            }
        }

        RelayCommand<IDictionary<string, string>> _navigatedToCommand;
        public RelayCommand<IDictionary<string, string>> NavigatedToCommand
        {
            get
            {
                return _navigatedToCommand ?? (_navigatedToCommand =
                    new RelayCommand<IDictionary<string, string>>(item => this.OnNavigatedTo(item)));
            }
        }

        #endregion

        #region Properties

        public FrameworkElement LayoutRoot { get; set; }

        private ChannelSummary _model;
        public ChannelSummary Model
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

        private ObservableCollection<ChannelMessage> _messagesList;
        public CollectionViewSource Messages { get; set; }

        private ChannelMessage _selectedItem;
        public ChannelMessage SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

                var oldValue = _selectedItem;
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        private string _sendMessage;
        public string SendMessage
        {
            get { return _sendMessage; }
            set
            {
                if (_sendMessage == value)
                    return;

                var oldValue = _sendMessage;
                _sendMessage = value;
                RaisePropertyChanged(() => SendMessage);
                CanSend = !string.IsNullOrEmpty(SendMessage);
            }
        }

        private string _sendWatermark;
        public string SendWatermark
        {
            get { return _sendWatermark; }
            private set
            {
                if (_sendWatermark == value)
                    return;

                var oldValue = _sendWatermark;
                _sendWatermark = value;
                RaisePropertyChanged(() => SendWatermark);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the ChannelDetailViewModel class.
        /// </summary>
        public ChannelDetailViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Init();
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
            }
        }

        public void Send()
        {

        }

        public void Switch()
        {
            // TODO: Wire in UI to choose where to send messages
            SendWatermark = string.Format("chat on {0}", ServerName);
        }

        private void Init()
        {
            Model = new ChannelSummary();
            Model.Name = "#Test";
            Model.Topic = "This is a test topic";
            Model.Count = 20;
            ServerName = "irc.efnet.org";
            _messagesList = new ObservableCollection<ChannelMessage>();
            _messagesList.Add(new ChannelMessage()
            {
                Summary = Model,
                SummaryId = Model.Id,
                Source = "w0rd-driven",
                Text = "urmom!",
                TimeStamp = DateTime.Now,
                Type = MessageType.Theirs,
            });
            _messagesList.Add(new ChannelMessage()
            {
                Summary = Model,
                SummaryId = Model.Id,
                Source = "derpirc",
                Text = "no, urmom!",
                TimeStamp = DateTime.Now,
                Type = MessageType.Mine,
            });
            ChannelName = Model.Name;
            ChannelTopic = Model.Topic;
            SendWatermark = string.Format("chat on {0}", ServerName);
            Messages = new CollectionViewSource() { Source = _messagesList };
        }

        private void OnNavigatedTo(IDictionary<string, string> queryString)
        {
            Init();
            //TODO: Link Model and VM via Events
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}
