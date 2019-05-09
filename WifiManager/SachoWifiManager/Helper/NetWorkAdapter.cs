using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SachoWifiManager.Helper
{
    public class NetWorkAdapter
    {

        #region Private

        static List<NetworkInterface> netList = NetworkInterface.GetAllNetworkInterfaces().ToList();

        /// <summary>
        /// 获取网卡列表
        /// </summary>
        static List<ManagementObject> NetWorkList()
        {
            string manage = "SELECT * FROM Win32_NetworkAdapterConfiguration";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(manage);
            ManagementObjectCollection collection = searcher.Get();
            List<ManagementObject> netWorkList = new List<ManagementObject>();
            foreach (ManagementObject obj in collection)
            {
                netWorkList.Add(obj);
            }
            return netWorkList;
            //var list = NetworkInterface.GetAllNetworkInterfaces().ToList();
            //return list;
        }

        /// <summary>
        /// 检测是否为无线网卡
        /// </summary>
        /// <returns></returns>
        static bool CheckIsNetWorkAdapterIsWireless(ManagementObject mo)
        {
            string fRegistryKey = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + mo["SettingID"] + "\\Connection";
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
            if (rk != null)
            {
                int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                if (fMediaSubType == 2)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 检测该网卡是否启用
        /// </summary>
        /// <returns></returns>
        static bool CheckIsNetWorkAdapterIsOnline(ManagementObject mo)
        {
            return netList.Exists(t => t.Id.Equals(mo["SettingID"]?.ToString()));
        }

        /// <summary>
        /// 设置网卡启用与否
        /// </summary>5
        /// <param name="netWorkName">网卡名</param>
        /// <returns></returns>
        static void SetNetWorkAdapter(ManagementObject network, bool state)
        {
            try
            {
                //wmic path win32_networkadapter where index = 11 call disable
                string cmd = ($"wmic path win32_networkadapter where index={network["Index"]} call ");
                cmd += state ? "enable" : "disable";
                RunCMD(cmd, out string strOutput);
            }
            catch (Exception ex)
            {

            }
        }



        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        static void RunCMD(string cmd, out string strOuput)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(cmd + "&exit");
            p.StandardInput.AutoFlush = true;
            strOuput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
        }

        #endregion

        #region Public
        /// <summary>
        /// 启用禁用网卡
        /// </summary>
        /// <param name="isEnabled"></param>
        public static void SetNetWorkAdapterEnabeld(ManagementObject adt, bool isEnabled)
        {
            if (adt != null)
            {
                SetNetWorkAdapter(adt, isEnabled);
            }
        }

        /// <summary>
        /// 获取无线网卡
        /// </summary>
        /// <returns></returns>
        public static ManagementObject GetFirstWirelessAdapter()
        {
            return NetWorkAdapter.NetWorkList()?.FirstOrDefault
                 ((t) => NetWorkAdapter.CheckIsNetWorkAdapterIsWireless(t));
        }

        /// <summary>
        /// 获取启用的无线网卡
        /// </summary>
        /// <returns></returns>
        public static ManagementObject GetFirstEnabledWirelessAdapter()
        {
            return NetWorkAdapter.NetWorkList()?.FirstOrDefault
                 ((t) => NetWorkAdapter.CheckIsNetWorkAdapterIsWireless(t)
                 && NetWorkAdapter.CheckIsNetWorkAdapterIsOnline(t));
        }
        #endregion

    }
}
