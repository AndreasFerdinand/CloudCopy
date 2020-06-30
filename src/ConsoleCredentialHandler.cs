namespace CloudCopy
{
    using System;
    using System.Net;

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
            string password = this.ReadPassword();

            Console.WriteLine("");

            return new NetworkCredential(this.Username, password);
        }

        private string ReadPassword()
        {
            // https://stackoverflow.com/questions/23433980/c-sharp-console-hide-the-input-from-console-window-while-typing
            string password = null;

            while (true)
            {
                var key = System.Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                password += key.KeyChar;
            }

            return password;
        }
    }
}