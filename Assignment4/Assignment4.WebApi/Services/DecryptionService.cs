using System.Security.Cryptography;

namespace Assignment4.WebApi.Services
{
    public class DecryptionService
    {
        private readonly IConfiguration _config;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIV;

        public DecryptionService(IConfiguration config)
        {
            _config = config;
            _aesKey = Convert.FromBase64String(_config["AES:Key"]!);
            _aesIV = Convert.FromBase64String(_config["AES:IV"]!);
        }

        /// <summary>
        /// Tar emot en byte array som med hjälps av Aes dekrypteras och returneras i form av en sträng
        /// Aes.IV och Aes.Key finns hårdkodade som Base64Strings och hämtas från appsettings.json. IV och Key är samma som i IoTSimulatorn
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            string plaintext = null!;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _aesKey;
                aesAlg.IV = _aesIV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
