using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Druk.WeChat
{
    /// <summary>
    /// 微信加解密类
    /// </summary>
    public class WX_DoEncrypt
    {

        /// <summary>
        /// 解密手机号
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string DecryptPhoneNumber(string sessionId, string encryptedData, string iv)
        {

            try
            {
                var phoneNumber = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecodeEncryptedData(sessionId, encryptedData, iv);

                return phoneNumber;
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        #region //解密 DecryptMsg

        /// <summary>
        /// 解密  对微信发送来的字符串进行验签 并解密为明文供使用
        /// </summary>
        /// <param name="Msg_Signature">签名串，对应URL参数的msg_signature</param>
        /// <param name="TimeStamp">时间戳，对应URL参数的timestamp</param>
        /// <param name="Nonce">随机串，对应URL参数的nonce</param>
        /// <param name="PostData">密文，对应POST请求的数据</param>
        /// <param name="Msg">解密后的原文，当return返回0时有效</param>
        /// <returns> 成功0，失败返回对应的错误码</returns>
        public static int DecryptMsg(string AppID, string EncodingAESKey, string Token, string TimeStamp, string Nonce, string PostData, string Msg_Signature, ref string Msg)
        {
            #region //数据准备与格式验证
            if (EncodingAESKey.Length != 43) { return (int)WXBizMsgCryptErrorCode.AESKey非法; }
            XmlDocument doc = new XmlDocument();
            XmlNode root;
            string sEncryptMsg;
            try
            {
                doc.LoadXml(PostData);
                root = doc.FirstChild;
                sEncryptMsg = root["Encrypt"].InnerText;
            }
            catch { return (int)WXBizMsgCryptErrorCode.xml解析失败; }
            #endregion

            int result = 0;
            //验证签名是否合法
            result = CheckSign(Token, TimeStamp, Nonce, sEncryptMsg, Msg_Signature);
            if (result == 0)
            {
                string cpid = "";
                try
                {
                    ///调用解密
                    Msg = AES_decrypt(sEncryptMsg, EncodingAESKey, ref cpid);
                }
                catch (FormatException) { return (int)WXBizMsgCryptErrorCode.base64解密异常; }
                catch (Exception) { return (int)WXBizMsgCryptErrorCode.AES解密失败; }
                if (cpid != AppID)
                    return (int)WXBizMsgCryptErrorCode.appid校验错误;
            }
            return result;
        }


        #region //解密 通过String

        /// <summary>
        /// 对字符串进行解密
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="EncodingAESKey"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        static string AES_decrypt(String Input, string EncodingAESKey, ref string appid)
        {
            byte[] Key;
            Key = Convert.FromBase64String(EncodingAESKey + "=");
            byte[] Iv = new byte[16];
            Array.Copy(Key, Iv, 16);
            byte[] btmpMsg = AES_decrypt(Input, Iv, Key);

            int len = BitConverter.ToInt32(btmpMsg, 16);
            len = IPAddress.NetworkToHostOrder(len);


            byte[] bMsg = new byte[len];
            byte[] bAppid = new byte[btmpMsg.Length - 20 - len];
            Array.Copy(btmpMsg, 20, bMsg, 0, len);
            Array.Copy(btmpMsg, 20 + len, bAppid, 0, btmpMsg.Length - 20 - len);
            string oriMsg = Encoding.UTF8.GetString(bMsg);
            appid = Encoding.UTF8.GetString(bAppid);

            return oriMsg;
        }
        #endregion

        #region //AES解密 通过Iv Key

        static byte[] AES_decrypt(String Input, byte[] Iv, byte[] Key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            aes.Key = Key;
            aes.IV = Iv;
            var decrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    byte[] msg = new byte[xXml.Length + 32 - xXml.Length % 32];
                    Array.Copy(xXml, msg, xXml.Length);
                    cs.Write(xXml, 0, xXml.Length);
                }
                xBuff = decode2(ms.ToArray());
            }
            return xBuff;
        }

        static byte[] decode2(byte[] decrypted)
        {
            int pad = (int)decrypted[decrypted.Length - 1];
            if (pad < 1 || pad > 32)
            {
                pad = 0;
            }
            byte[] res = new byte[decrypted.Length - pad];
            Array.Copy(decrypted, 0, res, 0, decrypted.Length - pad);
            return res;
        }
        #endregion
        #endregion

        #region //加密 EncryptMsg

        /// <summary>
        /// 将企业号回复用户的消息加密打包
        /// </summary>
        /// <param name="ReplyMsg">企业号待回复用户的消息,xml格式的字符串</param>
        /// <param name="TimeStamp">时间戳，可以自己生成，也可以用请求过来的URL参数的timestamp</param>
        /// <param name="Nonce">随机串，可以自己生成，也可以用请求过来的URL参数的nonce</param>
        /// <param name="EncryptMsg">加密后的可以直接回复用户的密文，包括msg_signature, timestamp, nonce, encrypt的xml格式的字符串,当return返回0时有效</param>
        /// <returns>成功0，失败返回对应的错误码</returns>
        public static int EncryptMsg(string AppID, string EncodingAESKey, string Token, string ReplyMsg, string TimeStamp, string Nonce, ref string EncryptMsg)
        {
            if (EncodingAESKey.Length != 43) { return (int)WXBizMsgCryptErrorCode.AESKey非法; }
            string raw = "";
            try { raw = AES_encrypt(ReplyMsg, EncodingAESKey, AppID); }
            catch (Exception) { return (int)WXBizMsgCryptErrorCode.AES加密失败; }
            string MsgSigature = "";
            int ret = 0;
            //创建Sign验签
            ret = CreateSign(Token, TimeStamp, Nonce, raw, ref MsgSigature);
            if (ret == 0)
            {
                EncryptMsg = string.Format(@"
<xml>
    <Encrypt><![CDATA[{0}]]></Encrypt>
    <MsgSignature><![CDATA[{1}]]></MsgSignature>
    <TimeStamp><![CDATA[{2}]]></TimeStamp>
    <Nonce><![CDATA[{3}]]></Nonce>
</xml>", raw, MsgSigature, TimeStamp, Nonce);
            }
            return ret;
        }

        #region //对字符串进行加密操作

        /// <summary>
        /// 对字符串进行加密操作
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="EncodingAESKey"></param>
        /// <param name="appid"></param>
        /// <returns></returns>
        static String AES_encrypt(String Input, string EncodingAESKey, string appid)
        {
            byte[] Key;
            Key = Convert.FromBase64String(EncodingAESKey + "=");
            byte[] Iv = new byte[16];
            Array.Copy(Key, Iv, 16);
            string Randcode = CreateRandCode(16);
            byte[] bRand = Encoding.UTF8.GetBytes(Randcode);
            byte[] bAppid = Encoding.UTF8.GetBytes(appid);
            byte[] btmpMsg = Encoding.UTF8.GetBytes(Input);
            byte[] bMsgLen = BitConverter.GetBytes(HostToNetworkOrder(btmpMsg.Length));
            byte[] bMsg = new byte[bRand.Length + bMsgLen.Length + bAppid.Length + btmpMsg.Length];

            Array.Copy(bRand, bMsg, bRand.Length);
            Array.Copy(bMsgLen, 0, bMsg, bRand.Length, bMsgLen.Length);
            Array.Copy(btmpMsg, 0, bMsg, bRand.Length + bMsgLen.Length, btmpMsg.Length);
            Array.Copy(bAppid, 0, bMsg, bRand.Length + bMsgLen.Length + btmpMsg.Length, bAppid.Length);

            return AES_encrypt(bMsg, Iv, Key);

        }
        #endregion

        #region //加密方法 使用Iv Key

        /// <summary>
        /// 加密方法 使用Iv Key
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Iv"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        static String AES_encrypt(byte[] Input, byte[] Iv, byte[] Key)
        {
            var aes = new RijndaelManaged();
            //秘钥的大小，以位为单位
            aes.KeySize = 256;
            //支持的块大小
            aes.BlockSize = 128;
            //填充模式
            //aes.Padding = PaddingMode.PKCS7;
            aes.Padding = PaddingMode.None;
            aes.Mode = CipherMode.CBC;
            aes.Key = Key;
            aes.IV = Iv;
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;

            #region 自己进行PKCS7补位，用系统自己带的不行
            byte[] msg = new byte[Input.Length + 32 - Input.Length % 32];
            Array.Copy(Input, msg, Input.Length);
            byte[] pad = KCS7Encoder(Input.Length);
            Array.Copy(pad, 0, msg, Input.Length, pad.Length);
            #endregion

            #region 注释的也是一种方法，效果一样
            //ICryptoTransform transform = aes.CreateEncryptor();
            //byte[] xBuff = transform.TransformFinalBlock(msg, 0, msg.Length);
            #endregion

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    cs.Write(msg, 0, msg.Length);
                }
                xBuff = ms.ToArray();
            }

            String Output = Convert.ToBase64String(xBuff);
            return Output;
        }
        #endregion

        #region //加密工具

        /// <summary>
        /// 获取随机码
        /// </summary>
        /// <param name="codeLen"></param>
        /// <returns></returns>
        static string CreateRandCode(int codeLen)
        {
            string codeSerial = "2,3,4,5,6,7,a,c,d,e,f,h,i,j,k,m,n,p,r,s,t,A,C,D,E,F,G,H,J,K,M,N,P,Q,R,S,U,V,W,X,Y,Z";
            if (codeLen == 0)
            {
                codeLen = 16;
            }
            string[] arr = codeSerial.Split(',');
            string code = "";
            int randValue = -1;
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < codeLen; i++)
            {
                randValue = rand.Next(0, arr.Length - 1);
                code += arr[randValue];
            }
            return code;
        }
        static Int32 HostToNetworkOrder(Int32 inval)
        {
            Int32 outval = 0;
            for (int i = 0; i < 4; i++)
                outval = (outval << 8) + ((inval >> (i * 8)) & 255);
            return outval;
        }
        static byte[] KCS7Encoder(int text_length)
        {
            int block_size = 32;
            // 计算需要填充的位数
            int amount_to_pad = block_size - (text_length % block_size);
            if (amount_to_pad == 0)
            {
                amount_to_pad = block_size;
            }
            // 获得补位所用的字符
            char pad_chr = chr(amount_to_pad);
            string tmp = "";
            for (int index = 0; index < amount_to_pad; index++)
            {
                tmp += pad_chr;
            }
            return Encoding.UTF8.GetBytes(tmp);
        }
        static char chr(int a)
        {

            byte target = (byte)(a & 0xFF);
            return (char)target;
        }
        #endregion

        #endregion

        #region //工具

        #region //状态枚举

        /// <summary>
        /// 状态枚举
        /// </summary>
        enum WXBizMsgCryptErrorCode
        {
            WXBizMsgCrypt_OK = 0,
            签名验证错误 = -40001,
            xml解析失败 = -40002,
            sha加密生成签名失败 = -40003,
            AESKey非法 = -40004,
            appid校验错误 = -40005,
            AES加密失败 = -40006,
            AES解密失败 = -40007,
            解密后得到的buffer非法 = -40008,
            base64加密异常 = -40009,
            base64解密异常 = -40010
        };
        #endregion

        #region //验证签名是否合法
        /// <summary>
        /// 验证签名是否合法
        /// </summary>
        /// <param name="sToken"></param>
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="sMsgEncrypt"></param>
        /// <param name="sSigture"></param>
        /// <returns></returns>
        static int CheckSign(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, string sSigture)
        {
            string hash = "";
            int ret = 0;
            ret = CreateSign(sToken, sTimeStamp, sNonce, sMsgEncrypt, ref hash);
            if (ret != 0)
                return ret;
            //System.Console.WriteLine(hash);
            if (hash == sSigture)
                return 0;
            else
            {
                return (int)WXBizMsgCryptErrorCode.签名验证错误;
            }
        }
        #endregion

        #region //使用参数进行签名创建,用来验证是否相同

        /// <summary>
        /// 使用参数创建签名
        /// </summary>
        /// <param name="sToken"></param>
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="sMsgEncrypt"></param>
        /// <param name="sMsgSignature"></param>
        /// <returns></returns>
        static int CreateSign(string sToken, string sTimeStamp, string sNonce, string sMsgEncrypt, ref string sMsgSignature)
        {
            ArrayList AL = new ArrayList();
            AL.Add(sToken);
            AL.Add(sTimeStamp);
            AL.Add(sNonce);
            AL.Add(sMsgEncrypt);
            AL.Sort(new DictionarySort());
            string raw = "";
            for (int i = 0; i < AL.Count; ++i) { raw += AL[i]; }

            SHA1 sha;
            ASCIIEncoding enc;
            string hash = "";
            try
            {
                sha = SHA1.Create();
                enc = new ASCIIEncoding();
                byte[] dataToHash = enc.GetBytes(raw);
                byte[] dataHashed = sha.ComputeHash(dataToHash);
                hash = BitConverter.ToString(dataHashed).Replace("-", "");
                hash = hash.ToLower();
            }
            catch (Exception)
            {
                return (int)WXBizMsgCryptErrorCode.sha加密生成签名失败;
            }
            sMsgSignature = hash;
            return 0;
        }
        #endregion
        #endregion


        #region //解密AES_decrypt

        /// <summary>
        /// 解密AES_decrypt
        /// </summary>
        /// <param name="encryptedDataStr"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static string AES_decrypt(string encryptedDataStr, string key, string iv)
        {
            try
            {
                RijndaelManaged rijalg = new RijndaelManaged();
                //-----------------      
                //设置 cipher 格式 AES-128-CBC      

                rijalg.KeySize = 128;

                rijalg.Padding = PaddingMode.PKCS7;
                rijalg.Mode = CipherMode.CBC;

                rijalg.Key = Convert.FromBase64String(key);
                rijalg.IV = Convert.FromBase64String(iv);


                byte[] encryptedData = Convert.FromBase64String(encryptedDataStr);
                //解密      
                ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);

                string result;

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        #endregion

        #region //微信小程序数据验签

        /// <summary>
        /// 根据微信小程序平台提供的签名验证算法验证用户发来的数据是否有效
        /// </summary>
        /// <param name="rawData">公开的用户资料</param>
        /// <param name="signature">公开资料携带的签名信息</param>
        /// <param name="sessionKey">从服务端获取的SessionKey</param>
        /// <returns>True：资料有效，False：资料无效</returns>
        public static bool VaildateUserInfo(string rawData, string signature, string sessionKey)
        {
            //创建SHA1签名类
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            //编码用于SHA1验证的源数据
            byte[] source = Encoding.UTF8.GetBytes(rawData + sessionKey);
            //生成签名
            byte[] target = sha1.ComputeHash(source);
            //转化为string类型，注意此处转化后是中间带短横杠的大写字母，需要剔除横杠转小写字母
            string result = BitConverter.ToString(target).Replace("-", "").ToLower();
            //比对，输出验证结果
            return signature == result;
        }

        #endregion
    }

    #region //工具类

    #region //DictionarySort
    class DictionarySort : System.Collections.IComparer
    {

        /// <summary>
        /// 比较对象
        /// </summary>
        /// <param name="oLeft"></param>
        /// <param name="oRight"></param>
        /// <returns></returns>
        public int Compare(object oLeft, object oRight)
        {
            string sLeft = oLeft as string;
            string sRight = oRight as string;
            int iLeftLength = sLeft.Length;
            int iRightLength = sRight.Length;
            int index = 0;
            while (index < iLeftLength && index < iRightLength)
            {
                if (sLeft[index] < sRight[index])
                    return -1;
                else if (sLeft[index] > sRight[index])
                    return 1;
                else
                    index++;
            }
            return iLeftLength - iRightLength;

        }
    }
    #endregion

    #endregion
}
