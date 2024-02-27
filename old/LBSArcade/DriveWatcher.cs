using System;
using System.Management;

namespace LBSArcade
{
    internal static class DriveWatcher
    {
        public static Action<object, EventArrivedEventArgs> DriveConnected;
        public static Action<object, EventArrivedEventArgs> DriveDisconnected;

        private static ManagementEventWatcher connectWatcher;
        private static ManagementEventWatcher disconnectWatcher;
        private static NotImplementedException notImplemented;
        private static NullReferenceException notInitalized;
        private static bool isInitalized = false;

        public static void Init()
        {
            notImplemented = new NotImplementedException("DriveWatcher is only implemented for Windows. Please run LBSArcade on Windows.");
            notInitalized = new NullReferenceException("DriveWatcher was not initalized");

            if (!OperatingSystem.IsWindows()) throw notImplemented;

            isInitalized = true;
            WqlEventQuery queryConnect = new("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            connectWatcher = new();
            connectWatcher.Query = queryConnect;
            connectWatcher.EventArrived += new EventArrivedEventHandler(OnDriveConnected);

            WqlEventQuery queryDisconnect = new("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3");
            disconnectWatcher = new();
            disconnectWatcher.Query = queryDisconnect;
            disconnectWatcher.EventArrived += new EventArrivedEventHandler(OnDriveDisconnected);
        }

        public static void StartWatching()
        {
            if (!OperatingSystem.IsWindows()) throw notImplemented;
            if (!isInitalized) throw notInitalized;

            connectWatcher.Start();
            disconnectWatcher.Start();
        }

        private static void OnDriveConnected(object sender, EventArrivedEventArgs e)
        {
            if (!OperatingSystem.IsWindows()) throw notImplemented;
            DriveConnected?.Invoke(sender, e);
        }

        private static void OnDriveDisconnected(object sender, EventArrivedEventArgs e)
        {
            if (!OperatingSystem.IsWindows()) throw notImplemented;
            DriveDisconnected?.Invoke(sender, e);
        }
    }
}
