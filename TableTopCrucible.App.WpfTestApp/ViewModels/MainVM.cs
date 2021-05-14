using Splat;

using System;
using System.Collections.Generic;
using System.Text;

namespace TableTopCrucible.App.WpfTestApp.ViewModels
{
    interface IMain
    {
        static IMain()
        {
            Locator.CurrentMutable.Register(() => new MainVM(), typeof(IMain));

        }
    }
    class MainVM : IMain
    {
        public MainVM()
        {
        }
    }
}
