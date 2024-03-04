using BlockCounterCLI.program;
using System;
using System.Collections.Generic;

namespace BlockCounterCLI.command
{
    internal class ProgramRegistry
    {
        private ProgramRegistry() {
            programs = new List<BaseProgram>();
        }

        private static ProgramRegistry instance = null;

        private readonly List<BaseProgram> programs;

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
            if (program.DependsOn.Length != 0)
            {
                foreach (var dep in program.DependsOn)
                {
                    if (!dep.IsSubclassOf(typeof(BaseProgram)))
                    {
                        throw new ArgumentException("Tried to register program with dependency of invalid type " +  dep.ToString());
                    }
                }
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
