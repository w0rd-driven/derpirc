using derpirc.Data;
using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    /// <summary>
    /// List-based ItemViewModel
    /// </summary>
    public class ChannelsViewModel : ViewModelBase
    {
        #region Commands

        #endregion

        #region Properties

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

        #endregion

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public ChannelsViewModel() : this(new ChannelSummary()) { }

        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public ChannelsViewModel(ChannelSummary model)
        {
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
                model.Name = "#Test";
                model.Topic = "This is a test topic";
                //del.LastItem = new IMessage();
                model.Count = 20;
            }
            else
            {
                // code runs "for real": connect to service, etc...
                model.Name = "#Test";
                model.Topic = "This is a test topic";
                //del.LastItem = new IMessage();
                model.Count = 20;
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