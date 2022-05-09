using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExMapping
{
    public sealed class KeyConflictException: ApplicationException
    {
        public string Key { get; }
        public int Line { get; }

        public KeyConflictException(string key, int line): base($"Key Conflict: [{key}] at line #{line}.")
        {
            Key = key;
            Line = line;
        }

    }
}
