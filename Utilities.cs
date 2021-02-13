using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Codenames
{
    public class Utilities
    {
        public static string AppPath { get => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
    }
}
