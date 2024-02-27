using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CLI cli = new CLI();
            cli.Setup();
            cli.RunLoop();
        }
    }
}
