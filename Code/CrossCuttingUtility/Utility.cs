using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CrossCuttingUtility
{
    public class Utility
    {

        public static string Encrypt(string password , string emailAddress, string salt)
        {
            var provider = MD5.Create();
            string fullSalt = string.Format("{0}{1}", RemoveSpecialCharacters(emailAddress).ToLower(), salt);
            byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(fullSalt + password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
    }
}
