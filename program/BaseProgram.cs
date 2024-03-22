using BlockCounterCLI.helpers;
using System;
using System.IO;

namespace BlockCounterCLI.program
{
    internal abstract class BaseProgram
    {
        public abstract string Name { get; }

        public abstract Type[] DependsOn { get; }

        public abstract bool IsSetup();

        public abstract void Setup();

        public virtual void Remove()
        {
            string programFolder = FileHelper.GetProgramsPath(Name);
            if (Directory.Exists(programFolder))
            {
                Directory.Delete(programFolder, true);
            }
        }

        public void EnsureIsSetup()
        {
            if (IsSetup())
            {
                throw new Exception(Name + " is not setup");
            }
        }
    }
}
