
using System;
using System.Collections.Generic;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.DataTransfer.Master;
using TableTopCrucible.Data.Library.DataTransfer.Models;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.DataTransfer.Services
{
    [Singleton(typeof(DirectoryDataStorageService))]
    public interface IDirectoryDataStorageService
    {
        IObservable<IEnumerable<SourceDirectory>> SourceDirectoryListChanges { get; }

        void Quicksave(IEnumerable<SourceDirectory> directories);
    }
    public class DirectoryDataStorageService : IDirectoryDataStorageService
    {
        private static readonly DirectoryName _subDirectory = DirectoryName.From("Directories");
        private DirectoryPath _workingDirectory => MasterFileService.WorkDirectoryPath + _subDirectory;
        private readonly IMapperService _mapperService;

        public IObservable<IEnumerable<SourceDirectory>> SourceDirectoryListChanges { get; }
        public IMasterFileService MasterFileService { get; }

        public DirectoryDataStorageService(IMasterFileService masterFileService, IMapperService mapperService)
        {
            MasterFileService = masterFileService;
            _mapperService = mapperService;
        }
        public void Quicksave(IEnumerable<SourceDirectory> directories)
        {
            MasterFileService.Quicksave<SourceDirectoryDTO>(_subDirectory, directories);
        }
    }
}
