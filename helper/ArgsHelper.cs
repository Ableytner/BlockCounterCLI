using BlockCounterCLI.command;
using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.helper
{
    internal class ArgsHelper
    {
        public static BaseProgram[] ParsePrograms(string[] args)
        {
            if (args.Length == 0)
            {
                return new BaseProgram[0];
            }

            BaseProgram[] programs = new BaseProgram[args.Length];
            for(int i = 0; i < args.Length; i++)
            {
                BaseProgram program = ProgramRegistry.Instance.GetProgram(args[i]);
                programs[i] = program ?? throw new ArgumentException("Could not find program " + args[i]);
            }
            return programs;
        }
    }
}
