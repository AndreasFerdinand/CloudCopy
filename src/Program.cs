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

            CloudCopyMainApp mainApp = new CloudCopyMainApp();
            int result = mainApp.run(args).Result;;

            return result;

        }
    }
}
