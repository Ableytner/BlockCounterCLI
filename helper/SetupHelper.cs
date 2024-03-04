using BlockCounterCLI.command;
using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.helper
{
    internal class SetupHelper
    {
        public static void SetupAll()
        {
            BaseProgram[] all_targets = ProgramRegistry.Instance.GetPrograms();
            Setup(all_targets);
        }

        public static void Setup(Type[] targets)
        {
            BaseProgram[] corr_targets = new BaseProgram[targets.Length];

            for(int i = 0; i < targets.Length; i++)
            {
                corr_targets[i] = ProgramRegistry.Instance.GetProgram(targets[i]);
            }

            Setup(corr_targets);
        }

        public static void Setup(BaseProgram[] targets)
        {
            foreach (BaseProgram program in targets)
            {
                Setup(program);
            }
        }

        public static void Setup(Type program)
        {
            BaseProgram corr_program = ProgramRegistry.Instance.GetProgram(program);
            Setup(corr_program);
        }

        public static void Setup(BaseProgram program)
        {
            if (program.DependsOn.Length != 0)
            {
                Setup(program.DependsOn);
            }

            if (program.IsSetup())
            {
                Console.WriteLine("Skipping already installed program " + program.Name);
            }
            else
            {
                Console.WriteLine("Setting up " + program.Name);
                program.Setup();
            }
        }

        public static void RemoveAll()
        {
            Remove(ProgramRegistry.Instance.GetPrograms());
        }

        public static void Remove(BaseProgram[] programs)
        {
            foreach (BaseProgram program in programs)
            {
                Remove(program);
            }
        }

        public static void Remove(BaseProgram program)
        {
            Console.WriteLine("Deleting " + program.Name);
            program.Remove();
        }
    }
}
