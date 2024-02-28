using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

internal static class WinApi
{
    [DllImport("user32.dll")]
    internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string className, string windowName);


    internal static void SetWindowFocus(Process process, bool retry = false, nCmdShow nCmdShow = nCmdShow.SW_SHOWMAXIMIZED)
    {
        IntPtr hWnd = process.MainWindowHandle;
        if (hWnd != IntPtr.Zero)
        {
            ShowWindow(hWnd, (int)nCmdShow);
            SetForegroundWindow(hWnd);
        }

        else if (retry)
        {
            while (hWnd == IntPtr.Zero)
            {
                Thread.Sleep(100);
                hWnd = process.MainWindowHandle;
            }

            ShowWindow(hWnd, (int)nCmdShow);
            SetForegroundWindow(hWnd);
        }
    }

    internal static bool IsWindowFocused(Process process)
    {
        IntPtr activatedHandle = GetForegroundWindow();
        if (activatedHandle == IntPtr.Zero)
            return false; // No window is currently activated

        int procId = process.Id;
        GetWindowThreadProcessId(activatedHandle, out int activeProcId);

        return activeProcId == procId;
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

    [Flags()]
    internal enum SetWindowPosFlags : uint
    {
        AsynchronousWindowPosition = 0x4000,
        DeferErase = 0x2000,
        DrawFrame = 0x0020,
        FrameChanged = 0x0020,
        HideWindow = 0x0080,
        DoNotActivate = 0x0010,
        DoNotCopyBits = 0x0100,
        IgnoreMove = 0x0002,
        DoNotChangeOwnerZOrder = 0x0200,
        DoNotRedraw = 0x0008,
        DoNotReposition = 0x0200,
        DoNotSendChangingEvent = 0x0400,
        IgnoreResize = 0x0001,
        IgnoreZOrder = 0x0004,
        ShowWindow = 0x0040,
        NoFlag = 0x0000,
    }

    internal enum nCmdShow
    {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_SHOWDEFAULT = 10,
        SW_FORCEMINIMIZE = 11,
    }
}