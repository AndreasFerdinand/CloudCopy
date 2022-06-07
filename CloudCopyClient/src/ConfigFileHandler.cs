namespace CloudCopy
{
    using System;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Xml;
    using System.Xml.Serialization;

    public class ConfigFileHandler : INetworkCredentialHandler
    {
        private readonly string filepath;
        private string username;
        private string password;
        private string hostname;
        private bool passwordencrypted;

        public ConfigFileHandler()
        {
            this.filepath = GetDefaultConfigFilePath();

            this.LoadData();
        }

        public ConfigFileHandler(bool ignoreFile)
        {
            this.filepath = GetDefaultConfigFilePath();
        }

        public ConfigFileHandler(string filepath)
        {
            this.filepath = filepath;

            this.LoadData();
        }

        public string Username { get => this.username; set => this.username = value; }

        public string Password { get => this.password; set => this.SetPassword(value); }

        public string Hostname { get => this.hostname; set => this.hostname = value; }

        public bool PasswordEncrypted { get => this.passwordencrypted; set => this.passwordencrypted = value; }

        public static string GetDefaultConfigFilePath()
        {
            string tempFilepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            tempFilepath = tempFilepath + Path.DirectorySeparatorChar + "CloudCopy" + Path.DirectorySeparatorChar + "default.xml";

            return tempFilepath;
        }

        public string GetConfigFilePath()
        {
            return this.filepath;
        }

        public NetworkCredential GetCredentials()
        {
            return new NetworkCredential(this.Username, this.GetPassword());
        }

        public void SaveConfigurationFile()
        {
            if (!this.PasswordEncrypted && this.IsPasswordSet() && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                this.EncryptPassword();
            }

            Directory.CreateDirectory(Path.GetDirectoryName(this.filepath));

            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

            using (var fileToWrite = new FileStream(this.filepath, FileMode.Create, FileAccess.Write))
            {
                xmlSerializer.Serialize(fileToWrite, this);
            }
        }

        public bool IsPasswordSet()
        {
            return !string.IsNullOrEmpty(this.password);
        }

        private string GetPassword()
        {
            return this.PasswordEncrypted ? this.DecryptPassword() : this.password;
        }

        private void SetPassword(string password)
        {
            this.password = password;
            this.passwordencrypted = false;
        }

        private string DecryptPassword()
        {
            byte[] salt = System.Text.Encoding.Unicode.GetBytes(this.Hostname);
            byte[] decryptedPassword = ProtectedData.Unprotect(Convert.FromBase64String(this.password), salt, DataProtectionScope.CurrentUser);
            return System.Text.Encoding.Unicode.GetString(decryptedPassword);
        }

        private void EncryptPassword()
        {
            byte[] salt = System.Text.Encoding.Unicode.GetBytes(this.Hostname);
            byte[] decryptedPassword = System.Text.Encoding.Unicode.GetBytes(this.password);
            byte[] encryptedPassword = ProtectedData.Protect(decryptedPassword, salt, DataProtectionScope.CurrentUser);

            this.password = Convert.ToBase64String(encryptedPassword);
            this.PasswordEncrypted = true;
        }

        private void LoadData()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(this.filepath);

                XmlElement root = xmlDoc.DocumentElement;

                XmlNode node = root.SelectSingleNode("Hostname");
                this.Hostname = node?.InnerText ?? null;

                node = root.SelectSingleNode("Username");
                this.Username = node?.InnerText ?? null;

                node = root.SelectSingleNode("Password");
                this.password = node?.InnerText ?? null;

                node = root.SelectSingleNode("PasswordEncrypted");
                var passwordEncryptedRaw = node?.InnerText ?? null;

                Boolean.TryParse(passwordEncryptedRaw, out passwordencrypted);
            }
            catch (Exception ex)
            {
                throw new FileProcessingError(this.filepath, ex);
            }
        }
    }
}