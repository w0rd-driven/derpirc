using derpirc.Data;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class MessagesViewModel : ViewModelBase
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

        #endregion

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public MessagesViewModel() : this(new MessageSummary()) { }

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public MessagesViewModel(MessageSummary model)
        {
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
                model.Name = "w0rd-driven";
                //del.LastItem = new IMessage();
                model.Count = 4;
            }
            else
            {
                // code runs "for real": connect to service, etc...
                model.Name = "w0rd-driven";
                //del.LastItem = new IMessage();
                model.Count = 4;
            }
            Model = model;
        }

        public override void Cleanup()
        {
            // clean own resources if needed

            base.Cleanup();
        }
    }
}