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

        public static byte[] json_encrypt(string key_hex, byte[] json_string)
        {
            //byte[] byteArray = System.Text.Encoding.Default.GetBytes(key_hex);
            int length = json_string.Length;
            int last_length = length + (16 - (length % 16));
            byte[] encrypt = null;
            encrypt = Encoding.UTF8.GetBytes(Encrypt(Encoding.UTF8.GetString(padding_by_zero(json_string, last_length)), key_hex));
            return encrypt;
        }

        public static byte[] json_decrypt(string key_hex, byte[] cipher_text)
        {
            string decrypt = Decrypt(Encoding.UTF8.GetString(cipher_text), key_hex);
            return cut_tail_zero(Encoding.UTF8.GetBytes(decrypt));
        }


        public static string Encrypt(string toEncrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string toDecrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}

