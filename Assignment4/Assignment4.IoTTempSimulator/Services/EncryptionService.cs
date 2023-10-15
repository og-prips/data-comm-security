using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Assignment4.IoTTempSimulator.Services
{
    internal class EncryptionService
    {
        private readonly IConfiguration _config;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIV;

        public EncryptionService(IConfiguration config)
        {
            _config = config;
            _aesKey = Convert.FromBase64String(_config["AES:Key"]!);
            _aesIV = Convert.FromBase64String(_config["AES:IV"]!);
        }

        /// <summary>
        /// Tar emot en sträng och gör om denna till en krypterad byte array med hjälp utav Aes.
        /// Aes.IV och Aes.Key finns hårdkodade som Base64Strings och hämtas från config.json. IV och Key är samma som i API:et
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public byte[] EncryptStringToBytes_Aes(string s)
        {
            if (s == null || s.Length <= 0)
                throw new ArgumentNullException("s");
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _aesKey;
                aesAlg.IV = _aesIV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(s);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }
    }
}
