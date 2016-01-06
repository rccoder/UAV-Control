using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//导入MD5模块，AES
using System.Security.Cryptography;
//System.Random模块默认已经导入


namespace MessageModel.utils
{
    public static class Utils
    {
        private static byte[] _key1 = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        public static byte[] int2bytes(int number)
        {
            byte[] num_list = System.BitConverter.GetBytes(number);
            return num_list;
        }
        public static int bytes2int(byte[] bytes_int)
        {
            int number = System.BitConverter.ToInt32(bytes_int, 0);
            return number;
        }
        public static byte[] padding_by_zero(byte[] bytes_array, int length)
        {
            int s;
            byte[] copy_array = new byte[length];
            if (bytes_array.Length > length)
                throw new ArgumentOutOfRangeException("length is smaller than length of bytes array");
            s = length - bytes_array.Length;
            for (int j = 0; j < bytes_array.Length; j++)
                copy_array[j] = bytes_array[j];
            for (int i = 0; i < s; i++)
                copy_array[bytes_array.Length + i] = System.Convert.ToByte('\0');
            return copy_array;
        }
        public static byte[] cut_tail_zero(byte[] bytes_array)
        {
            int i = 0;
            foreach (byte s in bytes_array)
            {
                if (s != '\0')
                {
                    i += 1;
                }
                else
                    break;
            }
            byte[] copy_array = new byte[i];
            for (int j = 0; j < i; j++)
            {
                copy_array[j] = bytes_array[j];
            }
            return copy_array;
        }
        public static byte[] gen_command_bytes_array(byte[] command)
        {
            string com = System.Text.Encoding.UTF8.GetString(command);
            com = com.ToUpper();
            string[] str = new string[] { "POST", "GETDATA", "DATA", "ACK" };
            if (!str.Contains(com))
                throw new ArgumentOutOfRangeException("the command must be POST, GETDATA, DATA or ACK");
            return padding_by_zero(command, 8);
        }
        public static string gen_aes_key_hex()
        {
            string result;
            Random rd = new Random();
            rd.NextDouble();
            result = rd.ToString();
            MD5 md5 = new MD5CryptoServiceProvider();
            string key_hex = HashHelper.Hash_MD5_32(result);
            return key_hex;
        }

        /*
        public static byte[] json_encrypt(string key_hex, byte[] json_string)
        {
            MemoryStream mStream = new MemoryStream();
            RijndaelManaged aes = new RijndaelManaged();


            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            //aes.Key = _key; 
            aes.Key = Encoding.UTF8.GetBytes(key_hex);
            //aes.IV = _iV; 
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            try
            {
                cryptoStream.Write(json_string, 0, json_string.Length);
                cryptoStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
            
        }

        public static byte[] json_decrypt(string key_hex, byte[] cipher_text)
        {
            MemoryStream mStream = new MemoryStream(cipher_text);
            //mStream.Write( encryptedBytes, 0, encryptedBytes.Length ); 
            //mStream.Seek( 0, SeekOrigin.Begin ); 
            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = Encoding.UTF8.GetBytes(key_hex);
            //aes.IV = _iV; 
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            try
            {
                byte[] tmp = new byte[cipher_text.Length + 32];
                int len = cryptoStream.Read(tmp, 0, cipher_text.Length + 32);
                byte[] ret = new byte[len];
                Array.Copy(tmp, 0, ret, 0, len);
                return ret;
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
            
        }*/
        
        public static byte[] json_encrypt(string key_hex, byte[] json_string)
        {
            /*
            int length = json_string.Length;
            int last_length = length + (16 - (length % 16));
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key_hex);
            byte[] toEncryptArray = padding_by_zero(json_string, last_length);

            SymmetricAlgorithm des = Rijndael.Create();
            byte[] inputByteArray = toEncryptArray;
            des.Key = Encoding.UTF8.GetBytes(key_hex);
            des.IV = _key1;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();//得到加密后的字节数组
            cs.Close();
            ms.Close();

            string d = Convert.ToBase64String(cipherBytes);
            string e = Base64StringToString(d);
            byte[] result = Encoding.UTF8.GetBytes(e);
            return result;
            */
            return json_string;
            
        }

        public static byte[] json_decrypt(string key_hex, byte[] cipher_text_temp)
        {
            /*
            //string b = Encoding.UTF8.GetString(cipher_text_temp);
            //string c = changebase64(b);
            //byte[] cipher_text = Convert.FromBase64String(c);
            //string b = Convert.ToBase64String(cipher_text_temp);
            //byte[] cipher_text = Convert.FromBase64String(b);
            //Console.WriteLine("aaaaaaaaaaa{0}", cipher_text.Length);
            byte[] cipher_text = cipher_text_temp;
            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = Encoding.UTF8.GetBytes(key_hex);
            des.IV = _key1;
            
            byte[] decryptBytes = new byte[cipher_text.Length];
            
            MemoryStream ms = new MemoryStream(cipher_text);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(decryptBytes, 0, decryptBytes.Length);
            cs.Close();
            ms.Close();

            string d = Convert.ToBase64String(decryptBytes);
            string e = Base64StringToString(d);
            byte[] result = Encoding.UTF8.GetBytes(e);

            return cut_tail_zero(decryptBytes);
            */
            return cipher_text_temp;


        }
        /*
        public static byte[] json_encrypt(string key_hex, byte[] json_string_temp)
        {
            int length = json_string_temp.Length;
            int last_length = length + (16 - (length % 16));
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key_hex);
            byte[] toEncryptArray = padding_by_zero(json_string_temp, last_length);

            string b = Encoding.UTF8.GetString(toEncryptArray);
            string c = changebase64(b);
            byte[] json_string = Convert.FromBase64String(c);
            
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(json_string, 0, json_string.Length);

            string d = Convert.ToBase64String(resultArray);
            string e = Base64StringToString(d);
            byte[] result = Encoding.UTF8.GetBytes(e);
            return result;
        }

        public static byte[] json_decrypt(string key_hex, byte[] cipher_text_temp)
        {
            string b = Encoding.UTF8.GetString(cipher_text_temp);
            string c = changebase64(b);
            byte[] toEncryptArray = Convert.FromBase64String(c);

            byte[] keyArray = Encoding.UTF8.GetBytes(key_hex);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            string d = Convert.ToBase64String(resultArray);
            string e = Base64StringToString(d);
            byte[] result = Encoding.UTF8.GetBytes(e);
            return cut_tail_zero(result);
        }
        */


        public static string Base64StringToString(string base64)
        {
            if (base64 != "")
            {
                char[] charBuffer = base64.ToCharArray();
                byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
                string returnstr = Encoding.Default.GetString(bytes);
                return returnstr;
            }
            else
            {
                return "";
            }
        }

        public static string changebase64(string str)
        {
            if (str != "" && str != null)
            {
                byte[] b = Encoding.Default.GetBytes(str);
                string returnstr = Convert.ToBase64String(b);
                return returnstr;
            }
            else
            {
                return "";
            }
        }



    }
}

