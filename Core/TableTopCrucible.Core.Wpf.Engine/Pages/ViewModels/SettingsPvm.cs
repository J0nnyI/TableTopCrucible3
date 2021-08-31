using System;
using System.Reactive.Disposables;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using TableTopCrucible.Core.Wpf.Engine.Models;
using TableTopCrucible.Core.Wpf.Engine.Services;
using TableTopCtucible.Core.DependencyInjection.Attributes;

namespace TableTopCrucible.Core.Wpf.Engine.Pages.ViewModels
{
    [Singleton(typeof(SettingsPvm))]
    public interface ISettingsPage
    {

    }
    public class SettingsPvm : ISettingsPage, IActivatableViewModel
    {
        public ObservableCollectionExtended<ISettingsCategoryPage> Pages { get; } =
            new ObservableCollectionExtended<ISettingsCategoryPage>();
        public SettingsPvm(ISettingsService settingsService)
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                settingsService
                    .Pages
                    .Connect()
                    .Bind(Pages)
                    .Subscribe()
                    .DisposeWith(disposables);
            });
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}
