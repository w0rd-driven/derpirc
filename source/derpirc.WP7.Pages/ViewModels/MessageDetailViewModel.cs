using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using derpirc.Core;
using derpirc.Data;
using derpirc.Data.Models;
using derpirc.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace derpirc.ViewModels
{
    public class MessageDetailViewModelFactory : ViewModelFactory<MessageDetailViewModel, MessageDetailViewModel> { }

    public class MessageDetailViewModel : ViewModelBase
    {
        #region Commands

        RelayCommand<NavigationEventArgs> _navigatedToCommand;
        public RelayCommand<NavigationEventArgs> NavigatedToCommand
        {
            get
            {
                return _navigatedToCommand ?? (_navigatedToCommand =
                    new RelayCommand<NavigationEventArgs>(eventArgs => this.OnNavigatedTo(eventArgs)));
            }
        }

        RelayCommand<NavigationEventArgs> _navigatedFromCommand;
        public RelayCommand<NavigationEventArgs> NavigatedFromCommand
        {
            get
            {
                return _navigatedFromCommand ?? (_navigatedFromCommand =
                    new RelayCommand<NavigationEventArgs>(eventArgs => this.OnNavigatedFrom(eventArgs)));
            }
        }

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
                DispatcherHelper.CheckBeginInvokeOnUI(() => SendCommand.RaiseCanExecuteChanged());
                RaisePropertyChanged(() => CanSend);
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
                DispatcherHelper.CheckBeginInvokeOnUI(() => SwitchCommand.RaiseCanExecuteChanged());
                RaisePropertyChanged(() => CanSwitch);
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

        #endregion

        #region Properties

        private string _nickName;
        public string NickName
        {
            get { return _nickName; }
            set
            {
                if (_nickName == value)
                    return;

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

                _networkName = value;
                RaisePropertyChanged(() => NetworkName);
            }
        }

        public FrameworkElement LayoutRoot { get; set; }

        private Message _model;
        public Message Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;

                _model = value;
                RaisePropertyChanged(() => Model);
            }
        }

        private ObservableCollection<MessageItem> _messagesList;
        public CollectionViewSource Messages { get; set; }

        private MessageItem _selectedItem;
        public MessageItem SelectedItem
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

        private string _sendText;
        public string SendText
        {
            get { return _sendText; }
            set
            {
                if (_sendText == value)
                    return;

                _sendText = value;
                RaisePropertyChanged(() => SendText);
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

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                if (_isConnected == value)
                    return;

                _isConnected = value;
                RaisePropertyChanged(() => IsConnected);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MessageDetailViewModel class.
        /// </summary>
        public MessageDetailViewModel() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the MessageDetailViewModel class.
        /// </summary>
        public MessageDetailViewModel(Message model)
        {
            _messagesList = new ObservableCollection<MessageItem>();
            Messages = new CollectionViewSource() { Source = _messagesList };

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                NickName = "w0rd-driven";
                NetworkName = "clefnet";
                SendWatermark = string.Format("chat on {0}", NetworkName);

                _messagesList.Add(new MessageItem()
                {
                    Source = "derpirc",
                    Text = "hay",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Me,
                });
                _messagesList.Add(new MessageItem()
                {
                    Source = "derpirc",
                    Text = "sup?",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Me,
                });
                _messagesList.Add(new MessageItem()
                {
                    Source = "w0rd-driven",
                    Text = "nm",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Them,
                });
                _messagesList.Add(new MessageItem()
                {
                    Source = "derpirc",
                    Text = "lame",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Me,
                });
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                SupervisorFacade.Default.StateChanged += this.OnStateChanged;
                SupervisorFacade.Default.MessageItemReceived += this.OnMessageItemReceived;
            }
        }

        private void OnStateChanged(object sender, ClientStatusEventArgs e)
        {
            if (this.Model != null)
                if (e.Info.NetworkName.Equals(this.Model.Network.Name, StringComparison.OrdinalIgnoreCase))
                this.IsConnected = e.Info.State == ClientState.Processed ? true : false;
        }

        private void OnMessageItemReceived(object sender, MessageItemEventArgs e)
        {
            if (this.Model != null)
                if (e.SummaryId == this.Model.Id)
                {
                    MessageItem newMessage = null;
                    using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString() { DatabaseMode = DatabaseMode.ReadOnly }))
                    {
                        newMessage = unitOfWork.MessageItems.FindById(e.MessageId);
                        if (newMessage != null)
                        {
                            // HACK: If Owner.Me, make sure it wasn't added by the UI. This could also serve as a MessageSent event
                            if (newMessage.Owner == Owner.Me)
                            {
                                var foundItem = _messagesList.Where(x => x.Timestamp == newMessage.Timestamp);
                                if (foundItem != null)
                                    return;
                            }
                        }
                    }
                    if (newMessage != null)
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            _messagesList.Add(newMessage);
                            Messages.View.MoveCurrentToLast();
                        });
                }
        }

        private void CheckCanSend()
        {
            var result = false;
            if (!string.IsNullOrEmpty(SendText))
            {
                if (SendText != SendWatermark && IsConnected)
                    result = true;
            }
            CanSend = result;
        }

        private void Send()
        {
            if (!string.IsNullOrEmpty(SendText) && (SendText != SendWatermark))
            {
                var newMessage = new MessageItem();
                newMessage.Summary = Model;
                newMessage.Owner = Owner.Me;
                newMessage.Timestamp = DateTime.Now;
                newMessage.IsRead = true;
                newMessage.Text = SendText;
                Send(newMessage);
                // Steps: Place item in List. Detect send callback. Use a resend hyperlink if necessary

                _messagesList.Add(newMessage);
                Messages.View.MoveCurrentToLast();

                SendText = string.Empty;
            }
        }

        private void Send(MessageItem message)
        {
            SupervisorFacade.Default.SendMessage(message);
        }

        private void Switch()
        {
            // TODO: Wire in UI to choose where to send messages
            SendWatermark = string.Format("chat on {0}", NetworkName);
        }

        private void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            var queryString = eventArgs.Uri.ParseQueryString();
            var id = string.Empty;
            queryString.TryGetValue("id", out id);
            var integerId = -1;
            int.TryParse(id, out integerId);

            ThreadPool.QueueUserWorkItem((object userState) =>
            {
                Message model = null;
                var networkName = string.Empty;
                var messages = new List<MessageItem>();
                using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString() { DatabaseMode = DatabaseMode.ReadOnly }))
                {
                    model = unitOfWork.Messages.FindById(integerId);
                    if (model.Network != null)
                        networkName = model.Network.Name;
                    messages = model.Messages.ToList();
                    if (model != null)
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            NickName = model.Name;
                            if (model.Network != null)
                                NetworkName = model.Network.Name;
                            IsConnected = CheckConnection();
                            SendText = string.Empty;
                            SendWatermark = string.Format("chat on {0}", NetworkName);
                            foreach (var item in messages)
                            {
                                if (!_messagesList.Any(x => x.Id == item.Id))
                                    _messagesList.Add(item);
                            }
                            PurgeOrphans(messages);
                            Messages.View.MoveCurrentToLast();
                        });
                    }
                }
            });
            if (!eventArgs.IsNavigationInitiator)
                SupervisorFacade.Default.Reconnect(null, true);
        }

        private void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
        }

        private void PurgeOrphans(List<MessageItem> messages)
        {
            List<int> messagesToSmash = new List<int>();
            var startingPos = _messagesList.Count - 1;

            for (int index = startingPos; index >= 0; --index)
            {
                var record = _messagesList[index];
                var foundMessage = messages.Where(x => x.Id.Equals(record.Id)).FirstOrDefault();
                if (foundMessage == null)
                    _messagesList.RemoveAt(index);
            }
        }

        private bool CheckConnection()
        {
            var result = false;
            var foundConnection = SupervisorFacade.Default.Connections.FirstOrDefault(x => x.NetworkName == NetworkName);
            if (foundConnection != null)
                result = true;
            return result;
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}
