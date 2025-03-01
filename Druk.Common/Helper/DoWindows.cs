﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// 系统信息类 - 获取CPU、内存、磁盘、进程信息 
    /// </summary>
    public class DoWindows
    {
        private int m_ProcessorCount = 0;   //CPU个数 
        private PerformanceCounter pcCpuLoad;   //CPU计数器 
        private long m_PhysicalMemory = 0;   //物理内存 

        private const int GW_HWNDFIRST = 0;
        private const int GW_HWNDNEXT = 2;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 268435456;
        private const int WS_BORDER = 8388608;

        #region AIP声明 
        [DllImport("IpHlpApi.dll")]
        extern static public uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

        [DllImport("User32")]
        private extern static int GetWindow(int hWnd, int wCmd);

        [DllImport("User32")]
        private extern static int GetWindowLongA(int hWnd, int wIndx);

        [DllImport("user32.dll")]
        private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static int GetWindowTextLength(IntPtr hWnd);
        #endregion

        #region 构造函数 
        ///  
        /// 构造函数，初始化计数器等 
        ///  
        public DoWindows()
        {
            //初始化CPU计数器 
            pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            pcCpuLoad.MachineName = ".";
            pcCpuLoad.NextValue();

            //CPU个数 
            m_ProcessorCount = Environment.ProcessorCount;

            //获得物理内存 
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    m_PhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());
                }
            }
        }
        #endregion

        #region CPU个数   CpuCount
        ///  
        /// 获取CPU个数 
        ///  
        public int CpuCount
        {
            get
            {
                return m_ProcessorCount;
            }
        }
        #endregion

        #region CPU占用率   CpuRate
        ///  
        /// 获取CPU占用率 
        ///  
        public float CpuRate
        {
            get
            {
                return pcCpuLoad.NextValue();
            }
        }
        #endregion

        #region 可用内存 
        ///  
        /// 获取可用内存 
        ///  
        public long MemoryAvailable
        {
            get
            {
                long availablebytes = 0;
                //ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PerfRawData_PerfOS_Memory"); 
                //foreach (ManagementObject mo in mos.Get()) 
                //{ 
                //    availablebytes = long.Parse(mo["Availablebytes"].ToString()); 
                //} 
                ManagementClass mos = new ManagementClass("Win32_OperatingSystem");
                foreach (ManagementObject mo in mos.GetInstances())
                {
                    if (mo["FreePhysicalMemory"] != null)
                    {
                        availablebytes = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());
                    }
                }
                return availablebytes;
            }
        }
        #endregion

        #region 物理内存 
        ///  
        /// 获取物理内存 
        ///  
        public long PhysicalMemory
        {
            get
            {
                return m_PhysicalMemory;
            }
        }
        #endregion

        #region 获得分区信息 
        ///  
        /// 获取分区信息 
        ///  
        public List<DiskInfo> GetLogicalDrives()
        {
            var drives = new List<DiskInfo>();
            ManagementClass diskClass = new ManagementClass("Win32_LogicalDisk");
            ManagementObjectCollection disks = diskClass.GetInstances();
            foreach (ManagementObject disk in disks)
            {
                // DriveType.Fixed 为固定磁盘(硬盘) 
                if (int.Parse(disk["DriveType"].ToString()) == (int)DriveType.Fixed)
                {
                    drives.Add(new DiskInfo(disk["Name"].ToString(), disk["Size"].ToLong(), disk["FreeSpace"].ToLong()));
                }
            }
            return drives;
        }
        ///  
        /// 获取特定分区信息 
        ///  
        /// 盘符 
        public List<DiskInfo> GetLogicalDrives(char DriverID)
        {
            var drives = new List<DiskInfo>();
            WqlObjectQuery wmiquery = new WqlObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DeviceID = ’" + DriverID + ":’");
            ManagementObjectSearcher wmifind = new ManagementObjectSearcher(wmiquery);
            foreach (ManagementObject disk in wmifind.Get())
            {
                if (int.Parse(disk["DriveType"].ToString()) == (int)DriveType.Fixed)
                {
                    drives.Add(new DiskInfo(disk["Name"].ToString(), long.Parse(disk["Size"].ToString()), long.Parse(disk["FreeSpace"].ToString())));
                }
            }
            return drives;
        }
        #endregion

        #region 获得进程列表 
        ///  
        /// 获得进程列表 
        ///  
        public List<ProcessInfo> GetProcessInfo()
        {
            var pInfo = new List<ProcessInfo>();
            Process[] processes = Process.GetProcesses();
            foreach (Process instance in processes)
            {
                try
                {
                    pInfo.Add(new ProcessInfo(
                        instance.Id,
                        instance.ProcessName,
                        instance.TotalProcessorTime,
                        instance.WorkingSet64,
                        instance.MainModule.FileName)
                    );
                }
                catch { }
            }
            return pInfo;
        }
        ///  
        /// 获得特定进程信息 
        ///  
        /// 进程名称 
        public List<ProcessInfo> GetProcessInfo(string ProcessName)
        {
            var pInfo = new List<ProcessInfo>();
            Process[] processes = Process.GetProcessesByName(ProcessName);
            foreach (Process instance in processes)
            {
                try
                {
                    pInfo.Add(new ProcessInfo(

                        instance.Id,
                        instance.ProcessName,
                        instance.TotalProcessorTime,
                        instance.WorkingSet64,
                        instance.MainModule.FileName
                    ));
                }
                catch { }
            }
            return pInfo;
        }
        #endregion

        #region 结束指定进程 
        ///  
        /// 结束指定进程 
        ///  
        /// 进程的 Process ID 
        public static void EndProcess(int pid)
        {
            try
            {
                Process process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch { }
        }
        #endregion

        #region 查找所有应用程序标题 
        ///  
        /// 查找所有应用程序标题 
        ///  
        /// 应用程序标题范型 
        public static List<string> FindAllApps(int Handle)
        {
            var Apps = new List<string>();

            int hwCurr;
            hwCurr = GetWindow(Handle, GW_HWNDFIRST);

            while (hwCurr > 0)
            {
                int IsTask = (WS_VISIBLE | WS_BORDER);
                int lngStyle = GetWindowLongA(hwCurr, GWL_STYLE);
                bool TaskWindow = ((lngStyle & IsTask) == IsTask);
                if (TaskWindow)
                {
                    int length = GetWindowTextLength(new IntPtr(hwCurr));
                    StringBuilder sb = new StringBuilder(2 * length + 1);
                    GetWindowText(hwCurr, sb, sb.Capacity);
                    string strTitle = sb.ToString();
                    if (!string.IsNullOrEmpty(strTitle))
                    {
                        Apps.Add(strTitle);
                    }
                }
                hwCurr = GetWindow(hwCurr, GW_HWNDNEXT);
            }

            return Apps;
        }
        #endregion
    }

    #region //对象 硬盘信息

    /// <summary>
    /// 硬盘信息
    /// </summary>
    public class DiskInfo
    {
        public DiskInfo(string name, long size, long free)
        {
            DiskName = name;
            Size = size;
            SizeFree = free;
        }
        /// <summary>
        /// 硬盘名称
        /// </summary>
        public string DiskName { get; }
        /// <summary>
        /// 硬盘全部空间
        /// </summary>
        public long Size { get; }
        /// <summary>
        /// 硬盘全部空间 表述字符串
        /// </summary>
        public string SizeStr { get { return Druk.Common.DoString.GetDiskSizeStr(this.Size); } }
        /// <summary>
        /// 硬盘空闲空间
        /// </summary>
        public long SizeFree { get; }
        /// <summary>
        /// 硬盘空闲空间 表述字符串
        /// </summary>
        public string SizeFreeStr { get { return Druk.Common.DoString.GetDiskSizeStr(this.SizeFree); } }
    }
    #endregion

    #region //对象 进程信息
    /// <summary>
    /// 进程信息
    /// </summary>
    public class ProcessInfo
    {
        public ProcessInfo(int id, string name, TimeSpan time, long size, string exe)
        {
            Id = id;
            ProcessName = name;
            ProcessorTime = time;
            WorkingSet64 = size;
            ExeFullPath = exe;
        }

        /// <summary>
        /// 关联进程的唯一标识符
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// 该进程的名称
        /// </summary>
        public string ProcessName { get; }
        /// <summary>
        /// 此进程的总的处理器时间
        /// </summary>
        public TimeSpan ProcessorTime { get; }
        /// <summary>
        /// 此进程的总的处理器时间 文字表述
        /// </summary>
        public string ProcessorTimeStr { get { return Druk.Common.DoDateTime.GetTimeSpanStr(this.ProcessorTime); } }
        /// <summary>
        /// 为此进程分配的物理内存量 以字节为单位
        /// </summary>
        public long WorkingSet64 { get; }
        /// <summary>
        /// 可执行文件的完整路径
        /// </summary>
        public string ExeFullPath { get; }
    }
    #endregion
}
