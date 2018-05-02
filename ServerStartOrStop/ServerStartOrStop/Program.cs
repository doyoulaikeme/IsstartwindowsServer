// =========================================================
// 　　 CRL版本：4.0.30319.239
// 　  创建时间：2017-07-07 10:33
//      修改人：WH09 （Xiao Peng）
//    修改时间：2017-07-07 10:45
// =========================================================
// Copyright © 2015 Beijing BKC Technology CO.,LTD
// All rights reserved.
// =========================================================

using System;
using System.Windows.Forms;

namespace ServerStartOrStop
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}