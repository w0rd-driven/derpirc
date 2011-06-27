using derpirc.Data;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class MessagesViewModel : ViewModelBase
    {
        private MessagesView _model;
        public MessagesView Model
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

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public MessagesViewModel() : this(new MessagesView()) { }

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public MessagesViewModel(MessagesView model)
        {
            Model = model;
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
                model.Id = 0;
                model.ServerId = 0;
                model.Name = "w0rd-driven";
                //del.LastItem = new IMessage();
                model.Count = 20;
            }
            else
            {
                // code runs "for real": connect to service, etc...
            }
        }

        public override void Cleanup()
        {
            // clean own resources if needed

            base.Cleanup();
        }
    }
}