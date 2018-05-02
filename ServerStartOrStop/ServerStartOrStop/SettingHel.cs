// =========================================================
// 　　 CRL版本：4.0.30319.239
// 　  创建时间：2017-07-07 10:44
//      修改人：WH09 （Xiao Peng）
//    修改时间：2017-07-07 10:44
// =========================================================
// Copyright © 2015 Beijing BKC Technology CO.,LTD
// All rights reserved.
// =========================================================

using System;

using Microsoft.Win32;

namespace ServerStartOrStop
{
    public static class SettingHel
    {
        /// <summary>
        /// 设置应用程序开机自动运行
        /// </summary>
        /// <param name="fileName">应用程序的文件名</param>
        /// <param name="isAutoRun">是否自动运行,为false时，取消自动运行</param>
        /// <exception cref="Exception">设置不成功时抛出异常</exception>
        /// <returns>返回1成功，非1不成功</returns>
        public static String SetAutoRun(string fileName, bool isAutoRun)
        {
            string reSet = string.Empty;
            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(fileName))
                {
                    reSet = "该文件不存在!";
                }
                string name = fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                {
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                }
                if (isAutoRun)
                {
                    reg.SetValue(name, fileName);
                    reSet = "1";
                }
                else
                {
                    reg.SetValue(name, false);
                }
            }
            catch (Exception ex)
            {
                //“记录异常”
            }
            finally
            {
                if (reg != null)
                {
                    reg.Close();
                }
            }
            return reSet;
        } 
    }
}