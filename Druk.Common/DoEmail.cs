using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Druk.Common
{
    /// <summary>
    /// 邮件操作
    /// </summary>
    public class DoEmail
    {

        const string SnedSmtpServer = "smtp.exmail.qq.com";
        const string SnedSmtpAccount = "syssend@inexten.com";
        const string SnedSmtpPassword = "Happy@2022";
        const string SnedFromEmail = "syssend@inexten.com";

        /// <summary>
        /// 开发人员邮箱
        /// </summary>
        public static string developmentUser = "";
        /// <summary>
        /// 致命错误邮箱
        /// </summary>
        public static string fatalErrorToUser = "";

        /// <summary>
        /// 报警邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="toAddress">收件人</param>
        /// <param name="fromName">发件人名称</param>
        /// <param name="mailPriority"></param>
        public static void AlarmEmal(string title, string content, string toAddress = "", string fromName = "SaaS", MailPriority mailPriority = MailPriority.Normal)
        {
            Task.Run(() =>
            {
                try
                {
                    var sendREsult = "";
                    string addressee = toAddress;

                    SendDirect(SnedSmtpServer, SnedSmtpAccount, SnedSmtpPassword, SnedFromEmail, fromName, GetRecipientsByMailPriority(toAddress, mailPriority), title, content, mailPriority, ref sendREsult);
                }
                catch
                {
                }
            });
        }



        /// <summary>
        /// 内部错误邮件
        /// </summary>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="fromName">发件人名称</param>
        ///  <param name="mailPriority">发件人名称</param>
        public static void ErrorEmail(string title, string content, string fromName = "蜂高商城", MailPriority mailPriority = MailPriority.Normal)
        {
            Task.Run(() =>
            {
                try
                {
                    var sendREsult = "";
                    SendDirect(SnedSmtpServer, SnedSmtpAccount, SnedSmtpPassword, SnedFromEmail, fromName, GetRecipientsByMailPriority(developmentUser, mailPriority), title, content, mailPriority, ref sendREsult);
                }
                catch
                {
                }
            });
        }

        //根据紧急程度决定收件人
        private static string GetRecipientsByMailPriority(string toAddress, MailPriority mailPriority)
        {
            string recipients = toAddress;
            return recipients;
            //switch (mailPriority)
            //{
            //    case MailPriority.Low:
            //        recipients = toAddress;
            //        break;
            //    case MailPriority.Normal:
            //        if (!toAddress.Contains("774771628"))
            //            recipients = $"{toAddress},{developmentUser}";
            //        break;
            //    case MailPriority.High:
            //        recipients = $"{toAddress},{fatalErrorToUser}";
            //        break;
            //}
            //return recipients;
        }

        public static bool SendDirect(string SmtpServer, string SmtpAccount, string SmtpPassword, string FromEmail, string FromEmailName, string EmailAddress, string mailTitle, string mailBody, MailPriority mailPriority, ref string SendResult)
        {
            return SendDirect(SmtpServer, SmtpAccount, SmtpPassword, FromEmail, FromEmailName, EmailAddress, mailTitle, mailBody, null, ref SendResult, mailPriority);
        }



        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="SmtpServer"></param>
        /// <param name="SmtpAccount"></param>
        /// <param name="SmtpPassword"></param>
        /// <param name="FromEmail"></param>
        /// <param name="FromEmailName"></param>
        /// <param name="EmailAddress"></param>
        /// <param name="mailTitle"></param>
        /// <param name="mailBody"></param>
        /// <param name="attachmentFilePath">附件，（key为附件名称，value为附件绝对路径）</param>
        /// <param name="SendResult"></param>
        /// <param name="mailPriority"></param>
        /// <returns></returns>
        public static bool SendDirect(string SmtpServer, string SmtpAccount, string SmtpPassword, string FromEmail, string FromEmailName, string EmailAddress, string mailTitle, string mailBody, Dictionary<string, string> attachmentFilePath, ref string SendResult, MailPriority mailPriority = MailPriority.Normal)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(FromEmail, FromEmailName),
                Subject = mailTitle
            };

            foreach (var email in EmailAddress.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) { if (IsValidEmail(email)) mail.To.Add(email); }


            if (mail.To.Count > 0)
            {
                mail.Priority = MailPriority.High;
                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(mailBody, Encoding.UTF8, MediaTypeNames.Text.Html);
                mail.AlternateViews.Add(avHtml);
                mail.IsBodyHtml = true;
                mail.Priority = mailPriority;
                if (attachmentFilePath != null && attachmentFilePath.Count > 0)
                {
                    foreach (var item in attachmentFilePath)
                    {
                        System.Net.Mail.Attachment attachment = AttachmentHelper.CreateAttachment(item.Value, item.Key, TransferEncoding.Base64);
                        mail.Attachments.Add(attachment);
                    }


                }

                try
                {
                    SmtpClient client = null;

                    client = new SmtpClient(SmtpServer);
                    client.EnableSsl = true;
                    //client.Port =465;
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    if (SmtpAccount != "")
                    {
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.Credentials = new NetworkCredential(SmtpAccount, SmtpPassword);
                    }
                    client.Send(mail);

                    client.Dispose();
                    SendResult = "发送成功!!";
                    return true;
                }
                catch (Exception ex)
                {
                    SendResult = ex.ToString();
                }
            }
            return false;

        }
        /// <summary>
        /// 检查邮件地址
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        private static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }

    public class AttachmentHelper
    {
        public static System.Net.Mail.Attachment CreateAttachment(string attachmentFile, string displayName, TransferEncoding transferEncoding)
        {
            Console.WriteLine("CreateAttachment attachmentFile" + attachmentFile);
            if (!System.IO.File.Exists(attachmentFile))
            {
                Console.WriteLine($"找不到对应附件:{attachmentFile}");
                throw new Exception($"找不到对应附件:{attachmentFile}");
            }
            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachmentFile);
            attachment.TransferEncoding = transferEncoding;

            string tranferEncodingMarker = String.Empty;
            string encodingMarker = String.Empty;
            int maxChunkLength = 0;

            switch (transferEncoding)
            {
                case TransferEncoding.Base64:
                    tranferEncodingMarker = "B";
                    encodingMarker = "UTF-8";
                    maxChunkLength = 30;
                    break;
                case TransferEncoding.QuotedPrintable:
                    tranferEncodingMarker = "Q";
                    encodingMarker = "ISO-8859-1";
                    maxChunkLength = 76;
                    break;
                default:
                    throw (new ArgumentException(String.Format("The specified TransferEncoding is not supported: {0}", transferEncoding, "transferEncoding")));
            }

            attachment.NameEncoding = Encoding.GetEncoding(encodingMarker);

            string encodingtoken = String.Format("=?{0}?{1}?", encodingMarker, tranferEncodingMarker);
            string softbreak = "?=";
            string encodedAttachmentName = encodingtoken;

            if (attachment.TransferEncoding == TransferEncoding.QuotedPrintable)
                encodedAttachmentName = HttpUtility.UrlEncode(displayName, Encoding.Default).Replace("+", " ").Replace("%", "=");
            else
                encodedAttachmentName = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(displayName));

            encodedAttachmentName = SplitEncodedAttachmentName(encodingtoken, softbreak, maxChunkLength, encodedAttachmentName);
            attachment.Name = encodedAttachmentName;

            return attachment;
        }

        private static string SplitEncodedAttachmentName(string encodingtoken, string softbreak, int maxChunkLength, string encoded)
        {
            int splitLength = maxChunkLength - encodingtoken.Length - (softbreak.Length * 2);
            var parts = SplitByLength(encoded, splitLength);

            string encodedAttachmentName = encodingtoken;

            foreach (var part in parts)
                encodedAttachmentName += part + softbreak + encodingtoken;

            encodedAttachmentName = encodedAttachmentName.Remove(encodedAttachmentName.Length - encodingtoken.Length, encodingtoken.Length);
            return encodedAttachmentName;
        }

        private static IEnumerable<string> SplitByLength(string stringToSplit, int length)
        {
            while (stringToSplit.Length > length)
            {
                yield return stringToSplit.Substring(0, length);
                stringToSplit = stringToSplit.Substring(length);
            }

            if (stringToSplit.Length > 0) yield return stringToSplit;
        }
    }
}
