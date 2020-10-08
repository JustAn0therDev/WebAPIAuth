using System;
using System.Security.Cryptography;
using System.Text;

namespace WebAPIAuth.Utils
{
    public static class CrypthographyExtensions {
        ///<summary>
        ///<description>Returns the input string's encrypted version in SHA1.</description>
        ///<paramref name="stringToEncrypt"/>
        ///</summary>
        public static string GetEncryptedString(this string stringToEncrypt) {
            byte[] dataToEncryptInByteArray = Encoding.Unicode.GetBytes(stringToEncrypt);
            SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();

            byte[] byteArrayData = sha1CryptoServiceProvider.ComputeHash(dataToEncryptInByteArray);

            return Convert.ToBase64String(byteArrayData);
        }
    }
}