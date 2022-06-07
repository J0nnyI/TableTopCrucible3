using System.IO.Abstractions.TestingHelpers;
using System.Windows.Media;

using DynamicData;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using TableTopCrucible.Core.DependencyInjection;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Core.TestHelper;

public class TestStlThumbSettings : ReactiveObject, IStlThumbSettings
{
    [Reactive]
    public ExecutableFilePath InstallationPath { get; set; }
    [Reactive]
    public Color DefaultMaterial { get; set; }
    [Reactive]
    public bool Enabled { get; set; }
    [Reactive]
    public int TimeOut { get; set; }
}
public class TestStorageController : IStorageController
{
    public SourceCache<Item, ItemId> Items { get; } = new(d=>d.Id);
    public SourceCache<ImageData, ImageDataId> Images { get; } = new(d=>d.Id);
    public SourceCache<FileData, FileDataId> Files { get; } = new(d=>d.Id);
    public SourceCache<DirectorySetup, DirectorySetupId> DirectorySetups { get; } = new(d=>d.Id);
    public SourceCache<ItemGroup, ItemGroupId> ItemGroups { get; } = new(d=>d.Id);
    public SourceCache<ZipMapping, ZipMappingId> ZipMappings { get; } = new(d=>d.Id);

    public void AutoSave()
    {
    }

    public void Load(LibraryFilePath file)
    {
    }

    public void Save(LibraryFilePath file = null)
    {
    }
}

public class Synchronization : ReactiveObject, ISynchronizationSettings
{
    public bool AutoGenerateThumbnail { get; set; }
}
public class testSettingsService : ISettingsService
{
    public IStlThumbSettings StlThumb { get; } = new TestStlThumbSettings();
    public ISynchronizationSettings Synchronization { get; }= new Synchronization();
}

// test environment setups
public static class Prepare
{
    // prepares an integration application environment
    public static IHost ApplicationEnvironment()
    {
        return Host
            .CreateDefaultBuilder()
            .ConfigureServices(Services)
            .ConfigureLogging(loggingBuilder => { loggingBuilder.AddSplat(); })
            .UseEnvironment(
                Environments.Development
            )
            .Build();
    }

    public static IServiceCollection Services()
    {
        var services = new ServiceCollection();
        Services(services);
        return services;
    }

    public static void Services(IServiceCollection services)
    {
        services.UseMicrosoftDependencyResolver();
        var resolver = Locator.CurrentMutable;
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();


        DependencyBuilder.AddTtcServices(services);
        services.ReplaceFileSystem<MockFileSystem>();
        services.RemoveAll<IStorageController>();
        services.AddSingleton<IStorageController, TestStorageController>();
    }
}