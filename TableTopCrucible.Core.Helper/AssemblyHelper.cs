using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TableTopCrucible.Core.Helper
{
    public static class AssemblyHelper
    {
        public static IEnumerable<Assembly> GetSolutionAssemblies()
        {
            return Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Where(file => Path.GetFileName(file).Contains("TableTopCrucible"))
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)))
                .ToArray();
        }

    }
}
