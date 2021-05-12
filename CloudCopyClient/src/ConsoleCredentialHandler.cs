namespace CloudCopy
{
    using System;
    using System.Net;
    using System.Text;

    public class ConsoleCredentialHandler : INetworkCredentialHandler
    {
        private string username;

        public ConsoleCredentialHandler()
        {
        }

        public ConsoleCredentialHandler(string username)
        {
            this.Username = username;
        }

        public string Username { get => this.username; set => this.username = value; }

        public NetworkCredential GetCredentials()
        {
            if (string.IsNullOrEmpty(this.Username))
            {
                Console.Write("Username: ");
                this.Username = Console.ReadLine();
            }

            Console.Write("Password: ");
            string password = ConsoleCredentialHandler.ReadPassword();

            Console.WriteLine("");

            return new NetworkCredential(this.Username, password);
        }

        public static string ReadPassword()
        {
            StringBuilder stringBuilder = new StringBuilder();

            while (true)
            {
                var key = System.Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                stringBuilder.Append(key.KeyChar);
            }

            return stringBuilder.ToString();
        }
    }
}