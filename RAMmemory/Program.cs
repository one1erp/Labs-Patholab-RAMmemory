using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;

namespace RAMmemory
{
    class Program
    {

        static void Main(string[] args)

        {
            while (true)
            {
                string msg = "";

                msg += "\r\n"+ " "+ (DateTime.Now);
                msg += "\r\n" + " " + ("*****************");

                 msg += "\r\n" + " " + ("Computer Situation");
                 msg += "\r\n" + " " + ("*****************");

                Int64 phav = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
                Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
                decimal percentFree = ((decimal)phav / (decimal)tot) * 100;
                decimal percentOccupied = 100 - percentFree;
                 msg += "\r\n" + ("Available Physical Memory (MiB) " + phav.ToString());
                 msg += "\r\n" + ("Total Memory (MiB) " + tot.ToString());
                 msg += "\r\n" + ("Free (%) " + percentFree.ToString("C2"));
                 msg += "\r\n" + ("Occupied (%) " + percentOccupied.ToString("C2"));
                 msg += "\r\n" + ("*****************");
                 msg += "\r\n" + ("Nautilus Process");
                 msg += "\r\n" + ("*****************");

                var proc = Process.GetProcessesByName("Nautilus");
                if (proc.Length == 0)
                     msg += "\r\n" + ("Error : Nautilus isn't working");
                else
                {
                    for (int i = 0; i < proc.Length; i++)
                    {
                         msg += "\r\n" + ("PrivateMemorySize64 " + ConvertKilobytesToMegabytes(proc[i].PrivateMemorySize64));
                         msg += "\r\n" + ("WorkingSet64 " + ConvertKilobytesToMegabytes(proc[i].WorkingSet64));
                         msg += "\r\n" + ("PeakWorkingSet64 " + ConvertKilobytesToMegabytes(proc[i].PeakWorkingSet64));
                         msg += "\r\n" + ("SessionId " + proc[i].SessionId);
                         msg += "\r\n" + ("Id " + proc[i].Id);
                    }
                }
                 msg += "\r\n" + ("------------------------");
                 msg += "\r\n" + ("------------------------");
                Log(msg);
                Thread.Sleep(5000);

            }

        }
        static string ConvertKilobytesToMegabytes(long kilobytes)
        {
            return (kilobytes / 1024f) + " mb";
        }
        static void Log(string msg)
        {
            try
            {
                var fullPath = "C:\\One_Logs\\Memory Log - " + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
                using (FileStream file = new FileStream(fullPath, FileMode.Append, FileAccess.Write))
                {
                    var streamWriter = new StreamWriter(file);
                    streamWriter.WriteLine(DateTime.Now);
                    streamWriter.WriteLine(msg);
                    streamWriter.WriteLine("///////////////////////////////////////////");
                    streamWriter.WriteLine();
                    streamWriter.Close();
                }
            }
            catch
            {
            }
        }


        public static class PerformanceInfo
        {
            [DllImport("psapi.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

            [StructLayout(LayoutKind.Sequential)]
            public struct PerformanceInformation
            {
                public int Size;
                public IntPtr CommitTotal;
                public IntPtr CommitLimit;
                public IntPtr CommitPeak;
                public IntPtr PhysicalTotal;
                public IntPtr PhysicalAvailable;
                public IntPtr SystemCache;
                public IntPtr KernelTotal;
                public IntPtr KernelPaged;
                public IntPtr KernelNonPaged;
                public IntPtr PageSize;
                public int HandlesCount;
                public int ProcessCount;
                public int ThreadCount;
            }

            public static Int64 GetPhysicalAvailableMemoryInMiB()
            {
                PerformanceInformation pi = new PerformanceInformation();
                if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                {
                    return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
                }
                else
                {
                    return -1;
                }

            }

            public static Int64 GetTotalMemoryInMiB()
            {
                PerformanceInformation pi = new PerformanceInformation();
                if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
                {
                    return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
                }
                else
                {
                    return -1;
                }

            }



        }
    }
    class MyClass
    {
        static void Main1(string[] args)
        {
            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();

            double res;

            foreach (ManagementObject result in results)
            {
                res = Convert.ToDouble(result["TotalVisibleMemorySize"]);
                double fres = Math.Round((res / (1024 * 1024)), 2);
                Console.WriteLine("Total usable memory size: " + fres + "GB");
                Console.WriteLine("Total usable memory size: " + res + "KB");
            }

            Console.ReadLine();
            DisplayTotalRam();
            Console.ReadLine();

        }
        private static void DisplayTotalRam()
        {
            string Query = "SELECT MaxCapacity FROM Win32_PhysicalMemoryArray";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query);
            foreach (ManagementObject WniPART in searcher.Get())
            {
                UInt32 SizeinKB = Convert.ToUInt32(WniPART.Properties["MaxCapacity"].Value);
                UInt32 SizeinMB = SizeinKB / 1024;
                UInt32 SizeinGB = SizeinMB / 1024;
                Console.WriteLine("Size in KB: {0}, Size in MB: {1}, Size in GB: {2}", SizeinKB, SizeinMB, SizeinGB);
            }
        }
    }
}
