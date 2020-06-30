namespace CloudCopy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Program
    {
        public static int Main(string[] args)
        {
            var cloudCopyMainApp = new CloudCopyMainApp();

            int result = cloudCopyMainApp.run(args).Result;

            return result;
        }
    }
}
