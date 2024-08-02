using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace hello_world_api.Services
{
    public class SymmetricEncryptionManager
    {
        private Aes _aes;

        /// <summary>
        /// This constructor initializes an instance of the class with a given AES key.
        /// </summary>
        /// <param name="key">The symmetric key to use for encryption and decryption using AES.</param>
        /// <exception cref="ArgumentNullException">If the key is null or empty</exception>
        public SymmetricEncryptionManager(byte[] key) {
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");

            this._aes = Aes.Create();
            this._aes.Key = key;
            this._aes.Mode = CipherMode.CBC;
            this._aes.Padding = PaddingMode.PKCS7; 
        }

        /// <summary>
        /// Encrypt given data using AES and the given initialization vector.
        /// </summary>
        /// <param name="plainText">The data to encrypt</param>
        /// <param name="initializationVector">The initiazation vector used to randomize ciphertext, must be cryptorgaphically random per method call</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If either of the arguments are null or empty</exception>
        public byte[] encrypt(byte[] plainText, byte[] initializationVector)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (initializationVector == null || initializationVector.Length <= 0)
                throw new ArgumentNullException("initializationVector");

            this._aes.IV = initializationVector;
            ICryptoTransform encryptor = _aes.CreateEncryptor();
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        /// <summary>
        /// Decrypt given encrypted data using AES and the given initialization vector.
        /// </summary>
        /// <param name="cipherText">The data to decrypt</param>
        /// <param name="initializationVector">The initiazation vector that was used during encryption</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If either of the arguments are null or empty</exception>
        public byte[] decrypt(byte[] cipherText, byte[] initializationVector)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (initializationVector == null || initializationVector.Length <= 0)
                throw new ArgumentNullException("initializationVector");

            this._aes.IV = initializationVector;
            this._aes.Key = Encoding.ASCII.GetBytes("MPxBpDwpOpyS1k6kxdE++7KelLkvt99bSPZC2c0B/Mc=");
            ICryptoTransform decryptor = _aes.CreateDecryptor();
            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swDecrypt = new StreamWriter(csDecrypt))
                    {
                        swDecrypt.Write(cipherText);
                    }
                    return msDecrypt.ToArray();
                }
            }
        }

    }
}
