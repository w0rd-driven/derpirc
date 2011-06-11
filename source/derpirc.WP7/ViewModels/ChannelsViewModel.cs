﻿using GalaSoft.MvvmLight;

namespace derpirc.ViewModels
{
    public class ChannelsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the ChannelViewModel class.
        /// </summary>
        public ChannelsViewModel()
        {
            if (IsInDesignMode)
            {
                // code runs in blend --> create design time data.
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