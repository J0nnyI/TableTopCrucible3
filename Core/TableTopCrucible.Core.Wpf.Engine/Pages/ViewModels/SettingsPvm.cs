using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;

using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton(typeof(SettingsPvm))]
    public interface ISettingsPage
    {

    }
    public class SettingsPvm : ReactiveObject, ISettingsPage, IActivatableViewModel
    {
        public ObservableCollectionExtended<ISettingsCategoryPage> Pages { get; } = new();
        [Reactive]
        public ISettingsCategoryPage CurrentPage { get; set; }
        public SettingsPvm(ISettingsService settingsService)
        {
            this.WhenActivated(() => new[]
            {
                settingsService
                    .Pages
                    .Connect()
                    .Do(changes=>
                        CurrentPage=changes.FirstOrDefault().Current)
                    .Bind(Pages)
                    .Subscribe(),
            });
        }


        public ViewModelActivator Activator { get; } = new();
    }
}
