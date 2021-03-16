using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI;
using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.App.WPF
{
    [Transient(typeof(MainWindow))]
    public interface IMainViewModel { }
    internal class MainViewModel:IMainViewModel
    {
    }
}
