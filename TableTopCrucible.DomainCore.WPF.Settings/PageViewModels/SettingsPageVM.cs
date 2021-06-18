
using TableTopCrucible.Core.DI.Attributes;

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
