// <copyrights>
// Ce programme est la propriété de la société Cap Vision (capvision.fr).
// Tous droits réservés.
// Ce programme est protégé par les lois sur les droits d''auteur en vigueur en France
// et dans d''autres pays. Toute reproduction, modification, distribution ou utilisation
// sans autorisation préalable est strictement interdite.
//
// This program is the property of Cap Vision company (capvision.fr).
// All rights reserved.
// This program is protected by copyright laws in force in France
// and other countries. Any reproduction, modification, distribution or use
// without prior authorization is strictly prohibited.
// </copyrights>

using System.Security.Cryptography;
using System.Text;

namespace Extranet.Models.Helpers
{
    public class DESHelper
    {
        private static readonly string keyEncrypt = "yqz68yp1";
        private static readonly byte[] bEncryptionInitVector = [22, 51, 6, 45, 33, 37, 5, 62];

        internal static string EncryptToBase64String(string stringToEncrypt)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(stringToEncrypt))
                {
                    byte[] key = Encoding.UTF8.GetBytes(keyEncrypt);
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                    byte[] IV = bEncryptionInitVector;

                    MemoryStream ms = new();

                    var des = DES.Create();

                    CryptoStream cs = new(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    des.Dispose();
                    cs.Dispose();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception)
            { }

            return "";
        }

        internal static string DecryptFromBase64String(string stringToDecrypt)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(stringToDecrypt))
                {
                    byte[] key = Encoding.UTF8.GetBytes(keyEncrypt);
                    byte[] inputByteArray = Convert.FromBase64String(stringToDecrypt);
                    byte[] IV = bEncryptionInitVector;

                    MemoryStream ms = new();

                    var des = DES.Create();

                    CryptoStream cs = new(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);

                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();

                    des.Dispose();
                    cs.Dispose();

                    Encoding encoding = Encoding.UTF8;

                    return encoding.GetString(ms.ToArray());
                }
            }
            catch (Exception)
            { }

            return "";
        }
    }
}
