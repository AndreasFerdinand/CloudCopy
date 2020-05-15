using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace CloudCopy
{
    class Program
    {
        static int Main(string[] args)
        {
            var cloudCopyMainApp = new CloudCopyMainApp();

            int result = cloudCopyMainApp.run(args).Result;

            return result;
        }
    }
}
