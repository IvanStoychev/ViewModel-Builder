using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Backend.Utility
{
    /// <summary>
    /// Performs operations utilizing the interoperability technology.
    /// </summary>
    static class Interop
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        /// <summary>
        /// Opens notepad.exe and send the given text to it.
        /// </summary>
        /// <param name="text">The text to send to notepad.exe.</param>
        internal static void ExportToNotepad(string text)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("notepad");
            startInfo.UseShellExecute = false;
            Process notepad = Process.Start(startInfo);
            notepad.WaitForInputIdle();
            IntPtr child = FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), null, null);
            SendMessage(child, 0x000c, 0, text);
        }
    }
}
