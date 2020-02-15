using System;
using System.Net;

namespace CloudCopy
{
    class ConsoleCredentialHandler : INetworkCredentialHandler
    {
        string _Username;

        public ConsoleCredentialHandler()
        {
        }

        public ConsoleCredentialHandler(string username)
        {
            Username = username;
        }

        public string Username { get => _Username; set => _Username = value; }

        public NetworkCredential GetCredentials()
        {
            if ( string.IsNullOrEmpty(Username) )
            {
                Console.Write("Username: ");
                Username = Console.ReadLine();
            }


            Console.Write("Password: ");
            string password = readPassword();

            Console.WriteLine("");

            return new NetworkCredential(Username, password);
        }

        private string readPassword()
        {
            // https://stackoverflow.com/questions/23433980/c-sharp-console-hide-the-input-from-console-window-while-typing

            string password = null;
            while (true)
            {
                var key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }

            return password;
        }
    }
}