using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Security.Principal;
using System.Linq;

namespace Cleaner_PRO
{
    public partial class White1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
            (
            int nLeftRecr,
            int nTopRecr,
            int RightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );
        public White1()
        {
            InitializeComponent();

            notifyIcon1.Visible = false;
            timer2.Enabled = true;
            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

            ulong totalRam = 0;
            ulong frram = 0;

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
                frram = Convert.ToUInt64(objram["FreePhysicalMemory"]);
            }

            int fram2 = Convert.ToInt32(frram);
            int fram3 = Convert.ToInt32(totalRam);
            string fram4 = Convert.ToString(fram2);
            string fram5 = Convert.ToString(fram3);
            double fram6 = Convert.ToDouble(fram4);
            double fram7 = Convert.ToDouble(fram5);
            double percent = fram6 / fram7 * 100;
            int per2 = (int)Math.Round(percent);

            Progressbar1.Value = per2;
            Progressbar1.Text = Progressbar1.Value.ToString() + "%";
            Progressbar2.Value = 0;
            Progressbar2.Text = Progressbar2.Value.ToString() + "%";

            string[] Drives = Environment.GetLogicalDrives();

            foreach (string s in Drives)
            {
                bunifuDropdown1.AddItem(s);
            }

            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
        IniFile iniFile = new IniFile("config.ini");

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private void White1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon1.Visible = true;
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process[] proc = Process.GetProcesses();
            foreach (Process process in proc)
                if (process.ProcessName == "Cleaner PRO")
                {
                    process.Kill();
                }
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void bunifuDropdown1_onItemSelected(object sender, EventArgs e)
        {
            string s = bunifuDropdown1.selectedValue.ToString();
            DriveInfo di = new DriveInfo(@s);
            double Ffree = (di.AvailableFreeSpace / 1024) / 1024;
            double Ftot = (di.TotalSize / 1024) / 1024;
            double percent = Ffree / Ftot * 100;
            int per2 = (int)Math.Round(percent);
            Progressbar2.Value = per2;
            Progressbar2.Text = Progressbar2.Value.ToString() + "%";
        }
        private void circularProgressBar1_Click(object sender, EventArgs e)
        {

        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        [DllImport("shell32.dll")] static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);
        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            try
            {
                string s = bunifuDropdown1.selectedValue.ToString();
                string s2 = iniFile.ReadString("recbin_clean", "config");
                string s3 = iniFile.ReadString("temp_clean", "config");

                if (s2 == "true") SHEmptyRecycleBin(Handle, null, 0);

                if (s3 == "true")
                {
                    string directoryPath = Path.GetTempPath();
                    var di = new DirectoryInfo(directoryPath);

                    foreach (var fse in di.EnumerateFileSystemInfos())
                    {
                        try
                        {
                            fse.Delete();
                        }
                        catch
                        {

                        }
                    }
                }

                DriveInfo di2 = new DriveInfo(@s);
                double Ffree = (di2.AvailableFreeSpace / 1024) / 1024;
                double Ftot = (di2.TotalSize / 1024) / 1024;
                double percent = Ffree / Ftot * 100;
                int per2 = (int)Math.Round(percent);
                Progressbar2.Value = per2;
                Progressbar2.Text = Progressbar2.Value.ToString() + "%";

                NI.BalloonTipText = "Hard drive has been cleaned";
                NI.BalloonTipTitle = "INFO";
                NI.BalloonTipIcon = ToolTipIcon.Info;
                NI.Icon = this.Icon;
                NI.Visible = true;
                NI.ShowBalloonTip(1000);
            }
            catch
            {
                MessageBox.Show("Select the hard drive to clean", "INFO");
            }
        }
        internal struct TokenPrivileges
        {
            internal int Count;
            internal long Luid;
            internal int Attr;
        }
        private const int PrivilegeEnabled = 2;
        private const int MemoryPurgeStandbyList = 4;
        private const string IncreaseQuotaName = "SeIncreaseQuotaPrivilege";
        private const string ProfileSingleProcessName = "SeProfileSingleProcessPrivilege";
        internal static class NativeMethods
        {
            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]

            internal static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref long pluid);

            [DllImport("advapi32.dll", SetLastError = true)]
            internal static extern bool AdjustTokenPrivileges(IntPtr tokenHandle, bool disableAllPrivileges, ref TokenPrivileges newState, int bufferLength, IntPtr previousState, IntPtr returnLength);
            [DllImport("ntdll.dll")]
            internal static extern uint NtSetSystemInformation(int infoClass, IntPtr info, int length);
            [DllImport("psapi.dll")]
            internal static extern int EmptyWorkingSet(IntPtr hwProc);
            [DllImport("user32.dll")]
            internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
            [DllImport("user32.dll")]
            internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
            [DllImport("user32.dll")]
            internal static extern bool OpenClipboard(IntPtr hWnd);
            [DllImport("user32.dll")]
            internal static extern bool EmptyClipboard();
            [DllImport("user32.dll")]
            internal static extern bool CloseClipboard();
        }
        private bool SetIncreasePrivilege(string privilegeName)
        {
            using (WindowsIdentity current = WindowsIdentity.GetCurrent(TokenAccessLevels.Query | TokenAccessLevels.AdjustPrivileges))
            {
                TokenPrivileges tokenPrivileges;
                tokenPrivileges.Count = 1;
                tokenPrivileges.Luid = 0L;
                tokenPrivileges.Attr = PrivilegeEnabled;

                if (!NativeMethods.LookupPrivilegeValue(null, privilegeName, ref tokenPrivileges.Luid)) throw new Exception("LookupPrivilegeValue: ", new Win32Exception(Marshal.GetLastWin32Error()));
                int adjustTokenPrivilegesRet = NativeMethods.AdjustTokenPrivileges(current.Token, false, ref tokenPrivileges, 0, IntPtr.Zero, IntPtr.Zero) ? 1 : 0;
                if (adjustTokenPrivilegesRet == 0) throw new Exception("AdjustTokenPrivileges: ", new Win32Exception(Marshal.GetLastWin32Error()));
                return adjustTokenPrivilegesRet != 0;
            }
        }
        private bool Is64BitMode()
        {
            bool is64Bit = Marshal.SizeOf(typeof(IntPtr)) == 8;
            return is64Bit;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SystemCacheInformation
        {
            internal uint CurrentSize;
            internal uint PeakSize;
            internal uint PageFaultCount;
            internal uint MinimumWorkingSet;
            internal uint MaximumWorkingSet;
            internal uint Unused1;
            internal uint Unused2;
            internal uint Unused3;
            internal uint Unused4;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SystemCacheInformation64Bit
        {
            internal long CurrentSize;
            internal long PeakSize;
            internal long PageFaultCount;
            internal long MinimumWorkingSet;
            internal long MaximumWorkingSet;
            internal long Unused1;
            internal long Unused2;
            internal long Unused3;
            internal long Unused4;
        }
        internal enum SystemInformationClass
        {
            SystemFileCacheInformation = 0x0015,
            SystemMemoryListInformation = 0x0050
        }
        private static void CleanProcessesWorkingSet()
        {
            foreach (Process process in Process.GetProcesses().Where(process => process != null))
            {
                try
                {
                    using (process)
                    {
                        if (!process.HasExited && NativeMethods.EmptyWorkingSet(process.Handle) == 0)
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                catch
                {
                }
            }
        }
        private void bunifuThinButton22_Click(object sender, EventArgs e)
        {
            long totalMemory = GC.GetTotalMemory(false);

            GC.Collect(1, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            if (SetIncreasePrivilege(IncreaseQuotaName))
            {
                uint ntSetSystemInformationRet;
                int systemInfoLength;
                GCHandle gcHandle;

                if (!Is64BitMode())
                {
                    SystemCacheInformation cacheInformation =
                        new SystemCacheInformation
                        {
                            MinimumWorkingSet = uint.MaxValue,
                            MaximumWorkingSet = uint.MaxValue
                        };
                    systemInfoLength = Marshal.SizeOf(cacheInformation);
                    gcHandle = GCHandle.Alloc(cacheInformation, GCHandleType.Pinned);
                    ntSetSystemInformationRet = NativeMethods.NtSetSystemInformation((int)SystemInformationClass.SystemFileCacheInformation, gcHandle.AddrOfPinnedObject(), systemInfoLength);
                    gcHandle.Free();
                }

                else
                {
                    SystemCacheInformation64Bit information64Bit =
                        new SystemCacheInformation64Bit
                        {
                            MinimumWorkingSet = -1L,
                            MaximumWorkingSet = -1L
                        };
                    systemInfoLength = Marshal.SizeOf(information64Bit);
                    gcHandle = GCHandle.Alloc(information64Bit, GCHandleType.Pinned);
                    ntSetSystemInformationRet = NativeMethods.NtSetSystemInformation((int)SystemInformationClass.SystemFileCacheInformation, gcHandle.AddrOfPinnedObject(), systemInfoLength);
                    gcHandle.Free();
                }
            }

            CleanProcessesWorkingSet();
            CleanSystemWorkingSet();
            CleanStandbyList();

            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

            ulong totalRam = 0;
            ulong frram = 0;

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
                frram = Convert.ToUInt64(objram["FreePhysicalMemory"]);
            }

            int fram2 = Convert.ToInt32(frram);
            int fram3 = Convert.ToInt32(totalRam);
            string fram4 = Convert.ToString(fram2);
            string fram5 = Convert.ToString(fram3);
            double fram6 = Convert.ToDouble(fram4);
            double fram7 = Convert.ToDouble(fram5);
            double percent = fram6 / fram7 * 100;
            int per2 = (int)Math.Round(percent);

            Progressbar1.Value = per2;
            Progressbar1.Text = Progressbar1.Value.ToString() + "%";

            NI.BalloonTipText = "RAM has been cleaned";
            NI.BalloonTipTitle = "INFO";
            NI.BalloonTipIcon = ToolTipIcon.Info;
            NI.Icon = this.Icon;
            NI.Visible = true;
            NI.ShowBalloonTip(1000);
        }
        private void bunifuThinButton23_Click(object sender, EventArgs e)
        {
            try
            {
                string s = bunifuDropdown1.selectedValue.ToString();
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/C " + "defrag" + s + ":";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Verb = "runas";
                p.Start();

                NI.BalloonTipText = "Optimization was successful";
                NI.BalloonTipTitle = "INFO";
                NI.BalloonTipIcon = ToolTipIcon.Info;
                NI.Icon = this.Icon;
                NI.Visible = true;
                NI.ShowBalloonTip(1000);
            }
            catch
            {
                MessageBox.Show("Select a drive", "INFO");
            }
        }
        private void label6_Click(object sender, EventArgs e)
        {

        }
        internal static bool Is64Bit
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }
        private static void CleanSystemWorkingSet()
        {
            if (!ComputerHelper.IsWindowsVistaOrAbove)
            {
                return;
            }

            if (!ComputerHelper.SetIncreasePrivilege(Constants.Windows.IncreaseQuotaName))
            {
                return;
            }

            GCHandle handle = GCHandle.Alloc(0);

            try
            {
                object systemCacheInformation;

                if (ComputerHelper.Is64Bit)
                    systemCacheInformation = new Structs.Windows.SystemCacheInformation64 { MinimumWorkingSet = -1L, MaximumWorkingSet = -1L };
                else
                    systemCacheInformation = new Structs.Windows.SystemCacheInformation32 { MinimumWorkingSet = uint.MaxValue, MaximumWorkingSet = uint.MaxValue };

                handle = GCHandle.Alloc(systemCacheInformation, GCHandleType.Pinned);

                int length = Marshal.SizeOf(systemCacheInformation);
            }
            catch
            {

            }
            finally
            {
                try
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
                catch (InvalidOperationException)
                {

                }
            }

            try
            {
                IntPtr fileCacheSize = IntPtr.Subtract(IntPtr.Zero, 1);
            }
            catch
            {

            }
        }
        private static void CleanStandbyList(bool lowPriority = false)
        {
            if (!ComputerHelper.IsWindowsVistaOrAbove)
            {
                return;
            }

            if (!ComputerHelper.SetIncreasePrivilege(Constants.Windows.ProfileSingleProcessName))
            {
                return;
            }

            object memoryPurgeStandbyList = lowPriority ? Constants.Windows.MemoryPurgeLowPriorityStandbyList : Constants.Windows.MemoryPurgeStandbyList;
            GCHandle handle = GCHandle.Alloc(memoryPurgeStandbyList, GCHandleType.Pinned);

            try
            {
                if (NativeMethods.NtSetSystemInformation(Constants.Windows.SystemMemoryListInformation, handle.AddrOfPinnedObject(), Marshal.SizeOf(memoryPurgeStandbyList)) != (int)Enums.Windows.SystemErrorCode.ERROR_SUCCESS)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            catch
            {
            }

            finally
            {
                try
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }
        private void White1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void White1_Load(object sender, EventArgs e)
        {

        }

        private NotifyIcon NI = new NotifyIcon();
        private void timer1_Tick(object sender, EventArgs e)
        {
            long totalMemory = GC.GetTotalMemory(false);

            GC.Collect(1, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            if (SetIncreasePrivilege(IncreaseQuotaName))
            {
                uint ntSetSystemInformationRet;
                int systemInfoLength;
                GCHandle gcHandle;
                if (!Is64BitMode())
                {
                    SystemCacheInformation cacheInformation =
                        new SystemCacheInformation
                        {
                            MinimumWorkingSet = uint.MaxValue,
                            MaximumWorkingSet = uint.MaxValue
                        };
                    systemInfoLength = Marshal.SizeOf(cacheInformation);
                    gcHandle = GCHandle.Alloc(cacheInformation, GCHandleType.Pinned);
                    ntSetSystemInformationRet = NativeMethods.NtSetSystemInformation((int)SystemInformationClass.SystemFileCacheInformation, gcHandle.AddrOfPinnedObject(), systemInfoLength);
                    gcHandle.Free();
                }
                else
                {
                    SystemCacheInformation64Bit information64Bit =
                        new SystemCacheInformation64Bit
                        {
                            MinimumWorkingSet = -1L,
                            MaximumWorkingSet = -1L
                        };
                    systemInfoLength = Marshal.SizeOf(information64Bit);
                    gcHandle = GCHandle.Alloc(information64Bit, GCHandleType.Pinned);
                    ntSetSystemInformationRet = NativeMethods.NtSetSystemInformation((int)SystemInformationClass.SystemFileCacheInformation, gcHandle.AddrOfPinnedObject(), systemInfoLength);
                    gcHandle.Free();
                }
            }

            CleanProcessesWorkingSet();
            CleanSystemWorkingSet();
            CleanStandbyList();

            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

            ulong totalRam = 0;
            ulong frram = 0;

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
                frram = Convert.ToUInt64(objram["FreePhysicalMemory"]);
            }

            int fram2 = Convert.ToInt32(frram);
            int fram3 = Convert.ToInt32(totalRam);
            string fram4 = Convert.ToString(fram2);
            string fram5 = Convert.ToString(fram3);
            double fram6 = Convert.ToDouble(fram4);
            double fram7 = Convert.ToDouble(fram5);
            double percent = fram6 / fram7 * 100;
            int per2 = (int)Math.Round(percent);

            Progressbar1.Value = per2;
            Progressbar1.Text = Progressbar1.Value.ToString() + "%";

            string s2 = iniFile.ReadString("recbin_clean", "config");
            string s3 = iniFile.ReadString("temp_clean", "config");

            if (s2 == "true") SHEmptyRecycleBin(Handle, null, 0);

            if (s3 == "true")
            {
                string directoryPath = Path.GetTempPath();
                var di = new DirectoryInfo(directoryPath);

                foreach (var fse in di.EnumerateFileSystemInfos())
                {
                    try
                    {
                        fse.Delete();
                    }
                    catch
                    {

                    }
                }
            }

            string s4 = iniFile.ReadString("interval", "config");
            NI.BalloonTipText = "Cleaning was successful, next cleaning in " + s4;
            NI.BalloonTipTitle = "INFO";
            NI.BalloonTipIcon = ToolTipIcon.Info;
            NI.Icon = this.Icon;
            NI.Visible = true;
            NI.ShowBalloonTip(1000);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string s = iniFile.ReadString("autoclean", "config");
            if (s == "true")
            {
                string s2 = iniFile.ReadString("interval", "config");
                if (s2 == "15 minutes") timer1.Interval = 900000;
                if (s2 == "30 minutes") timer1.Interval = 1800000;
                if (s2 == "1 hour") timer1.Interval = 3600000;
                if (s2 == "2 hours") timer1.Interval = 7200000;
                if (s2 == "5 hours") timer1.Interval = 18000000;
                if (s2 == "10 hours") timer1.Interval = 36000000;
                if (s2 == "15 hours") timer1.Interval = 54000000;
                if (s2 == "1 day") timer1.Interval = 86400000;

                timer1.Enabled = true;
            }
            else timer1.Enabled = false;

            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem");

            ulong totalRam = 0;
            ulong frram = 0;

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
                frram = Convert.ToUInt64(objram["FreePhysicalMemory"]);
            }

            int fram2 = Convert.ToInt32(frram);
            int fram3 = Convert.ToInt32(totalRam);
            string fram4 = Convert.ToString(fram2);
            string fram5 = Convert.ToString(fram3);
            double fram6 = Convert.ToDouble(fram4);
            double fram7 = Convert.ToDouble(fram5);
            double percent = fram6 / fram7 * 100;
            int per2 = (int)Math.Round(percent);

            Progressbar1.Value = per2;
            Progressbar1.Text = Progressbar1.Value.ToString() + "%";

            try
            {
                string s2 = bunifuDropdown1.selectedValue.ToString();

                DriveInfo di2 = new DriveInfo(@s2);
                double Ffree = (di2.AvailableFreeSpace / 1024) / 1024;
                double Ftot = (di2.TotalSize / 1024) / 1024;
                double percent2 = Ffree / Ftot * 100;
                int per3 = (int)Math.Round(percent2);
                Progressbar2.Value = per3;
                Progressbar2.Text = Progressbar2.Value.ToString() + "%";
            }
            catch
            {

            }
        }

        private void bunifuThinButton24_Click_1(object sender, EventArgs e)
        {
            string s = iniFile.ReadString("theme", "config");
            if (s == "dark")
            {
                Settings se_form = new Settings();
                se_form.Show();
                this.Hide();
            }
            else
            {
                White2 se_form = new White2();
                se_form.Show();
                this.Hide();
            }
        }
    }
}
