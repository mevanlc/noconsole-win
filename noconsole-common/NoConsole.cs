using System;
using System.Runtime.InteropServices;
using System.Text;

namespace noconsole
{
    public static class NoConsole
    {
        public static void CreateProcess(string commandLine, string prefix, bool noWindow)
        {
            var split = new CommandLineSplit(commandLine);
            if (!split.OK)
            {
                MessageBoxError("Invalid command line:\n" + commandLine);
                Environment.Exit(1);
            }
            var fileName = split.FileName;
            var arguments = split.Arguments;
            if (string.IsNullOrEmpty(arguments))
            {
                string basename = System.IO.Path.GetFileNameWithoutExtension(fileName);
                string usage = "Usage: " + basename + " <commandline>";
                MessageBoxError(usage);
                Environment.Exit(1);
            }
            NativeStructs.STARTUPINFO si = new NativeStructs.STARTUPINFO();
            NativeStructs.PROCESS_INFORMATION pi = new NativeStructs.PROCESS_INFORMATION();
            si.cb = Marshal.SizeOf(si);

            string lpApplicationName = null;
            StringBuilder lpCommandLine = new StringBuilder(arguments);
            IntPtr lpProcessAttributes = IntPtr.Zero;
            IntPtr lpThreadAttributes = IntPtr.Zero;
            bool bInheritHandles = false;
            uint dwCreationFlags = noWindow ? NativeConstants.CREATE_NO_WINDOW : 0;
            IntPtr lpEnvironment = IntPtr.Zero;
            string lpCurrentDirectory = null;
            if (prefix != null)
            {
                lpCommandLine.Insert(0, prefix);
            }
            if (Environment.GetEnvironmentVariable("NOCONSOLE_DEBUG") == "1")
            {
                string debugMessage = lpCommandLine.ToString();
                try
                {
                    NativeClipboard.SetClipboardText(debugMessage);
                    debugMessage += "\n\nThe command line was copied to the clipboard.";
                }
                catch (Exception exception)
                {
                    debugMessage += "\n\nThe command line could not be copied to the clipboard:\n"
                        + exception.Message;
                }

                MessageBoxInfo(debugMessage);
            }
            bool success = NativeMethods.CreateProcess(lpApplicationName,
                lpCommandLine,
                lpProcessAttributes,
                lpThreadAttributes,
                bInheritHandles,
                dwCreationFlags,
                lpEnvironment,
                lpCurrentDirectory,
                ref si,
                out pi);
            if (!success)
            {
                MessageBoxError("CreateProcess failed: " + Marshal.GetLastWin32Error());
                Environment.Exit(1);
            }

            NativeMethods.CloseHandle(pi.hThread);
            NativeMethods.CloseHandle(pi.hProcess);
        }

        public static void MessageBoxInfo(string message)
        {
            NativeMessageBox.MessageBox(IntPtr.Zero, message, "noconsole", NativeMessageBox.MB_OK | NativeMessageBox.MB_ICONINFORMATION);
        }

        public static void MessageBoxError(string message)
        {
            NativeMessageBox.MessageBox(IntPtr.Zero, message, "noconsole", NativeMessageBox.MB_OK | NativeMessageBox.MB_ICONERROR);
        }
    }
}
