/*
 * 
 * 
 * 
 * ===============================================================================
 * Copyright © 2015 Beijing BKC Technology CO.,LTD
 * All rights reserved.
 * ==============================================================================
 */

using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace ServerStartOrStop
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An emain helper. </summary>
    ///
    /// <remarks>   xiaop, 2014/12/10. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class EmailHelper
    {
        // //常用的邮箱服务器（SMTP、POP3）地址、端口
        //
        //smtp.163.com
        //        bool reVal = SendMail("smtp.163.com", 25, "", "", "发送邮箱地址", TextBox2.Text, TextBox3.Text, "");
        //参数说明
        /*
         * strSmtpServer:指定发送邮件服务器 
         * iSmtpPort:发送邮件服务器端口
         * Password:发送邮件地址的密码
         * strFrom:发送邮件地址
         * strto:收件地址
         * strSubject:邮件标题 
         * strBody:邮件内容
         */

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  参数说明 </summary>
        ///
        /// <remarks>   xiaop, 2014/12/12. </remarks>
        ///
        /// <param name="strSmtpServer">    指定发送邮件服务器 </param>
        /// <param name="iSmtpPort">        发送邮件服务器端口 </param>
        /// <param name="displayName">      发送者姓名 </param>
        /// <param name="password">         发送邮件地址的密码 </param>
        /// <param name="strFrom">          发送邮件地址 </param>
        /// <param name="strto">            收件地址 </param>
        /// <param name="strSubject">       邮件标题  </param>
        /// <param name="strBody">          邮件内容. </param>
        /// <param name="strFileName">      附件名称 </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SendMail(string strSmtpServer, int iSmtpPort, string displayName, string password, string strFrom, string strto, string strSubject, string strBody, string strFileName)
        {
            //设置发件人信箱,及显示名字 
            MailAddress mailFrom = new MailAddress(strFrom, displayName);
            //设置收件人信箱,及显示名字 
            MailAddress mailTo = new MailAddress(strto);
            //创建一个MailMessage对象 
            MailMessage oMail = new MailMessage(mailFrom, mailTo);
            try
            {
                oMail.Subject = strSubject; //邮件标题 
                oMail.Body = strBody; //邮件内容 
                oMail.IsBodyHtml = true; //指定邮件格式,支持HTML格式 
                oMail.BodyEncoding = Encoding.GetEncoding("GB2312");//邮件采用的编码 
                oMail.SubjectEncoding = Encoding.GetEncoding("GB2312");//邮件采用的编码 
                oMail.Priority = MailPriority.High;//设置邮件的优先级为高 
                //添加附件
                //System.Web.Mail.MailAttachment mailAttachment=new System.Web.Mail.MailAttachment(@"f:/baihe.txt"); 
                if (!string.IsNullOrEmpty(strFileName))
                {
                    Attachment data = new Attachment(strFileName);
                    oMail.Attachments.Add(data);
                }
                //发送邮件服务器 
                SmtpClient client = new SmtpClient();
                //发送邮件服务器的smtp
                //每种邮箱都不一致
                client.Host = strSmtpServer; //指定邮件服务器 
                //发送邮件服务器端口
                client.Port = iSmtpPort;
                //设置超时时间 
                client.Timeout = 9999;
                //设置为发送认证消息
                client.UseDefaultCredentials = true;
                //指定服务器邮件,及密码 
                //发邮件人的邮箱地址和密码
                client.Credentials = new NetworkCredential(strFrom, password);
                client.Send(oMail); //发送邮件 
                //释放资源
                mailFrom = null;
                mailTo = null;
                client.Dispose();//释放资源
                oMail.Dispose(); //释放资源 
                return true;
            }
            catch
            {
                //释放资源
                mailFrom = null;
                mailTo = null;
                oMail.Dispose(); //释放资源 
                return false;
            }
        }

        /// <summary>
        /// 采用企业邮定时发邮件
        /// </summary>
        /// <param name="emailTitle"></param>
        /// <param name="emailMsg"></param>
        /// <param name="sendUser"></param>
        /// <returns></returns>
        public bool SendSysMails(string emailTitle, string emailMsg,string sendUser)
        {
            return SendMail("smtp.exmail.qq.com", 25, "系统管理员", Decode("2LvULoS/J8I="), "xm@bkctech.com", sendUser, emailTitle, emailMsg, "");
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decode(string data)
        {
            string KEY_64 = "THATISTR";
            string IV_64 = "thatistr";
            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(IV_64);
            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {

                return null;
            }
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cs = new CryptoStream(ms, provider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encode(string data)
        {
            string KEY_64 = "THATISTR";
            string IV_64 = "thatistr";
            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(IV_64);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            int i = provider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, provider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }
    }
}
