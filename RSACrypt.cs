using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CourseWork20
{
    public enum CryptChoise
    {
        Encrypt = 0,
        Decrypt = 1
    }
    public class RSACrypt
    {

        private string publicKeyPath = "publicKey.conf";
        private string privateKeyPath = "privateKey.conf";

        private string publicKey = string.Empty;
        private string privateKey = string.Empty;

        public RSACrypt()
        {
            if (!checkExists())
            {
                generateKeys();
            }
        }

        private void generateKeys()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.PersistKeyInCsp = false;

                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);

                File.WriteAllText(publicKeyPath, publicKey, Encoding.UTF8);
                File.WriteAllText(privateKeyPath, privateKey, Encoding.UTF8);
            }
        }

        private bool checkExists()
        {
            if (File.Exists(publicKeyPath) && File.Exists(privateKeyPath))
            {
                publicKey = File.ReadAllText(publicKeyPath);
                privateKey = File.ReadAllText(privateKeyPath);
                return true;
            }
            return false;
        }

        public byte[] Encrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] ^= 1;
            }
            return input;
        }

        public byte[] Decrypt(byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input[i] ^= 1;
            }
            return input;
        }
    }
}
