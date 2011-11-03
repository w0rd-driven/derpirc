using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using System.Threading;

/**********************************************/
/*      Copyright of Gabor Dolhai @ 2010      */
/*            under MS-Pl license             */
/*       can be used freely for anybody       */
/*   If you use this code please send me an   */
/*email about your project to dolhaig at gmail*/
/*******************Thanks*********************/
namespace derpirc.Core
{
    public class NetworkDetector
    {
        #region Events
        public event EventHandler<NetworkAvailableEventArgs> OnNetworkON;
        public event EventHandler<NetworkAvailableEventArgs> OnNetworkOFF;
        public event EventHandler<NetworkAvailableEventArgs> OnNetworkChanged;

        public event EventHandler<NetworkStatusEventArgs> OnZuneConnected;
        public event EventHandler<NetworkStatusEventArgs> OnZuneDisconnected;
        public event EventHandler<NetworkStatusEventArgs> OnStatusChanged;
        public event EventHandler<NetworkStatusEventArgs> OnLostNetworkType;

        public event EventHandler<NetworkStatusEventArgs> OnAsyncGetNetworkTypeCompleted;
        #endregion

        private DispatcherTimer updateTimer, pollTimer;
        private Queue<long> requestQueue;  //queue to store the requests timestemps
        private BackgroundWorker networkWorker;
        private bool online = false;
        private NetworkInterfaceType net;
        private NetworkTypeRequestStatus requestStatus;  //current status of the BackgroundWorker
        private bool IsInstantRequestPresent;
        private bool detailedMode;
        private bool isZuneConnected;

        #region Singleton Impl

        // Modified for http://www.yoda.arachsys.com/csharp/singleton.html #4. (Jon Skeet is a code machine)
        private static readonly NetworkDetector defaultInstance = new NetworkDetector();

        public static NetworkDetector Default
        {
            get
            {
                return defaultInstance;
            }
        }

        static NetworkDetector()
        {
        }

        #endregion

        private NetworkDetector()
        {
            requestQueue = new Queue<long>();
            requestQueue.Clear();
            requestStatus = NetworkTypeRequestStatus.Default;
            updateTimer = new DispatcherTimer();
            updateTimer.Tick += new EventHandler(updateTimer_Tick);
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);//there is no need to restart the BGWorker sooner then 300 millisec because the ~3request/sec requestlimit
            //updateTimer.Start();
            pollTimer = new DispatcherTimer();
            pollTimer.Tick += new EventHandler(pollTimer_Tick);
            networkWorker = new BackgroundWorker();
            networkWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(networkWorker_RunWorkerCompleted);
            networkWorker.DoWork += new DoWorkEventHandler(networkWorker_DoWork);
            IsInstantRequestPresent = false;
            detailedMode = false; //by default I hide the framework events for better Developer experience
            isZuneConnected = false;
            SetupNetworkChange();  //signing on the framework event
        }

        #region BackgroundWorker
        private void networkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine(">>>>> GetNetType started " + DateTime.Now.ToString("T") + " >>>>>");
            e.Result = NetworkInterface.NetworkInterfaceType;
        }
        //no need to lock the variables if We do everithing in the completed event handler

        private void networkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DetectOnlineStatus();
            if ((detailedMode) || (net != (NetworkInterfaceType)e.Result))
            {
                //there is no need to get events all the time, just when really changing something or the DetailedMode is true
                if (net != (NetworkInterfaceType)e.Result)
                    RaiseNotify(OnLostNetworkType, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                net = (NetworkInterfaceType)e.Result;
                Debug.WriteLine("      New NetType: " + net.ToString());
                if (net == NetworkInterfaceType.Ethernet)
                    Debug.WriteLine("!!!!! Zune is Connected");
                switch (net)
                {
                    case NetworkInterfaceType.Ethernet:
                        if (!isZuneConnected)
                        {
                            isZuneConnected = true;
                            RaiseNotify(OnZuneConnected, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        }
                        RaiseNotify(OnStatusChanged, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        break;
                    case NetworkInterfaceType.Wireless80211:
                        if (isZuneConnected)
                        {
                            isZuneConnected = false;
                            RaiseNotify(OnZuneDisconnected, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        }
                        RaiseNotify(OnStatusChanged, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        break;
                    case NetworkInterfaceType.MobileBroadbandCdma:
                        if (isZuneConnected)
                        {
                            isZuneConnected = false;
                            RaiseNotify(OnZuneDisconnected, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        }
                        RaiseNotify(OnStatusChanged, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        break;
                    case NetworkInterfaceType.MobileBroadbandGsm:
                        if (isZuneConnected)
                        {
                            isZuneConnected = false;
                            RaiseNotify(OnZuneDisconnected, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        }
                        RaiseNotify(OnStatusChanged, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        break;
                    case NetworkInterfaceType.None:
                        if (!online)
                        {
                            if (isZuneConnected)
                            {
                                /*if we lost all network connection and the Zune was present before,
                                 then we lost the Zune too. Normally when Zune is present and then we got
                                 a None NetType, the PC just lost the internet connection but not the Zune sync*/
                                isZuneConnected = false;
                                RaiseNotify(OnZuneDisconnected, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                            }
                        }
                        RaiseNotify(OnStatusChanged, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        break;
                    default://theoretically we can't get here but better be prepared
                        RaiseNotify(OnStatusChanged, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                        break;
                }
            }
            for (int i = 0; i < requestQueue.Count; i++)
            {
                if (requestQueue.Peek() < DateTime.Now.Ticks) requestQueue.Dequeue();
                //the requests before this moment just got answered
                //if other requests are coming right after this, they will be served inside the next networkWorker_RunWorkerCompleted
            }
            if (requestQueue.Count == 0) updateTimer.Stop();
            if (IsInstantRequestPresent)  //the user requested a single poll
            {
                RaiseNotify(OnAsyncGetNetworkTypeCompleted, new NetworkStatusEventArgs(online, GetCurrentNetworkType()));
                IsInstantRequestPresent = false;
            }
            requestStatus = NetworkTypeRequestStatus.Ended;
            Debug.WriteLine("<<<<< GetNetType ended " + DateTime.Now.ToString("T") + " <<<<<");
        }
        #endregion

        #region Private Functions
        private void EnqueueRequest()
        {
            requestQueue.Enqueue(DateTime.Now.Ticks);
            if (!updateTimer.IsEnabled) updateTimer.Start();
        }

        private void DetectOnlineStatus() //are we connected to any network or not
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!online)
                {
                    online = true; //the network just came back
                    RaiseNotify(OnNetworkON, new NetworkAvailableEventArgs(online));
                    // do what is needed to GoOnline();
                }
            }
            else
            {
                if (online)
                {
                    online = false;  //we just lost all network connectivity
                    RaiseNotify(OnNetworkOFF, new NetworkAvailableEventArgs(online));
                    Debug.WriteLine("-----  No network available.   -----");
                    // do what is needed to GoOffline();
                }
            }
        }

        void pollTimer_Tick(object sender, EventArgs e)
        {
            EnqueueRequest();
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            if (requestQueue.Count > 0)
            {
                if (!networkWorker.IsBusy)
                {
                    requestStatus = NetworkTypeRequestStatus.Started;
                    networkWorker.RunWorkerAsync();
                }
            }
        }

        #region Event Handling
        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("+++++ Changed started " + DateTime.Now.ToString("T") + " +++++");
            DetectOnlineStatus();
            if (detailedMode) RaiseNotify(OnNetworkChanged, new NetworkAvailableEventArgs(online));
            Debug.WriteLine("      IsOnline : " + online.ToString());
            Debug.WriteLine("      Current NetType: " + net.ToString());
            Debug.WriteLine("+++++ Changed Ended " + DateTime.Now.ToString("T"));
            Debug.WriteLine("      Changed GetNetType Launch" + DateTime.Now.ToString("T"));
            EnqueueRequest();
        }

        private void SetupNetworkChange()
        {
            // Get current network availalability and store the
            // initial value of the online variable
            ThreadPool.QueueUserWorkItem(new WaitCallback((objectState) =>
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    online = true;
                    // do what is needed to GoOnline();
                }
                else
                {
                    online = false;
                    // do what is needed to GoOffline();
                }
            }));
            // Now add a network change event handler to indicate network availability
            EnqueueRequest();
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
        }

        protected void RaiseNotify(EventHandler<NetworkStatusEventArgs> handler, NetworkStatusEventArgs e)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected void RaiseNotify(EventHandler<NetworkAvailableEventArgs> handler, NetworkAvailableEventArgs e)
        {
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #endregion

        #region Public Functions and Properties
        public void AsyncGetNetworkType()
        {
            //requestQueue.Enqueue(System.DateTime.Now.Ticks);
            IsInstantRequestPresent = true;
            if (!networkWorker.IsBusy)
            {
                requestStatus = NetworkTypeRequestStatus.Started;
                networkWorker.RunWorkerAsync();
            }
            if (!updateTimer.IsEnabled) updateTimer.Start();
        }

        public void SetNetworkPolling(int Minutes, int Seconds, int Milliseconds)
        {
            pollTimer.Interval = new TimeSpan(0, 0, Minutes, Seconds, Milliseconds);
            if (!pollTimer.IsEnabled) pollTimer.Start();
        }

        public void DisableNetworkPolling()
        {
            if (pollTimer.IsEnabled) pollTimer.Stop();
        }

        public NetworkType GetCurrentNetworkType()
        {
            var result = NetworkType.Undefined;
            switch (net)
            {
                case NetworkInterfaceType.Ethernet:
                    result = NetworkType.Ethernet;
                    break;
                case NetworkInterfaceType.MobileBroadbandCdma:
                    result = NetworkType.Data;
                    break;
                case NetworkInterfaceType.MobileBroadbandGsm:
                    result = NetworkType.Data;
                    break;
                case NetworkInterfaceType.None:
                    result = NetworkType.None;
                    break;
                case NetworkInterfaceType.Unknown:
                    result = NetworkType.Undefined;
                    break;
                case NetworkInterfaceType.Wireless80211:
                    result = NetworkType.Wireless;
                    break;
            }
            return result;
        }

        public NetworkTypeRequestStatus GetRequestStatus()
        {
            //NetworkTypeRequestStatus temp = requestStatus;
            //if (requestStatus == NetworkTypeRequestStatus.Ended) requestStatus = NetworkTypeRequestStatus.Default;
            //return temp;
            return requestStatus;
        }

        public bool DetailedMode
        {
            get
            {
                return detailedMode;
            }
            set
            {
                detailedMode = value;
            }
        }

        public bool GetZuneStatus()
        {
            return isZuneConnected;
        }
        #endregion
    }
}