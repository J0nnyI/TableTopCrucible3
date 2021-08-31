using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.ViewModels.Pages
{
    [Singleton(typeof(SettingsPvm))]
    public interface ISettingsPage
    {

    }
    public class SettingsPvm: ISettingsPage
    {
        public SettingsPvm()
        {
                
        }
    }
}
