// =========================================================
// 　　 CRL版本：4.0.30319.239
// 　  创建时间：2017-07-07 10:33
//      修改人：WH09 （Xiao Peng）
//    修改时间：2017-08-01 17:29
// =========================================================
// Copyright © 2015 Beijing BKC Technology CO.,LTD
// All rights reserved.
// =========================================================

using System;
using System.Drawing;
using System.ServiceProcess;
using System.Timers;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;

using log4net;

using ServerStartOrStop.Properties;

namespace ServerStartOrStop
{
    public partial class Form1 : Form
    {
        //实例化Timer类  
        readonly System.Timers.Timer _aTimer = new System.Timers.Timer();
        private  DateTime[] _time = new DateTime[ConfigurationManager.AppSettings["IntervalServerTime"].Split(',').Length];
        private static string[] _serverNames = { };
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Form1()
        {
            LogWrite("该监控程序在" + DateTime.Now + "开始监听服务", null, 1);

            try
            {
                //设置自动启动项
                SettingHel.SetAutoRun(Application.ExecutablePath, true);
                Form1._serverNames = ConfigurationManager.AppSettings["serverNames"].Replace("\r\n", string.Empty)
                                                                                    .Replace(" ", string.Empty).Split(',');
                InitializeComponent();

                AddGroupBox(Form1._serverNames);
            }
            catch (Exception ex)
            {
                LogWrite(ex.Message, ex);
            }

            this.Init(null,null);

            for (int i = 0; i < ConfigurationManager.AppSettings["IntervalServerTime"].Split(',').Length; i++)
            {
                _time[i] = DateTime.Now;
            }

            this._aTimer.Elapsed += this.Init;
            this._aTimer.Interval = int.Parse(ConfigurationManager.AppSettings["IntervalTime"]) * 1000;

            this._aTimer.AutoReset = true; //执行一次 false，一直执行true  

            //是否执行System.Timers.Timer.Elapsed事件  
            this._aTimer.Enabled = true;
        }

        /// <summary>
        /// 查询状态数据
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void Init(object source, ElapsedEventArgs e)
        {
            try
                {
                    for (int i = 0; i < Form1._serverNames.Length; i++)
                    {
                     
                        if (source==null && e==null)
                        {
                            SetController(i);
                        }
                        else
                        {
                            DateTime executeTime = _time[i].AddSeconds(double.Parse(ConfigurationManager.AppSettings["IntervalServerTime"].Split(',')[i]));
                            if (e.SignalTime >= executeTime)
                            {
                                _time[i] =
                                    _time[i].AddSeconds(
                                        double.Parse(ConfigurationManager.AppSettings["IntervalServerTime"].Split(',')[i]));
                                SetController(i);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWrite(ex.Message, ex);
                }
            
        }

        private void SetController(int i)
        {
            ServiceController sc = new ServiceController(Form1._serverNames[i]);
            string lab = Resources.Form1_AddLab_txtName + i;
            string labStatus = Resources.Form1_AddLab_txtSatus + i;
            if (this.FindControl(this, lab) != null)
            {
                Label tb = (Label) this.FindControl(this, lab);
                Label tbStatus = (Label) this.FindControl(this, labStatus);
                Button butt = (Button) this.FindControl(this, Resources.Form1_AddBtt_boxName + i);

                tb.Tag = sc.ServiceName;
                tb.Text = sc.DisplayName;
                tb.ForeColor = Color.White;
                tb.BackColor = Color.Gray;
                tb.TextAlign = ContentAlignment.MiddleCenter;
                SetController(sc, tbStatus, butt);

                try
                {
                    //如果该服务已停止   主动启动
                    if (tbStatus.Text == Resources.Form1_Init_服务未运行
                        && ConfigurationManager.AppSettings["status"] == "1")
                    {
                        sc.Start();

                        //记录日志
                        LogWrite("【" + sc.DisplayName + "】在" + DateTime.Now + "被强制启动", null, 1);
                    }
                }
                catch (Exception ex)
                {
                    EmailHelper email = new EmailHelper();
                    string msg = "【" + sc.DisplayName + "】在" + DateTime.Now + "启动失败;原因：" + ex;
                    email.SendSysMails(sc.DisplayName + "服务启动失败", msg, "liangqiang@bkctech.com");
                    //记录日志
                    LogWrite(msg, null, 1);
                }
           
                
            }
        }

        /// <summary>
        /// 赋值控件
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="tbStatus"></param>
        /// <param name="butt"></param>
        private void SetController(ServiceController sc, Label tbStatus, Button butt)
        {
            switch (sc.Status)
            {
                case ServiceControllerStatus.ContinuePending:
                    tbStatus.Text = Resources.Form1_Init_服务即将继续;
                    tbStatus.BackColor = Color.Red;
                    butt.Hide();
                    break;
                case ServiceControllerStatus.PausePending:
                    tbStatus.Text = Resources.Form1_Init_服务即将暂停;
                    tbStatus.BackColor = Color.Red;
                    butt.Hide();
                    break;
                case ServiceControllerStatus.Paused:
                    tbStatus.Text = Resources.Form1_Init_服务已暂停;
                    tbStatus.BackColor = Color.Red;

                    butt.Show();
                    butt.Text = Resources.Form1_Init_启动服务;
                    break;
                case ServiceControllerStatus.Running:
                    tbStatus.Text = Resources.Form1_Init_服务正在运行;
                    tbStatus.BackColor = Color.Green;
                    butt.Show();
                    butt.Text = Resources.Form1_Init_停止服务;
                    break;
                case ServiceControllerStatus.StartPending:
                    tbStatus.Text = Resources.Form1_Init_服务正在启动;
                    tbStatus.BackColor = Color.Green;

                    butt.Hide();

                    break;
                case ServiceControllerStatus.StopPending:
                    tbStatus.Text = Resources.Form1_Init_服务正在停止;
                    tbStatus.BackColor = Color.Red;
                    butt.Hide();
                    break;
                case ServiceControllerStatus.Stopped:
                    tbStatus.Text = Resources.Form1_Init_服务未运行;
                    tbStatus.BackColor = Color.Red;
                    butt.Show();
                    butt.Text = Resources.Form1_Init_启动服务;
                    break;
            }
            tbStatus.TextAlign = ContentAlignment.MiddleCenter;
            tbStatus.ForeColor = Color.White;
        }

        /// <summary>
        /// 启停开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void button1_Click(object sender, EventArgs e)
        {
            var i = ((Control)sender).Tag;
            string lab = Resources.Form1_AddLab_txtName + i;
            Label tb = (Label)this.FindControl(this, lab);
            ServiceController sc = new ServiceController(tb.Tag.ToString());
            try
            {
                if (((Control)sender).Text == Resources.Form1_Init_启动服务)
                {
          
                    sc.Start();
                    LogWrite("【" + sc.DisplayName + "】在" + DateTime.Now + "被手动启动", null, 1);
                }
                else
                {
                    sc.Stop();
                    LogWrite("【" + sc.DisplayName + "】在" + DateTime.Now + "被手动关闭", null, 1);
                }
            }
            catch (Exception ex)
            {
                EmailHelper email = new EmailHelper();
                string msg = "【" + sc.DisplayName + "】在" + DateTime.Now + "启动失败;原因："+ ex;
                email.SendSysMails(sc.DisplayName + "服务启动失败", msg, "liangqiang@bkctech.com");
                //记录日志
                LogWrite(msg, null, 1);
            }
         

        }

        #region 控件操作

        /// <summary>
        /// 添加控件
        /// </summary>
        /// <param name="serverNames"></param>
        public void AddGroupBox(string[] serverNames)
        {
            for (int i = 0; i < serverNames.Length; i++)
            {
                ServiceController sc = new ServiceController(serverNames[i]);
                GroupBox gbox = new GroupBox();
                gbox.Name = sc.ServiceName;

                //gbox.Text = sc.DisplayName;
                gbox.Width = 500;
                gbox.Height = 50;
                gbox.Location = new Point(50, 53 * i);
                this.Controls.Add(gbox);

                //调用添加文本控件的方法
                AddLab(gbox, i);
                AddBtt(gbox, i);
            }
        }

        /// <summary>
        /// 添加文本控件
        /// </summary>
        /// <param name="gb"></param>
        /// <param name="i"></param>
        public void AddLab(GroupBox gb, int i)
        {
            Label txt = new Label
            {
                Name = Resources.Form1_AddLab_txtName + i,
                Text = Resources.Form1_AddLab_txtName + i,
                Location = new Point(12, 14 + i * 1),
                Width = 200
            };
            gb.Controls.Add(txt);

            txt = new Label
            {
                Name = Resources.Form1_AddLab_txtSatus + i,
                Text = Resources.Form1_AddLab_txtSatus + i,
                Width = 100,
                Location = new Point(250, 14 + i * 1)
            };
            gb.Controls.Add(txt);
        }

        /// <summary>
        /// 添加按钮及按钮事件
        /// </summary>
        /// <param name="gb"></param>
        /// <param name="i"></param>
        public void AddBtt(GroupBox gb, int i)
        {
            Button box = new Button
            {
                Name = Resources.Form1_AddBtt_boxName + i,
                Text = Resources.Form1_AddBtt_boxName + i,
                Tag = i.ToString(),
                Location = new Point(420, 15 + i * 1)
            };
            box.Click += this.button1_Click;
            gb.Controls.Add(box);
        }

        /// <summary>
        /// 在winform中查找控件  
        /// </summary>
        /// <param name="control"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        private Control FindControl(Control control, string controlName)
        {
            Control c1;
            foreach (Control c in control.Controls)
            {
                if (c.Name == controlName)
                {
                    return c;
                }
                else if (c.Controls.Count > 0)
                {
                    c1 = this.FindControl(c, controlName);
                    if (c1 != null)
                    {
                        return c1;
                    }
                }
            }
            return null;
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //窗体关闭原因为单击"关闭"按钮或Alt+F4
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; //取消关闭操作 表现为不关闭窗体
                this.Hide(); //隐藏窗体
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show(); //窗体显示
            this.WindowState = FormWindowState.Normal; //窗体状态默认大小
            this.Activate(); //激活窗体给予焦点
        }

        #region 重写窗体

        private const int CP_NOCLOSE_BUTTON = 0x200;

        /// <summary>
        /// 禁用窗体的关闭按钮
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;

                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;

                return myCp;
            }
        }

        #endregion

        /// <summary>
        /// 在线的日志分类
        /// </summary>
        /// <param name="level">0-Info,1-警告,2-错误</param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public void LogWrite(string message, Exception ex = null, int level = 2)
        {
            switch (level)
            {
                case 0:
                    log.Info(message, ex); //提示
                    break;
                case 1:
                    log.Warn(message, ex); //警告
                    break;
                default:
                    log.Error(message, ex); //错误
                    break;
            }
        }

        //private void frmFileDisposal_Closed(object sender, System.EventArgs e)
        //{
        //    LogWrite("该监控程序在" + DateTime.Now + "关闭监听服务", null, 1);
        //    this.Dispose();
        //    this.Close();
        //}
    }
}