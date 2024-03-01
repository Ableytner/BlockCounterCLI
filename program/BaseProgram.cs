using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.program
{
    internal abstract class BaseProgram
    {
        public abstract string Name { get; }

        public abstract bool IsSetup();

        public abstract void Setup();
    }
}
