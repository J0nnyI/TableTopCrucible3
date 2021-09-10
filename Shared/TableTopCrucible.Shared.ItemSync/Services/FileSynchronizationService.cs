using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;
using TableTopCrucible.Shared.ItemSync.Models;
using MoreLinq;

namespace TableTopCrucible.Shared.ItemSync.Services
{
    /// <summary>
    /// Synchronizes the master file list with the files in the directory
    /// </summary>
    //[singleton()]
    public interface IFileSynchronizationService
    {

    }
    public class FileSynchronizationService : IFileSynchronizationService
    {
        private readonly IDirectorySetupRepository _directorySetupRepository;
        private readonly IScannedFileRepository _fileRepository;

        public FileSynchronizationService(
            IDirectorySetupRepository directorySetupRepository,
            IScannedFileRepository fileRepository)
        {
            _directorySetupRepository = directorySetupRepository;
            _fileRepository = fileRepository;
        }

        public void StartScan()
        {
            _directorySetupRepository.Data.Items.Select(dir => startScanForDirectory(dir));
        }

        private object startScanForDirectory(DirectorySetup directory)
        {
            var foundFiles = directory.Path.GetFiles(FileType.Image, FileType.Model).ToArray();
            var knownFiles = _fileRepository.Data.Items;
            var files = foundFiles.FullJoin(
                knownFiles,
                foundFile => foundFile.Value,
                knownFile => knownFile.FileLocation.Value,
                found => new RawSyncFileData(null, found),
                known => new RawSyncFileData(known, null),
                (found, known) => new RawSyncFileData(known, found))
                .GroupBy(file=>file.State);

            var newFiles = files.FirstOrDefault(g => g.Key == FileState.New);
            var deletedFiles = files.FirstOrDefault(g => g.Key == FileState.Deleted);
            var updatedFiles = files.FirstOrDefault(g => g.Key == FileState.Updated);



            return null;
        }
    }
}
