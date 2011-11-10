using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class MentionDetailViewModelFactory : ViewModelFactory<MentionDetailViewModel, MentionDetailViewModel> { }

    public class MentionDetailViewModel : ViewModelBase
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

        private Mention _model;
        public Mention Model
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

        private ObservableCollection<MentionItem> _messagesList;
        public CollectionViewSource Messages { get; set; }

        private MentionItem _selectedItem;
        public MentionItem SelectedItem
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

                var oldValue = _sendWatermark;
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
        /// Initializes a new instance of the ChannelDetailViewModel class.
        /// </summary>
        public MentionDetailViewModel() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the MentionDetailViewModel class.
        /// </summary>
        public MentionDetailViewModel(Mention model)
        {
            _messagesList = new ObservableCollection<MentionItem>();
            Messages = new CollectionViewSource() { Source = _messagesList };

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                PageTitle = "w0rd-driven";
                PageSubTitle = "clefnet";
                SendWatermark = string.Format("chat on {0}", PageSubTitle);

                _messagesList.Add(new MentionItem()
                {
                    Source = "w0rd-driven",
                    Text = "derpirc: hay",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Them,
                });
                _messagesList.Add(new MentionItem()
                {
                    Source = "w0rd-driven",
                    Text = "derpirc: sup?",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Them,
                });
                _messagesList.Add(new MentionItem()
                {
                    Source = "derpirc",
                    Text = "w0rd-driven: nm",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Me,
                });
                _messagesList.Add(new MentionItem()
                {
                    Source = "w0rd-driven",
                    Text = "derpirc: lame",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Them,
                });
                _messagesList.Add(new MentionItem()
                {
                    Source = "w0rd-driven",
                    Text = "derpirc: urmom!",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Them,
                });
                _messagesList.Add(new MentionItem()
                {
                    Source = "derpirc",
                    Text = "w0rd-driven: no, urmom!",
                    Timestamp = DateTime.Now,
                    Owner = Owner.Me,
                });
            }
            else
            {
                // Code runs "for real": Connect to service, etc...
                SupervisorFacade.Default.StateChanged += this.OnStateChanged;
                SupervisorFacade.Default.MentionItemReceived += this.OnMentionItemReceived;
            }
        }

        private void OnStateChanged(object sender, ClientStatusEventArgs e)
        {
            if (e.Info.NetworkName.Equals(this.Model.Network.Name, StringComparison.OrdinalIgnoreCase))
                this.IsConnected = e.Info.State == ClientState.Processed ? true : false;
        }

        private void OnMentionItemReceived(object sender, MessageItemEventArgs e)
        {
            if (e.SummaryId == Model.Id)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var newMessage = DataUnitOfWork.Default.MentionItems.FindById(e.MessageId);
                    if (newMessage != null)
                    {
                        // HACK: If Owner.Me, make sure it wasn't added by the UI. This could also serve as a MessageSent event
                        if (newMessage.Owner == Owner.Me)
                        {
                            var foundItem = _messagesList.Where(x => x.Timestamp == newMessage.Timestamp);
                            if (foundItem != null)
                                return;
                        }
                        _messagesList.Add(newMessage);
                        Messages.View.MoveCurrentToLast();
                    }
                });
            }
        }

        private void CheckCanSend()
        {
            var result = false;
            if (!string.IsNullOrEmpty(SendMessage))
            {
                if (SendMessage != SendWatermark && IsConnected)
                    result = true;
            }
            CanSend = result;
        }

        private void Send()
        {
            if (!string.IsNullOrEmpty(SendMessage) && (SendMessage != SendWatermark))
            {
                var newMessage = new MentionItem();
                newMessage.Summary = Model;
                newMessage.Owner = Owner.Me;
                newMessage.Timestamp = DateTime.Now;
                newMessage.IsRead = true;
                newMessage.Text = SendMessage;
                Send(newMessage);
                // Steps: Place item in List. Detect send callback. Use a resend hyperlink if necessary

                _messagesList.Add(newMessage);
                Messages.View.MoveCurrentToLast();

                SendMessage = string.Empty;
            }
        }

        private void Send(MentionItem message)
        {
            SupervisorFacade.Default.SendMessage(message);
        }

        private void Switch()
        {
            // TODO: Wire in UI to choose where to send messages
            SendWatermark = string.Format("chat on {0}", PageSubTitle);
        }

        private void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            var queryString = eventArgs.Uri.ParseQueryString();
            var id = string.Empty;
            queryString.TryGetValue("id", out id);
            var integerId = -1;
            int.TryParse(id, out integerId);
            var model = DataUnitOfWork.Default.Mentions.FindById(integerId);
            if (model != null)
                Model = model;
            if (!eventArgs.IsNavigationInitiator)
                SupervisorFacade.Default.Reconnect(null, true);
        }

        private void OnNavigatedFrom(NavigationEventArgs eventArgs)
        {
        }

        private void UpdateViewModel(Mention model)
        {
            PageTitle = model.Name;
            if (model.Network != null)
                PageSubTitle = model.Network.Name;
            IsConnected = model.IsConnected;
            SendMessage = string.Empty;
            SendWatermark = string.Format("chat on {0}", PageSubTitle);
            var messages = DataUnitOfWork.Default.MentionItems.FindBy(x => x.SummaryId == model.Id);
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

        private void PurgeOrphans(IQueryable<MentionItem> messages)
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

        public override void Cleanup()
        {
            // Clean own resources if needed

            base.Cleanup();
        }
    }
}
