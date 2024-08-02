using System.Security.Cryptography;
using System;

namespace hello_world_api.Services
{
    public class AsymmetricEncryptionManager
    {

        private RSA _rsaPublic;
        private RSA _rsaPrivate;

        /// <summary>
        /// This constructor initializes an instance of the class with a given pair of RSA keys.
        /// </summary>
        /// <param name="publicKey">The asymmetric public key to use for encryption using RSA.</param>
        /// <param name="privateKey">The asymmetric private key to use for decryption and signing using RSA.</param>
        /// <exception cref="ArgumentNullException">If either key is null or empty</exception>
        public AsymmetricEncryptionManager(char[] publicKey, char[] privateKey)
        {
            if (publicKey == null || publicKey.Length <= 0)
                throw new ArgumentNullException("publicKey");
            if (privateKey == null || privateKey.Length <= 0)
                throw new ArgumentNullException("privateKey");

            this._rsaPublic = RSA.Create();
            this._rsaPublic.ImportFromPem(publicKey);

            this._rsaPrivate = RSA.Create();
            this._rsaPrivate.ImportFromPem(privateKey);
        }

        /// <summary>
        /// Encrypt given data using RSA.
        /// </summary>
        /// <param name="plainText">The data to encrypt</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If the plainText is null or empty</exception>
        public byte[] encrypt(byte[] plainText)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");

            return this._rsaPublic.EncryptValue(plainText);
        }

        /// <summary>
        /// Decrypt given encrypted data using RSA.
        /// </summary>
        /// <param name="cipherText">The data to decrypt</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If the cipherText is null or empty</exception>
        public byte[] decrypt(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            return this._rsaPrivate.DecryptValue(cipherText);
        }

        /// <summary>
        /// Sign given data using RSA.
        /// </summary>
        /// <param name="cipherText">The data to sign</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If the data is null or empty</exception>
        public byte[] sign(byte[] data)
        {
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("data");

            return this._rsaPrivate.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

    }
}
