using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;

namespace TableTopCrucible.Core.WPF.MainWindow.Services
{
    [Singleton(typeof(WorkAreaNavigationService))]
    public interface IWorkAreaNavigationService
    {

    }
    class WorkAreaNavigationService
    {
    }
}
