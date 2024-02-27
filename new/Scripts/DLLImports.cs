using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

internal static class DLLImports
{
    [DllImport("user32.dll")]
    internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

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