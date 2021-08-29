using System;
using System.Collections.Generic;
using System.Text;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.ViewModels.Windows
{
    [Singleton(typeof(MainWindowVm))]
    public interface IMainWindow
    {
        string TestValue { get; set; }

    }
    internal class MainWindowVm: IMainWindow
    {
        public string TestValue { get; set; } = "works";
    }
}
