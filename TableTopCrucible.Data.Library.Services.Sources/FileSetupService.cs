using DynamicData;

using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Library.Models.IDs;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.Services.Sources
{
    [Singleton(typeof(FileSetupService))]
    public interface IFileSetupService
    {
        IObservableCache<SourceDirectory, SourceDirectoryId> Directories { get; }
        void AddOrUpdateDirectory(SourceDirectory directory);
    }
    internal class FileSetupService:IFileSetupService
    {
        private SourceCache<SourceDirectory, SourceDirectoryId> _directories = new SourceCache<SourceDirectory, SourceDirectoryId>(dir=>dir.Id);
        public IObservableCache<SourceDirectory, SourceDirectoryId> Directories => _directories;
        public void AddOrUpdateDirectory(SourceDirectory directory)
        {
            this._directories.AddOrUpdate(directory);
        }
    }
}
