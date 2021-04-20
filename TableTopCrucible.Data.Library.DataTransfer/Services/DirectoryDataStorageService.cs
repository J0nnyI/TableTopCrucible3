using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.DataTransfer.Services
{
    [Singleton(typeof(DirectoryDataStorageService))]
    public interface IDirectoryDataStorageService
    {
        IObservable<IEnumerable<SourceDirectory>> SourceDirectoryListChanges { get; }
    }
    public class DirectoryDataStorageService : IDirectoryDataStorageService
    {
        public IObservable<IEnumerable<SourceDirectory>> SourceDirectoryListChanges { get; }
        public DirectoryDataStorageService()
        {

        }
    }
}
