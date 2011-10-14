using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using derpirc.Data;
using derpirc.Data.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

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

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set
            {
                if (_pageTitle == value)
                    return;

                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        private string _pageSubTitle;
        public string PageSubTitle
        {
            get { return _pageSubTitle; }
            set
            {
                if (_pageSubTitle == value)
                    return;

                _pageSubTitle = value;
                RaisePropertyChanged(() => PageSubTitle);
            }
        }

        public FrameworkElement LayoutRoot { get; set; }

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

                _model = value;
                RaisePropertyChanged(() => Model);
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

                _channelTopic = value;
                RaisePropertyChanged(() => ChannelTopic);
            }
        }

        private ObservableCollection<ChannelItem> _messagesList;
        public CollectionViewSource Messages { get; set; }

        private ChannelItem _selectedItem;
        public ChannelItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;

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

                _sendMessage = value;
                RaisePropertyChanged(() => SendMessage);
                CheckCanSend();
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

                _sendWatermark = value;
                RaisePropertyChanged(() => SendWatermark);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the ChannelDetailViewModel class.
        /// </summary>
        public ChannelDetailViewModel() : this(new Channel()) { }

        /// <summary>
        /// Initializes a new instance of the ChannelDetailViewModel class.
        /// </summary>
        public ChannelDetailViewModel(Channel model)
        {
            _messagesList = new ObservableCollection<ChannelItem>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                model.Name = "#test";
                model.Topic = "This is a test topic";
                var network = new Network()
                {
                    Name = "efnet",
                    HostName = "irc.efnet.org",
                };
                model.Network = network;
                model.UnreadCount = 4;

                _messagesList.Add(new ChannelItem()
                {
                    Source = "derpirc",
                    Text = "hay",
                    Timestamp = DateTime.Now,
                    Type = MessageType.Mine,
                });
                _messagesList.Add(new ChannelItem()
                {
                    Source = "derpirc",
                    Text = "sup?",
                    Timestamp = DateTime.Now,
                    Type = MessageType.Mine,
                });
                _messagesList.Add(new ChannelItem()
                {
                    Source = "w0rd-driven",
                    Text = "nm",
                    Timestamp = DateTime.Now,
                    Type = MessageType.Theirs,
                });
                _messagesList.Add(new ChannelItem()
                {
                    Source = "derpirc",
                    Text = "lame",
                    Timestamp = DateTime.Now,
                    Type = MessageType.Mine,
                });

                PageTitle = model.Name;
                ChannelTopic = model.Topic;
                if (model.Network != null)
                    PageSubTitle = model.Network.Name;
                SendWatermark = string.Format("chat on {0}", PageSubTitle);
            }
            else
            {
                // Code runs "for real": Connect to service, etc...

                // This listens to both the sending of this VM and Receiving from the MainVM
                this.MessengerInstance.Register<GenericMessage<ChannelItem>>(this, message =>
                {
                    var target = message.Target as string;
                    if ((target == "in") && (message.Content != null))
                        AddIncoming(message.Content);
                });
            }

            Messages = new CollectionViewSource() { Source = _messagesList };
        }

        private void CheckCanSend()
        {
            var result = false;
            if (!string.IsNullOrEmpty(SendMessage))
            {
                if (SendMessage != SendWatermark)
                    result = true;
            }
            CanSend = result;
        }

        public void Send()
        {
            if (!string.IsNullOrEmpty(SendMessage) && (SendMessage != SendWatermark))
            {
                var newMessage = new ChannelItem();
                newMessage.Summary = Model;
                newMessage.Type = MessageType.Mine;
                newMessage.Timestamp = DateTime.Now;
                //newMessage.IsRead = true;
                newMessage.Text = SendMessage;
                this.MessengerInstance.Send(new GenericMessage<ChannelItem>(this, "out", newMessage));
                // Steps: Place item in List. Detect send callback. Use a resend hyperlink if necessary

                _messagesList.Add(newMessage);
                Messages.View.MoveCurrentToLast();

                SendMessage = string.Empty;
            }
        }

        public void Switch()
        {
            // TODO: Wire in UI to choose where to send messages
            SendWatermark = string.Format("chat on {0}", PageSubTitle);
        }

        private void OnNavigatedTo(IDictionary<string, string> queryString)
        {
            //TODO: Link Model and VM via Events
            var id = string.Empty;
            queryString.TryGetValue("id", out id);
            var integerId = -1;
            int.TryParse(id, out integerId);
            var model = DataUnitOfWork.Default.Channels.FindBy(x => x.Id == integerId).FirstOrDefault();
            if (model != null)
                Model = model;
        }

        private void UpdateViewModel(Channel model)
        {
            PageTitle = model.Name;
            ChannelTopic = model.Topic;
            if (model.Network != null)
                PageSubTitle = model.Network.Name;
            SendWatermark = string.Format("chat on {0}", PageSubTitle);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _messagesList.Clear();
                foreach (var item in model.Messages)
                {
                    _messagesList.Add(item);
                }
            });
        }

        private void AddIncoming(ChannelItem record)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                // HACK: If MessageType.Mine, make sure it wasn't added by the UI. This could also serve as a MessageSent event
                if (record.Type == MessageType.Mine)
                {
                    var foundItem = _messagesList.Where(x => x.Timestamp == record.Timestamp);
                    if (foundItem != null)
                        return;
                }
                _messagesList.Add(record);
            });
            Messages.View.MoveCurrentToLast();
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}
