using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth
{
    internal class Logger
    {
        public static void Error(string message, Exception ex)
        {
            Console.Error.WriteLine("[ERROR] [" +DateTime.Now.ToShortTimeString() + "] " + message + "\n" + ex.ToString());
        }
    }
}
