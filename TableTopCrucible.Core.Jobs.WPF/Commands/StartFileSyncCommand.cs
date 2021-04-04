using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.Jobs.WPF.Commands
{
    [Singleton(typeof(StartFileSyncCommand))]
    public interface IStartFileSync
    {

    }
    public class StartFileSyncCommand:IStartFileSync
    {
    }
}
