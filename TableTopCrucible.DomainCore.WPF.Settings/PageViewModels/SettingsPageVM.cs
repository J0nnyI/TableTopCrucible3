
using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.DomainCore.WPF.Settings.PageViews;

namespace TableTopCrucible.DomainCore.WPF.Settings.ViewModels
{
    [Singleton(typeof(SettingsPageVM))]
    public interface ISettingsVM
    {

    }
    public class SettingsPageVM : ISettingsVM
    {
    }
}
