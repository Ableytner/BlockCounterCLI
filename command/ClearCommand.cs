﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockCounterCLI.command
{
    internal class ClearCommand : BaseCommand
    {
        public static new string prefix = "clear";
        public static new string description = "clear the terminal screen";

        public ClearCommand(string[] args) { }

        public override void Execute()
        {
            Console.Clear();
        }
    }
}
