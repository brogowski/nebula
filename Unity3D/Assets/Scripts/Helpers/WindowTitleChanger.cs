using System;
using System.Runtime.InteropServices;

namespace Assets.Scripts.Helpers
{
    static class WindowTitleChanger
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern System.IntPtr FindWindow(System.String className, System.String windowName);


        public static void ChangeTitleName(string oldName, string newName)
        {
            var windowPtr = FindWindow(null, oldName);
            if (windowPtr == IntPtr.Zero)
                return;
            SetWindowText(windowPtr, newName);
        }
    }
}
