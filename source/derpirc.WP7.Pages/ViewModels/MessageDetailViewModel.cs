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
    public class MessageDetailViewModelFactory : ViewModelFactory<MessageDetailViewModel, MessageDetailViewModel> { }

    public class MessageDetailViewModel : ViewModelBase
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

        private Message _model;
        public Message Model
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
        /// Initializes a new instance of the MessageDetailViewModel class.
        /// </summary>
        public MessageDetailViewModel() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the MessageDetailViewModel class.
        /// </summary>
        public MessageDetailViewModel(Message model)
        {
            _messagesList = new ObservableCollection<MessageItem>();

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                PageTitle = "w0rd-driven";
                PageSubTitle = "clefnet";
                SendWatermark = string.Format("chat on {0}", PageSubTitle);

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

                PageTitle = model.Name;
                if (model.Network != null)
                    PageSubTitle = model.Network.Name;
                SendWatermark = string.Format("chat on {0}", PageSubTitle);
            }
            else
            {
                // Code runs "for real": Connect to service, etc...

                // This listens to both the sending of this VM and Receiving from the MainVM
                this.MessengerInstance.Register<GenericMessage<MessageItem>>(this, message =>
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
                var newMessage = new MessageItem();
                newMessage.Summary = Model;
                newMessage.Owner = Owner.Me;
                newMessage.Timestamp = DateTime.Now;
                newMessage.IsRead = true;
                newMessage.Text = SendMessage;
                this.MessengerInstance.Send(new GenericMessage<MessageItem>(this, "out", newMessage));
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
            var model = DataUnitOfWork.Default.Messages.FindById(integerId);
            if (model != null)
                Model = model;
        }

        private void UpdateViewModel(Message model)
        {
            PageTitle = model.Name;
            if (model.Network != null)
                PageSubTitle = model.Network.Name;
            SendMessage = string.Empty;
            SendWatermark = string.Format("chat on {0}", PageSubTitle);
            var messages = DataUnitOfWork.Default.MessageItems.FindBy(x => x.SummaryId == model.Id);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (var item in messages)
                {
                    if (!_messagesList.Contains(item))
                        _messagesList.Add(item);
                }
                PurgeOrphans(messages);
                Messages.View.MoveCurrentToLast();
            });
        }

        private void PurgeOrphans(IQueryable<MessageItem> messages)
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

        private void AddIncoming(MessageItem record)
        {
            if (record.SummaryId == Model.Id)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    // HACK: If Owner.Me, make sure it wasn't added by the UI. This could also serve as a MessageSent event
                    if (record.Owner == Owner.Me)
                    {
                        var foundItem = _messagesList.Where(x => x.Timestamp == record.Timestamp);
                        if (foundItem != null)
                            return;
                    }
                    _messagesList.Add(record);
                    Messages.View.MoveCurrentToLast();
                });
            }
        }

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}
