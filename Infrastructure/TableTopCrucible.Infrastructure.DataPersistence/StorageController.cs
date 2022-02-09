using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using TableTopCrucible.Core.DependencyInjection.Attributes;
using TableTopCrucible.Core.Engine.Services;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.DataPersistence.Exceptions;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.EntityIds;
using Version = TableTopCrucible.Core.ValueTypes.Version;

namespace TableTopCrucible.Infrastructure.DataPersistence
{
    /// <summary>
    ///     accessed by the application to read and write data<br />
    ///     <u>responsibilities:</u><br />
    ///     provide a easy to use data structure<br />
    ///     writing data to disk<br />
    ///     loading data from disk<br />
    /// </summary>
    [Singleton]
    public interface IStorageController
    {
        SourceCache<Item, ItemId> Items { get; }
        SourceCache<ImageData, ImageDataId> Images { get; }
        SourceCache<FileData, FileDataId> Files { get; }
        SourceCache<DirectorySetup, DirectorySetupId> DirectorySetups { get; }
        void Load(LibraryFilePath file);
        void Save(LibraryFilePath file = null);
        void AutoSave();
    }

    internal class StorageController : IStorageController
    {
        private readonly INotificationService _notificationService;
        private readonly Subject<Unit> _autoSaveThrottle = new();

        private LibraryFilePath _currentFile;

        public StorageController(INotificationService notificationService)
        {
            _notificationService = notificationService;

            if (SettingsHelper.AutoSaveEnabled)
                _autoSaveThrottle
                    .Throttle(SettingsHelper.AutoSaveThrottle)
                    .Subscribe(_ => Save());

            OnStartup();
        }

        public SourceCache<Item, ItemId> Items { get; } = new(item => item.Id);
        public SourceCache<ImageData, ImageDataId> Images { get; } = new(image => image.Id);
        public SourceCache<FileData, FileDataId> Files { get; } = new(file => file.Id);
        public SourceCache<DirectorySetup, DirectorySetupId> DirectorySetups { get; } = new(dir => dir.Id);

        public void Load(LibraryFilePath file)
        {
            try
            {
                var newData = file.ReadAllJson<StorageMasterObject>();

                ClearAllData();

                try
                {
                    Items.AddOrUpdate(newData.Items);
                    Images.AddOrUpdate(newData.Images);
                    Files.AddOrUpdate(newData.Files);
                    DirectorySetups.AddOrUpdate(newData.DirectorySetups);
                }
                catch
                {
                    ClearAllData();
                    throw;
                }

                _currentFile = file;
            }
            catch (Exception e)
            {
                Debugger.Break();
                throw new LibraryLoadException(file, e);
            }
        }

        public void AutoSave()
            => _autoSaveThrottle.OnNext();

        /// <summary>
        ///     saves to the given file
        /// </summary>
        /// <param name="file">
        ///     the target file.<br />
        ///     file is null (default) => save to CurrentFile<br />
        ///     file is null and CurrentFile is null => <see cref="NullReferenceException" />
        /// </param>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="LibraryLoadException"></exception>
        public void Save(LibraryFilePath file = null)
        {
            file ??= _currentFile ?? throw new NullReferenceException("no file selected");

            var tmpFile = file.ChangeExtension(FileExtension.TemporaryJSonLibrary);

            try
            {
                tmpFile.GetDirectoryPath().Create();
                tmpFile.WriteAllJson(
                    new StorageMasterObject
                    {
                        Version = Version.From(1, 0, 0),
                        Items = Items.Items.ToArray(),
                        Files = Files.Items.ToArray(),
                        DirectorySetups = DirectorySetups.Items.ToArray(),
                        Images = Images.Items.ToArray()
                    });
                file.Delete();
                if (!file.Exists())
                    file.GetDirectoryPath().Create();
                tmpFile.Move(file);


                _notificationService.AddNotification("Save Successful",
                    $"The changes have been saved to {file}", NotificationType.Confirmation,
                    (NotificationIdentifier)"StorageController.Save");
            }
            catch (Exception e)
            {
                Debugger.Break();
                _notificationService.AddNotification(
                    "Save successful",
                    (Description)($"The changes could not be saved to {file}:" + Environment.NewLine + e),
                    NotificationType.Error);
                throw new LibraryLoadException(file, e);
            }
        }

        public void OnStartup()
        {
            if (LibraryFilePath.DefaultFile.Exists())
                Load(LibraryFilePath.DefaultFile);
            else
                _currentFile = LibraryFilePath.DefaultFile;
        }

        private void ClearAllData()
        {
            Items.Clear();
            Images.Clear();
            Files.Clear();
            DirectorySetups.Clear();
        }
    }

    /// <summary>
    /// </summary>
    internal class StorageMasterObject
    {
        public Version Version { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<ImageData> Images { get; set; }
        public IEnumerable<FileData> Files { get; set; }
        public IEnumerable<DirectorySetup> DirectorySetups { get; set; }
    }
}