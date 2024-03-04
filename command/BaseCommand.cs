﻿using System;

namespace BlockCounterCLI.command
{
    internal abstract class BaseCommand
    {
        public static string Prefix;
        public static string Description;

        public virtual Type[] DependsOn
        { 
            get
            {
                return new Type[0];
            }
        }

        protected string ResultMessage { get; set; }

        protected bool Errored { get; set; }

        public string GetResultMessage()
        {
            return ResultMessage;
        }

        public bool HasErrored()
        {
            return Errored;
        }

        public abstract void Execute();
    }
}
