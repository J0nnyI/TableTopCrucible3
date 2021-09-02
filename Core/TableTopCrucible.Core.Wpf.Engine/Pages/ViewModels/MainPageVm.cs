using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton(typeof(MainPageVm))]
    public interface IMainPage
    {

    }
    public class MainPageVm:IMainPage
    {
        public ISettingsPage SettingsPage { get; }

        public MainPageVm(ISettingsPage settingsPage)
        {
            SettingsPage = settingsPage;
        }
    }
}
