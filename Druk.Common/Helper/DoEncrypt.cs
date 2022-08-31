using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Druk.Common
{
    /// <summary>
    /// 加解密
    /// </summary>
    public class DoEncrypt
    {

        #region //MD5

        /// <summary>
        /// md5签名算法
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        public static string MD5Bysign(string sign)
        {
            sign = sign.Replace(" ", "").ToLower();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(sign);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str.ToLower();
        }

        /// <summary>
        /// 32位 小写 MD5加密
        /// </summary>
        public static string MD5(string Str, bool isUpper = false)
        {
            var bytes = new MD5CryptoServiceProvider().ComputeHash(Str.ToBytes(Encoding.UTF8));
            string ret = "";
            foreach (byte bb in bytes) { ret += Convert.ToString(bb, 16).PadLeft(2, '0'); }
            var result = ret.PadLeft(32, '0');
            return isUpper ? result.ToUpper() : result.ToLower();
        }

        /// <summary>
        /// 16位 小写 MD5加密
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static string MD5_16(string Str, bool isUpper = false)
        {
            var result = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Str.ToBytes()), 4, 8).Replace("-", "");
            return isUpper ? result.ToUpper() : result.ToLower();
        }
        #endregion

        #region //DES

        #region ======== DES加密========

        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Encrypt(string Text, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray;
            inputByteArray = Encoding.Default.GetBytes(Text);
            des.Key = MD5(sKey).ToUpper().Substring(0, 8).ToBytes();
            des.IV = des.Key;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();
        }

        #endregion

        #region ======== DES解密========

        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="Text"></param> 
        /// <param name="sKey"></param> 
        /// <returns></returns> 
        public static string Decrypt(string Text, string sKey)
        {
            if (string.IsNullOrEmpty(Text)) return string.Empty;
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            int len;
            len = Text.Length / 2;
            byte[] inputByteArray = new byte[len];
            int x, i;
            for (x = 0; x < len; x++)
            {
                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = MD5(sKey).ToUpper().Substring(0, 8).ToBytes();
            des.IV = des.Key;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());
        }
        #endregion
        #endregion

        #region //3DES

        #region =========3DES加密==========

        /// <summary>
        /// 3DES加密
        /// </summary>
        /// <param name="data">要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>加密后字符串</returns>
        public static string Encrypt_DES3(string data, string key, string iv)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = Encoding.GetEncoding("GB2312").GetBytes(key);
            DES.IV = Encoding.GetEncoding("GB2312").GetBytes(iv);
            byte[] Buffer = Encoding.GetEncoding("GB2312").GetBytes(data);
            DES.Mode = CipherMode.CBC;
            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform DESEncrypt = DES.CreateEncryptor();
            return Convert.ToBase64String(DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
        }
        #endregion

        #region =========3DES解密==========
        /// <summary>
        /// 3DES解密
        /// </summary>
        /// <param name="data">要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>解密后字符串</returns>
        public static string Decrypt_DES3(string data, string key, string iv)
        {
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = Encoding.GetEncoding("GB2312").GetBytes(key);
            DES.IV = Encoding.GetEncoding("GB2312").GetBytes(iv);
            DES.Mode = CipherMode.CBC;
            DES.Padding = PaddingMode.PKCS7;
            string result = "";
            try
            {
                byte[] Buffer = Convert.FromBase64String(data);
                ICryptoTransform DESDecrypt = DES.CreateDecryptor();
                result = Encoding.GetEncoding("GB2312").GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
            }
            catch (Exception) { }
            return result;
        }
        #endregion

        #endregion

        #region //AES

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">要加密的字符串</param>
        /// <param name="key">盐值，字符串 长度可选为16位，24位，32位</param>
        /// <param name="iv">加密过程中的偏移量，增加密文的安全,16位字符串</param>
        /// <returns>加密后的密文</returns>
        public static string AesEncrypt(string value, string key, string iv = "")
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (key == null) throw new Exception("未将对象引用设置到对象的实例。");
            if (key.Length < 16) throw new Exception("指定的密钥长度不能少于16位。");
            if (key.Length > 32) throw new Exception("指定的密钥长度不能多于32位。");
            if (key.Length != 16 && key.Length != 24 && key.Length != 32) throw new Exception("指定的密钥长度不明确。");
            if (!string.IsNullOrEmpty(iv))
            {
                if (iv.Length < 16) throw new Exception("指定的向量长度不能少于16位。");
            }

            var _keyByte = Encoding.UTF8.GetBytes(key);
            var _valueByte = Encoding.UTF8.GetBytes(value);
            using (var aes = new RijndaelManaged())
            {
                aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16));
                aes.Key = _keyByte;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var cryptoTransform = aes.CreateEncryptor();
                var resultArray = cryptoTransform.TransformFinalBlock(_valueByte, 0, _valueByte.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">要解密的字符串</param>
        /// <param name="key">盐值，字符串 长度可选为16位，24位，32位</param>
        /// <param name="iv">加密过程中的偏移量，增加密文的安全,16位字符串</param>
        /// <returns>解密后的明文</returns>
        public static string AesDecrypt(string value, string key, string iv = "")
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (key == null) throw new Exception("未将对象引用设置到对象的实例。");
            if (key.Length < 16) throw new Exception("指定的密钥长度不能少于16位。");
            if (key.Length > 32) throw new Exception("指定的密钥长度不能多于32位。");
            if (key.Length != 16 && key.Length != 24 && key.Length != 32) throw new Exception("指定的密钥长度不明确。");
            if (!string.IsNullOrEmpty(iv))
            {
                if (iv.Length < 16) throw new Exception("指定的向量长度不能少于16位。");
            }

            var _keyByte = Encoding.UTF8.GetBytes(key);
            var _valueByte = Convert.FromBase64String(value);
            using (var aes = new RijndaelManaged())
            {
                aes.IV = !string.IsNullOrEmpty(iv) ? Encoding.UTF8.GetBytes(iv) : Encoding.UTF8.GetBytes(key.Substring(0, 16));
                aes.Key = _keyByte;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var cryptoTransform = aes.CreateDecryptor();
                var resultArray = cryptoTransform.TransformFinalBlock(_valueByte, 0, _valueByte.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }
        #endregion

        #region 与Java一样的加密解密

        /// <summary>
        /// Java一样的加密
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="encryptKey">只能是8位字符的key</param>
        /// <returns></returns>
        public static string Encrypt_Java(string encryptString, string encryptKey)
        {
            if (string.IsNullOrEmpty(encryptString))
                return "";
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, new byte[8]), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();

            return Convert.ToBase64String(mStream.ToArray());
        }
        /// <summary>
        /// Java一样的解密
        /// </summary>
        /// <param name="decryptString">要解密的字符串</param>
        /// <param name="decryptKey">只能是8位字符的key</param>
        /// <returns></returns>
        public static string Decrypt_Java(string decryptString, string decryptKey)
        {
            if (string.IsNullOrEmpty(decryptString)) return string.Empty;
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, new byte[8]), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region 与PHP一样的加密解密
        //加解密密钥  
        //初始化向量  
        private static byte[] DESIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// php 语言的加密方法
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt_PHP(string pToEncrypt, string sKey)
        {
            if (string.IsNullOrEmpty(pToEncrypt))
                return "";
            //  pToEncrypt = HttpContext.Current.Server.UrlEncode(pToEncrypt);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.GetEncoding("UTF-8").GetBytes(pToEncrypt);

            //建立加密对象的密钥和偏移量      
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法      
            //使得输入密码必须输入英文文本      
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }


        /// <summary>
        /// php 语言的解密方法
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt_PHP(string pToDecrypt, string sKey)
        {
            if (string.IsNullOrEmpty(pToDecrypt))
                return "";


            //    HttpContext.Current.Response.Write(pToDecrypt + "<br>" + sKey);     
            //    HttpContext.Current.Response.End();     
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            return System.Text.Encoding.UTF8.GetString(ms.ToArray());// HttpContext.Current.Server.UrlDecode(System.Text.Encoding.Default.GetString(ms.ToArray()));
        }

        #endregion

        #region RSA(包含分段加解密)
        public class RSAHelper
        {
            private readonly RSA _privateKeyRsaProvider;
            private readonly RSA _publicKeyRsaProvider;
            private readonly HashAlgorithmName _hashAlgorithmName;
            private readonly Encoding _encoding;

            /// <summary>
            /// 实例化RSAHelper
            /// </summary>
            /// <param name="rsaType">加密算法类型 RSA SHA1;RSA2 SHA256 密钥长度至少为2048</param>
            /// <param name="encoding">编码类型</param>
            /// <param name="privateKey">私钥</param>
            /// <param name="publicKey">公钥</param>
            public RSAHelper(RSAType rsaType, Encoding encoding, string privateKey, string publicKey = null)
            {
                _encoding = encoding;
                if (!string.IsNullOrEmpty(privateKey))
                {
                    _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKey);
                }

                if (!string.IsNullOrEmpty(publicKey))
                {
                    _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
                }

                _hashAlgorithmName = rsaType == RSAType.RSA ? HashAlgorithmName.SHA1 : HashAlgorithmName.SHA256;
            }

            #region 使用私钥签名

            /// <summary>
            /// 使用私钥签名
            /// </summary>
            /// <param name="data">原始数据</param>
            /// <returns></returns>
            public string Sign(string data)
            {
                byte[] dataBytes = _encoding.GetBytes(data);

                var signatureBytes = _privateKeyRsaProvider.SignData(dataBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

                return Convert.ToBase64String(signatureBytes);
            }

            #endregion

            #region 使用公钥验证签名

            /// <summary>
            /// 使用公钥验证签名
            /// </summary>
            /// <param name="data">原始数据</param>
            /// <param name="sign">签名</param>
            /// <returns></returns>
            public bool Verify(string data, string sign)
            {
                byte[] dataBytes = _encoding.GetBytes(data);
                byte[] signBytes = Convert.FromBase64String(sign);

                var verify = _publicKeyRsaProvider.VerifyData(dataBytes, signBytes, _hashAlgorithmName, RSASignaturePadding.Pkcs1);

                return verify;
            }

            #endregion

            #region 解密

            public string Decrypt(string cipherText)
            {
                if (_privateKeyRsaProvider == null)
                {
                    throw new Exception("_privateKeyRsaProvider is null");
                }
                int bufferSize = (_privateKeyRsaProvider.KeySize) / 8;
                var inputBytes = Convert.FromBase64String(cipherText);
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream())
                {
                    while (true)
                    {
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }
                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var rawBytes = _privateKeyRsaProvider.Decrypt(temp, RSAEncryptionPadding.Pkcs1);
                        outputStream.Write(rawBytes, 0, rawBytes.Length);
                    }
                    return Encoding.UTF8.GetString(outputStream.ToArray());
                }
            }

            #endregion

            #region 加密

            public string Encrypt(string text)
            {
                if (_publicKeyRsaProvider == null)
                {
                    throw new Exception("_publicKeyRsaProvider is null");
                }
                using (var rsaProvider = new RSACryptoServiceProvider())
                {
                    var inputBytes = Encoding.UTF8.GetBytes(text);//有含义的字符串转化为字节流
                    int bufferSize = (rsaProvider.KeySize / 8) - 11;//单块最大长度
                    var buffer = new byte[bufferSize];
                    using (MemoryStream inputStream = new MemoryStream(inputBytes),
                         outputStream = new MemoryStream())
                    {
                        while (true)
                        { //分段加密
                            int readSize = inputStream.Read(buffer, 0, bufferSize);
                            if (readSize <= 0)
                            {
                                break;
                            }
                            var temp = new byte[readSize];
                            Array.Copy(buffer, 0, temp, 0, readSize);
                            var encryptedBytes = _publicKeyRsaProvider.Encrypt(temp, RSAEncryptionPadding.Pkcs1);
                            outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                        }
                        return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
                    }
                }
            }

            #endregion

            #region 使用私钥创建RSA实例

            public RSA CreateRsaProviderFromPrivateKey(string privateKey)
            {
                var privateKeyBits = Convert.FromBase64String(privateKey);

                var rsa = RSA.Create();
                var rsaParameters = new RSAParameters();

                using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
                {
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        throw new Exception("Unexpected value read binr.ReadUInt16()");

                    twobytes = binr.ReadUInt16();
                    if (twobytes != 0x0102)
                        throw new Exception("Unexpected version");

                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        throw new Exception("Unexpected value read binr.ReadByte()");

                    rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
                    rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
                }

                rsa.ImportParameters(rsaParameters);
                return rsa;
            }

            #endregion

            #region 使用公钥创建RSA实例

            public RSA CreateRsaProviderFromPublicKey(string publicKeyString)
            {
                // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
                byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
                byte[] seq = new byte[15];

                var x509Key = Convert.FromBase64String(publicKeyString);

                // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
                using (MemoryStream mem = new MemoryStream(x509Key))
                {
                    using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                    {
                        byte bt = 0;
                        ushort twobytes = 0;

                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8230)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        seq = binr.ReadBytes(15);       //read the Sequence OID
                        if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
                            return null;

                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8203)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        bt = binr.ReadByte();
                        if (bt != 0x00)     //expect null byte next
                            return null;

                        twobytes = binr.ReadUInt16();
                        if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                            binr.ReadByte();    //advance 1 byte
                        else if (twobytes == 0x8230)
                            binr.ReadInt16();   //advance 2 bytes
                        else
                            return null;

                        twobytes = binr.ReadUInt16();
                        byte lowbyte = 0x00;
                        byte highbyte = 0x00;

                        if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                            lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                        else if (twobytes == 0x8202)
                        {
                            highbyte = binr.ReadByte(); //advance 2 bytes
                            lowbyte = binr.ReadByte();
                        }
                        else
                            return null;
                        byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                        int modsize = BitConverter.ToInt32(modint, 0);

                        int firstbyte = binr.PeekChar();
                        if (firstbyte == 0x00)
                        {   //if first byte (highest order) of modulus is zero, don't include it
                            binr.ReadByte();    //skip this null byte
                            modsize -= 1;   //reduce modulus buffer size by 1
                        }

                        byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                        if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                            return null;
                        int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                        byte[] exponent = binr.ReadBytes(expbytes);

                        // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                        var rsa = RSA.Create();
                        RSAParameters rsaKeyInfo = new RSAParameters
                        {
                            Modulus = modulus,
                            Exponent = exponent
                        };
                        rsa.ImportParameters(rsaKeyInfo);

                        return rsa;
                    }

                }
            }

            #endregion

            #region 导入密钥算法

            private int GetIntegerSize(BinaryReader binr)
            {
                byte bt = 0;
                int count = 0;
                bt = binr.ReadByte();
                if (bt != 0x02)
                    return 0;
                bt = binr.ReadByte();

                if (bt == 0x81)
                    count = binr.ReadByte();
                else
                if (bt == 0x82)
                {
                    var highbyte = binr.ReadByte();
                    var lowbyte = binr.ReadByte();
                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                    count = BitConverter.ToInt32(modint, 0);
                }
                else
                {
                    count = bt;
                }

                while (binr.ReadByte() == 0x00)
                {
                    count -= 1;
                }
                binr.BaseStream.Seek(-1, SeekOrigin.Current);
                return count;
            }

            private bool CompareBytearrays(byte[] a, byte[] b)
            {
                if (a.Length != b.Length)
                    return false;
                int i = 0;
                foreach (byte c in a)
                {
                    if (c != b[i])
                        return false;
                    i++;
                }
                return true;
            }

            #endregion

        }

        /// <summary>
        /// RSA算法类型
        /// </summary>
        public enum RSAType
        {
            /// <summary>
            /// SHA1
            /// </summary>
            RSA = 0,
            /// <summary>
            /// RSA2 密钥长度至少为2048
            /// SHA256
            /// </summary>
            RSA2
        }
        #endregion
    }
}
