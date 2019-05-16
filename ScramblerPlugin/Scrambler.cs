using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Plugin {
    public class Scrambler : IPlugin {
        Dictionary<string, object> configuration = new Dictionary<string, object>();
        private readonly byte[] KEY = {45, 111, 67, 77, 130, 1, 95, 187, 86, 3, 163, 42, 212, 162, 44, 199, 36,
                                       58, 5, 160, 79, 188, 220, 228, 58, 178, 66, 51, 168, 17, 48, 83};
        private readonly byte[] IV = { 54, 82, 122, 53, 5, 207, 253, 239, 227, 181, 47, 194, 36, 243, 12, 79 };

        public string Name => "Scrambler Plugin";

        public string Description => "Encodes/Decodes data usig AES-256";

        public object Decode(object data) {
            return DecryptStringFromBytes_Aes(data as byte[], KEY, IV);
        }

        public object Encode(object data) {
            return EncryptStringToBytes_Aes(data as byte[], KEY, IV);
        }

        static byte[] EncryptStringToBytes_Aes(byte[] plainText, byte[] Key, byte[] IV) {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (Aes aesAlg = Aes.Create()) {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }

        static byte[] DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV) {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            string plaintext = null;
            using (Aes aesAlg = Aes.Create()) {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText)) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return Encoding.UTF8.GetBytes(plaintext);
        }

        public void SetArgument(string key, object value) {
            configuration[key] = value;
        }

        public void Initialize(IHost host) {
        }

        public System.Windows.FrameworkElement GetUI() {
            return null;
        }

        public object Execute(object obj) {
            if (configuration.ContainsKey("mode"))
                if (configuration["mode"] as string == "encode")
                    return Encode(obj);
                else if (configuration["mode"] as string == "decode")
                    return Decode(obj);
            return null;
        }
    }
}
