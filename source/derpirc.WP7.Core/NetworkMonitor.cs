using System;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Net.NetworkInformation;

namespace derpirc.Core
{
    /// <summary>
    /// Handling network detection to conform to certification and personal requirements
    /// Stolen from WP7Contrib's NetworkMonitor
    /// </summary>
    public class NetworkMonitor : IDisposable
    {
        /// <summary>
        /// The status subject.
        /// </summary>
        private BehaviorSubject<NetworkType> _statusSubject = new BehaviorSubject<NetworkType>(DetermineCurrentStatusImpl());

        /// <summary>
        /// The network observer.
        /// </summary>
        private IDisposable _networkObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMonitor"/> class. The frequency to check network connection is set to 500 ms.
        /// </summary>
        public NetworkMonitor() : this(500)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkMonitor"/> class.
        /// </summary>
        /// <param name="frequency">
        /// The frequency to check the network connection.
        /// </param>
        public NetworkMonitor(int frequency)
        {
            this._networkObserver = Observable.Interval(TimeSpan.FromMilliseconds(frequency))
                        .Select(DetermineCurrentStatus)
                        .Subscribe(this._statusSubject);
        }

        /// <summary>
        /// The dispose
        /// </summary>
        public void Dispose()
        {
            if (this._networkObserver != null)
            {
                this._networkObserver.Dispose();
                this._networkObserver = null;
            }

            if (this._statusSubject != null)
            {
                this._statusSubject.OnCompleted();
                this._statusSubject = null;
            }
        }

        /// <summary>
        /// Returns the current status as an observable instance, any future changes will automatically notify subscribers
        /// to the observable.
        /// </summary>
        /// <returns>
        /// Returns an observable status.
        /// </returns>
        public IObservable<NetworkType> Status()
        {
            return this._statusSubject.DistinctUntilChanged();
        }

        /// <summary>
        /// The determine current status.
        /// </summary>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <returns>
        /// Returns the current network type.
        /// </returns>
        private static NetworkType DetermineCurrentStatus(long interval)
        {
            return DetermineCurrentStatusImpl();
        }

        /// <summary>
        /// The determine current status impl.
        /// </summary>
        /// <returns>
        /// Returns the current network type.
        /// </returns>
        private static NetworkType DetermineCurrentStatusImpl()
        {
            var currentNetworkType = NetworkInterface.NetworkInterfaceType;
            switch (currentNetworkType)
            {
                case NetworkInterfaceType.MobileBroadbandCdma:
                    return NetworkType.Cdma;
                case NetworkInterfaceType.MobileBroadbandGsm:
                    return NetworkType.Gsm;
                case NetworkInterfaceType.Wireless80211:
                    return NetworkType.Wireless;
                case NetworkInterfaceType.Ethernet:
                    return NetworkType.Ethernet;
               default:
                    return NetworkType.None;
            }
        }
    }
}
