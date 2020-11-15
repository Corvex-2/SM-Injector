using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM_Inject.Example
{
    public class Injecting
    {
        #region Kernel32 Imports
        [DllImport("kernel32")]
        public static extern IntPtr VirtualAllocEx(int hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32")]
        public static extern bool WriteProcessMemory(int hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, out int lpNumberOfBytesWritten);
        [DllImport("kernel32")]
        public static extern bool CloseHandle(int hObject);
        #endregion

        #region Enums
        public enum AccessRights
        {
            Terminate = 0x1,
            CreateThread = 0x2,
            VmOperation = 0x8,
            VmRead = 0x10,
            VmWrite = 0x20,
            DupHandle = 0x40,
            CreateProcess = 0x80,
            SetQuota = 0x100,
            SetInformation = 0x200,
            QueryInformation = 0x400,
            SuspendResume = 0x800,
            QueryLimitedInformation = 0x1000,
            Synchronize = 0x100000,
            AllAccess = 0x1F0FFF,
        }
        [Flags]
        public enum AllocationType : uint
        {
            MEM_COMMIT = 0x1000,
            MEM_RESERVE = 0x2000,
            MEM_DECOMMIT = 0x4000,
            MEM_RELEASE = 0x8000,
            MEM_RESET = 0x80000,
            MEM_PHYSICAL = 0x400000,
            MEM_TOPDOWN = 0x100000,
            MEM_WRITE_WATCH = 0x200000,
            MEM_LARGE_PAGES = 0x20000000
        }
        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
        #endregion

        [DllImport("Injector.dll")]
        public static extern int StealthyInject(int windowHandle, int address, int nSize);


        public static void Init()
        {
            Console.Title = "";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Init();

            Console.Write("Enter Window Name: ");

            string TargetWindow = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(TargetWindow))
                return;

            while (Process.GetProcessesByName(TargetWindow).Length == 0) { }

            var TargetProcess = Process.GetProcessesByName(TargetWindow).FirstOrDefault();

            while ((DateTime.Now - TargetProcess.StartTime).TotalMilliseconds < 2500) { }

            OpenFileDialog fd = new OpenFileDialog() { Filter = "Dynamic Link Library's|*.dll" };

            var Result = fd.ShowDialog();

            if(Result == DialogResult.OK)
            {
                var DATA = File.ReadAllBytes(fd.FileName);
                Inject(DATA, TargetProcess);
            }
        }

        public static bool Inject(byte[] DLL, Process Target)
        {
            if (Target == null)
                return false;

            var Handle = OpenProcess((uint)AccessRights.AllAccess, false, Target.Id);
            var Address = VirtualAllocEx(Handle, IntPtr.Zero, (uint)DLL.Length, (uint)AllocationType.MEM_COMMIT, (uint)MemoryProtection.ReadWrite);

            WriteProcessMemory(Handle, Address, DLL, (uint)DLL.Length, out var _);

            var Result = StealthyInject((int)Target.MainWindowHandle, (int)Address, DLL.Length);

            CloseHandle(Handle);

            return !(Result <= 0);
        }
    }
}
