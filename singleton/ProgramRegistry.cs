using BlockCounterCLI.program;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class ProgramRegistry
    {
        private ProgramRegistry() {
            programs = new List<BaseProgram>();
        }

        private static ProgramRegistry instance = null;

        private List<BaseProgram> programs;

        public static ProgramRegistry Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ProgramRegistry();
                }
                return instance;
            }
        }

        public void RegisterProgram(BaseProgram program) {
            if (program == null || !program.GetType().IsSubclassOf(typeof(BaseProgram)))
            {
                throw new ArgumentException("Tried to register program with invalid type " + program.GetType().ToString());
            }
            foreach (var prog in programs)
            {
                if (prog.GetType().Name == program.GetType().Name)
                {
                    throw new Exception("Program " +  program.GetType().Name + " is already registered");
                }
            }

            programs.Add(program);
        }

        public dynamic GetProgram(Type type)
        {
            foreach (var program in programs)
            {
                if (program.GetType().Name.ToLower() == type.Name.ToLower())
                {
                    return Convert.ChangeType(program, type);
                }
            }

            return null;
        }

        public BaseProgram GetProgram(string name)
        {
            foreach (var program in programs)
            {
                if (program.Name.ToLower() == name.ToLower())
                {
                    return program;
                }
            }

            return null;
        }

        public BaseProgram[] GetPrograms()
        {
            return programs.ToArray();
        }
    }
}
