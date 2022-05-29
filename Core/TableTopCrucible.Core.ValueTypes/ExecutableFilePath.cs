using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.ValueTypes;

public class ExecutableFilePath : FilePath<ExecutableFilePath>
{
    public void ThrowIfNotFound()
    {
        if (!Exists())
            throw new FileNotFoundException();
    }
    public Process Execute(string args)
    {
        ThrowIfNotFound();
        return Process.Start(new ProcessStartInfo(Value, args)
        {
            CreateNoWindow = true
        });
    }
}
