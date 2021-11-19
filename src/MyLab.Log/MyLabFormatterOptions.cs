using System.IO;
using Microsoft.Extensions.Logging.Console;

namespace MyLab.Log
{
    class MyLabFormatterOptions : ConsoleFormatterOptions
    {
        public TextWriter DebugWriter { get; set; }
    }
}
