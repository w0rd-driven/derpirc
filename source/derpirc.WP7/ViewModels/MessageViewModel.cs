﻿using System;
using System.Linq;
using derpirc.Data;
using derpirc.Data.Models;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class MessageViewModelFactory : ViewModelFactory<MessageViewModel, MessageViewModel> { }

    public class MessageViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

        private int _recordId;
        public int RecordId
        {
            get { return _recordId; }
            set
            {
                if (_recordId == value)
                    return;

                _recordId = value;
                RaisePropertyChanged(() => RecordId);
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

        private bool _messageIsRead;
        public bool MessageIsRead
        {
            get { return _messageIsRead; }
            set
            {
                if (_messageIsRead == value)
                    return;

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

                _unreadCount = value;
                RaisePropertyChanged(() => UnreadCount);
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected == value)
                    return;

                _isConnected = value;
                RaisePropertyChanged(() => IsConnected);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the MessageSummaryViewModel class.
        /// </summary>
        public MessageViewModel() : this(null) { }

        /// <summary>
        /// Initializes a new instance of the MessageSummaryViewModel class.
        /// </summary>
        public MessageViewModel(Message model)
        {
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
                NickName = "w0rd-driven";
                NetworkName = "clefnet";
                UnreadCount = 4;

                MessageIsRead = false;
                MessageSource = "clefnet";
                MessageText = "urmom!";
                MessageTimestamp = DateTime.Now;
            }
            else
            {
                // code runs "for real": connect to service, etc...
                if (model != null)
                    UpdateViewModel(model);
            }
        }

        public bool LoadById(int summaryId)
        {
            var result = false;
            Message model = null;
            using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString() { DatabaseMode = DatabaseMode.ReadOnly }))
            {
                model = unitOfWork.Messages.FindById(summaryId);
                if (model != null)
                {
                    result = true;
                    UpdateViewModel(model);
                }
            }
            return result;
        }

        public void LoadLastMessage(int id)
        {
            using (var unitOfWork = new DataUnitOfWork(new ContextConnectionString() { DatabaseMode = DatabaseMode.ReadOnly }))
            {
                var newMessage = unitOfWork.MessageItems.FindById(id);
                LoadLastMessage(newMessage);
            }
        }

        public void LoadLastMessage(MessageItem message)
        {
            if (message != null)
            {
                MessageIsRead = message.IsRead;
                MessageSource = NetworkName;
                MessageText = message.Text;
                MessageTimestamp = message.Timestamp;
            }
        }

        private void UpdateViewModel(Message model)
        {
            RecordId = model.Id;
            NickName = model.Name;
            if (model.Network != null)
                NetworkName = model.Network.Name;
            MessageSource = NetworkName;
            MessageTimestamp = DateTime.Now;
            //UnreadCount = DataUnitOfWork.Default.MessageItems.Count(x => x.SummaryId == model.Id && x.IsRead == false);
            // NOTE: Lookups on EntitySets only matter during initial load. They do not detect new children for some reason.
            var lastMessage = model.Messages.Count > 0 ? model.Messages.Last() : null;
            LoadLastMessage(lastMessage);
        }

        public override void Cleanup()
        {
            // clean own resources if needed

            base.Cleanup();
        }
    }
}
