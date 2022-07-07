using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using Serilog;

namespace HASS.Agent.Shared.Functions
{
    /// <summary>
    /// Various unclassified helper functions
    /// </summary>
    public static class SharedHelperFunctions
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();
        
        /// <summary>
        /// Returns the safe variant of the configured device name, or a safe version of the machinename if nothing's stored
        /// </summary>
        /// <returns></returns>
        public static string GetSafeConfiguredDeviceName() =>
            string.IsNullOrEmpty(Variables.DeviceName)
                ? GetSafeDeviceName()
                : Regex.Replace(Variables.DeviceName, @"[^a-zA-Z0-9_-_\s]", "_");

        /// <summary>
        /// Returns a safe version of this machine's name
        /// </summary>
        /// <returns></returns>
        public static string GetSafeDeviceName() => Regex.Replace(Environment.MachineName, @"[^a-zA-Z0-9_-_\s]", "_");

        /// <summary>
        /// Returns a safe (lowercase) version of the provided value
        /// </summary>
        /// <returns></returns>
        public static string GetSafeValue(string value) => Regex.Replace(value, @"[^a-zA-Z0-9_-_\s]", "_");

        /// <summary>
        /// Provides a dictionary containing the pointers and titles of all open windows
        /// </summary>
        /// <returns></returns>
        public static IDictionary<IntPtr, string> GetOpenWindows()
        {
            var shellWindow = GetShellWindow();
            var windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                var length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        /// <summary>
        /// Returns the owner of the supplied process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="includeDomain"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "StringIndexOfIsCultureSpecific.1")]
        public static string GetProcessOwner(Process process, bool includeDomain = true)
        {
            var processHandle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out processHandle);
                using var wi = new WindowsIdentity(processHandle);
                var user = wi.Name;
                if (!includeDomain) return user.Contains(@"\") ? user[(user.IndexOf(@"\") + 1)..] : user;
                else return user;
            }
            catch
            {
                return "NO OWNER";
            }
            finally
            {
                if (processHandle != IntPtr.Zero) CloseHandle(processHandle);
            }
        }

        /// <summary>
        /// Gets the category of the provided enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCategory(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null) return null;
            var attribute = (CategoryAttribute)fieldInfo.GetCustomAttribute(typeof(CategoryAttribute));
            return attribute?.Category ?? "?";
        }

        private static string _everyoneAccountName = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string EveryoneLocalizedAccountName()
        {
            try
            {
                if (!string.IsNullOrEmpty(_everyoneAccountName)) return _everyoneAccountName;

                // one time retrieval
                _everyoneAccountName = Principal.FindByIdentity(new PrincipalContext(ContextType.Machine), IdentityType.Sid, "S-1-1-0")?.Name ?? "Everyone";
                return _everyoneAccountName;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, ex.Message);
                return "Everyone";
            }
        }
    }
}
