using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal abstract class BaseCommand
    {
        public static string prefix;
        public static string description;

        public virtual Type[] DependsOn
        { 
            get
            {
                return new Type[0];
            }
        }

        protected string resultMessage { get; set; }

        protected bool errored { get; set; }

        public string GetResultMessage()
        {
            return resultMessage;
        }

        public bool HasErrored()
        {
            return errored;
        }

        public abstract void Execute();
    }
}
