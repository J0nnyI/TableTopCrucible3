using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.Wpf.Engine;

namespace TableTopCrucible.Starter
{
    public static class Program
    {
        [STAThread]
        public static void Main() => new EngineApplication().Run();
    }
}
