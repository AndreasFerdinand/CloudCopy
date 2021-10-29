namespace CloudCopy
{
    public class CloudClientFactory : ICloudClientFactory
    {
        public C4CHttpClient CreateCloudClient(string Hostname, string Username)
        {
            C4CHttpClient cloudClient;
            INetworkCredentialHandler credentialHandler = null;

            string C4CHostName = string.Empty;
            /*
                    HOSTNAME leer    & USERNAME leer    => Hostname von Configfile    Username von Configfile   Passwort von Configfile
                    HOSTNAME leer    & USERNAME gefüllt => Hostname von Configfile    Username übernehmen       Passwort von Console
                    HOSTNAME gefüllt & USERNAME leer    => Hostname übernehmen        Username von Console      Passwort von Console
                    HOSTNAME gefüllt & USERNAME gefüllt => Hostname übernehmen        Username übernehmen       Passwort von Console
            */

            if (string.IsNullOrEmpty(Hostname) && string.IsNullOrEmpty(Username))
            {
                var configFileHandler = new ConfigFileHandler();

                C4CHostName = configFileHandler.Hostname;

                if (!configFileHandler.IsPasswordSet() || string.IsNullOrEmpty(configFileHandler.Username))
                {
                    credentialHandler = new ConsoleCredentialHandler(configFileHandler.Username);
                }
                else
                {
                    credentialHandler = configFileHandler;
                }
            }

            if (string.IsNullOrEmpty(Hostname) && !string.IsNullOrEmpty(Username))
            {
                var configFileHandler = new ConfigFileHandler();

                C4CHostName = configFileHandler.Hostname;

                credentialHandler = new ConsoleCredentialHandler(Username);
            }

            if (!string.IsNullOrEmpty(Hostname) && string.IsNullOrEmpty(Username))
            {
                C4CHostName = Hostname;

                credentialHandler = new ConsoleCredentialHandler();
            }

            if (!string.IsNullOrEmpty(Hostname) && !string.IsNullOrEmpty(Username))
            {
                C4CHostName = Hostname;

                credentialHandler = new ConsoleCredentialHandler(Username);
            }

            if (string.IsNullOrEmpty(C4CHostName))
            {
                throw new CloudCopyParametrizationException("Hostname missing");
            }

            if (credentialHandler == null)
            {
                throw new CloudCopyParametrizationException("Credentials missing");
            }

            IC4CFactory factory = new C4CFactory();

            cloudClient = factory.CreateC4CHttpClient(C4CHostName, credentialHandler);
            return cloudClient;
        }
    }
}