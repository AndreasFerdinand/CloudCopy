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
            try
            {
                CloudCopyMainApp mainApp = new CloudCopyMainApp();
                mainApp.run(args);

                return 0;
            }
            catch (Exception ex)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine("The following exception occured during execution of the program:");
                errorWriter.WriteLine(ex.ToString());

                return 1;
            }
        }
    }
}
