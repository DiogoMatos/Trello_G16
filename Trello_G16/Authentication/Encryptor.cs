using System;
using System.Text;

namespace Etapa2.Authentication
{
    public class Encryptor
    {

        private static readonly UTF8Encoding Encoding = new UTF8Encoding();

        public static string Encrypt(string str)
        {
            byte[] encrypted = StrToByteArray(str);
            return BitConverter.ToString(encrypted);
        }

        public static string Decrypt(string str)
        {
            String[] array = str.Split('-');
            var decrypted = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
                decrypted[i] = Convert.ToByte(array[i], 16);

            return Encoding.GetString(decrypted);
        }

        private static byte[] StrToByteArray(string str)
        {
            return Encoding.GetBytes(str);
        }
    }
}