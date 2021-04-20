
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.WPF.Helper.Attributes;
using TableTopCrucible.DomainCore.WPF.Settings.PageViews;

namespace TableTopCrucible.DomainCore.WPF.Settings.ViewModels
{
    [Singleton(typeof(SettingsPageVM))]
    public interface ISettingsVM
    {

    }
    [ViewModel(typeof(SettingsPageV))]
    internal class SettingsPageVM : ISettingsVM
    {
    }
}
