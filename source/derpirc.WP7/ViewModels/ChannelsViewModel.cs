using derpirc.Data;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    /// <summary>
    /// List-based ItemViewModel
    /// </summary>
    public class ChannelsViewModel : ViewModelBase
    {
        private ChannelsView _model;
        public ChannelsView Model
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
        public ChannelsViewModel() : this(new ChannelsView()) { }

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public ChannelsViewModel(ChannelsView model)
        {
            Model = model;
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
                model.Id = 0;
                model.ServerId = 0;
                model.Name = "#Test";
                model.Topic = "This is a test topic";
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