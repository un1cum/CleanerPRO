using System;
using System.Security.Principal;
using Microsoft.VisualBasic.Devices;
using System.Runtime.InteropServices;

namespace Cleaner_PRO
{
    /// <summary>
    /// Computer Helper
    /// </summary>
    /// 
    internal static class NativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, [MarshalAs(UnmanagedType.Bool)] bool disableAllPrivileges, ref Structs.Windows.TokenPrivileges newState, int bufferLength, IntPtr previousState, IntPtr returnLength);

        [DllImport("psapi.dll", SetLastError = true)]
        internal static extern int EmptyWorkingSet(IntPtr hProcess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref long lpLuid);

        [DllImport("ntdll.dll", SetLastError = true)]
        internal static extern UInt32 NtSetSystemInformation(int infoClass, IntPtr info, int length);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetSystemFileCacheSize(IntPtr minimumFileCacheSize, IntPtr maximumFileCacheSize, int flags);
    }
    internal static class Structs
    {
        internal static class Windows
        {
            /// <summary>
            /// Memory Combine Information Ex
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            internal struct MemoryCombineInformationEx
            {
                private readonly IntPtr Handle;
                private readonly IntPtr PagesCombined;
                private readonly IntPtr Flags;
            }

            /// <summary>
            /// System Cache Information structure for x86 working set
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            internal struct SystemCacheInformation32
            {
                private readonly uint CurrentSize;
                private readonly uint PeakSize;
                private readonly uint PageFaultCount;
                internal uint MinimumWorkingSet;
                internal uint MaximumWorkingSet;
                private readonly uint Unused1;
                private readonly uint Unused2;
                private readonly uint Unused3;
                private readonly uint Unused4;
            }

            /// <summary>
            /// System Cache Information structure for x64 working set
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            internal struct SystemCacheInformation64
            {
                private readonly long CurrentSize;
                private readonly long PeakSize;
                private readonly long PageFaultCount;
                internal long MinimumWorkingSet;
                internal long MaximumWorkingSet;
                private readonly long Unused1;
                private readonly long Unused2;
                private readonly long Unused3;
                private readonly long Unused4;
            }

            /// <summary>
            /// Token Privileges structure, used for adjusting token privileges
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            internal struct TokenPrivileges
            {
                internal int Count;
                internal long Luid;
                internal int Attr;
            }
        }
    }
    internal static class Constants
    {
        internal static class Windows
        {
            internal const string DebugPrivilege = "SeDebugPrivilege";
            internal const string IncreaseQuotaName = "SeIncreaseQuotaPrivilege";
            internal const int MemoryFlushModifiedList = 3;
            internal const int MemoryPurgeLowPriorityStandbyList = 5;
            internal const int MemoryPurgeStandbyList = 4;
            internal const int PrivilegeEnabled = 2;
            internal const string ProfileSingleProcessName = "SeProfileSingleProcessPrivilege";
            internal const int SystemCombinePhysicalMemoryInformation = 130;
            internal const int SystemFileCacheInformation = 21;
            internal const int SystemMemoryListInformation = 80;
        }
    }
    internal static class ComputerHelper
    {
        #region Fields

        private static readonly ComputerInfo _computer = new ComputerInfo();

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the current operating system is a 64-bit operating system
        /// </summary>
        /// <value>
        ///   <c>true</c> if it 64-bit; otherwise, <c>false</c>.
        /// </value>
        internal static bool Is64Bit
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is windows 10 or above.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is windows 10 or above; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsWindows8OrAbove
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6.2;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is windows vista or above.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is windows vista or above; otherwise, <c>false</c>.
        /// </value>
        internal static bool IsWindowsVistaOrAbove
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the memory available.
        /// </summary>
        /// <returns></returns>
        internal static long GetMemoryAvailable()
        {
            return Convert.ToInt64(_computer.AvailablePhysicalMemory);
        }

        /// <summary>
        /// Gets the size of the memory.
        /// </summary>
        /// <returns></returns>
        internal static long GetMemorySize()
        {
            return Convert.ToInt64(_computer.TotalPhysicalMemory);
        }

        /// <summary>
        /// Gets the memory usage.
        /// </summary>
        /// <returns></returns>
        internal static long GetMemoryUsage()
        {
            return Convert.ToInt64(100 - ((GetMemoryAvailable() / (double)GetMemorySize()) * 100));
        }

        /// <summary>
        /// Sets the increase privilege.
        /// </summary>
        /// <param name="privilegeName">Name of the privilege.</param>
        /// <returns></returns>
        internal static bool SetIncreasePrivilege(string privilegeName)
        {
            using (WindowsIdentity current = WindowsIdentity.GetCurrent(TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges))
            {
                Structs.Windows.TokenPrivileges newState;
                newState.Count = 1;
                newState.Luid = 0L;
                newState.Attr = Constants.Windows.PrivilegeEnabled;

                if (NativeMethods.LookupPrivilegeValue(null, privilegeName, ref newState.Luid))
                {
                    int result = NativeMethods.AdjustTokenPrivileges(current.Token, false, ref newState, 0, IntPtr.Zero, IntPtr.Zero) ? 1 : 0;
                    return result != 0;
                }
            }

            return false;
        }

        #endregion
    }
}