using System;
using System.Security.Cryptography;
using System.Text;

namespace WebAPIAuth.Utils
{
    public static class AuthenticationUtils {
        public static string GetEncryptedString(this string dataToEncrypt) {
            byte[] dataToEncryptInByteArray = Encoding.ASCII.GetBytes(dataToEncrypt);
            SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();

            byte[] byteArrayData = sha1CryptoServiceProvider.ComputeHash(dataToEncryptInByteArray);

            return Convert.ToBase64String(byteArrayData);
        }
    }
}