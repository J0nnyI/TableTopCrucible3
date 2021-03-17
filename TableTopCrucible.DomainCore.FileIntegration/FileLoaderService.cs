using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TableTopCrucible.Core.DI.Attributes;
using TableTopCrucible.Core.Helper;
using TableTopCrucible.Core.Jobs;
using TableTopCrucible.Data.Library.Services.Sources;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.DomainCore.FileIntegration
{
    [Singleton((typeof(FileLoaderService)))]
    public interface IFileLoaderService
    {

    }
    internal class FileLoaderService
    {
        private readonly IItemService _itemService;
        private readonly IJobManagementService _jobManagementService;
        private readonly IFileSetupService _fileSetupService;

        public IEnumerable<DirectoryWithFilePaths> GetFilePathsForDirectory()
        => _fileSetupService
                .Directories
                .Items
                .Select(dir => 
                {
                    return new direc
                    dir,
                    images = 
                    files = Directory.GetFiles(dir.Path, "*", SearchOption.AllDirectories)
                        .Where(filePath=>FileSupportHelper.IsImage(filePath) || FileSupportHelper.IsModel(filePath))
                });
        public FileLoaderService(IItemService itemService, IJobManagementService jobManagementService, IFileSetupService fileSetupService)
        {
            _itemService = itemService;
            _jobManagementService = jobManagementService;
            _fileSetupService = fileSetupService;
        }

        private struct DirectoryWithFilePaths
        {
            SourceDirectory Directory { init; get; }
            IEnumerable<string> Files { init; get; }
        }
    }
}
