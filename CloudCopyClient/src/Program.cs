namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public static class Program
    {
        public static int Main(string[] args)
        {
            var cloudCopyMainApp = new CloudCopyMainApp();

            int result = cloudCopyMainApp.Run(args).Result;

            return result;
        }
    }
}
