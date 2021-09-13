using System;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Repositories.Models.Entities;

namespace TableTopCrucible.Shared.ItemSync.Models
{
    enum FileState
    {
        New,
        Deleted,
        Updated,
        Unchanged
    }
    internal class RawSyncFileData
    {
        public RawSyncFileData(ScannedFileData knownFile, FilePath foundFile)
        {
            this.KnownFile = knownFile;
            FoundFile = foundFile;
            State = GetFilestate();
        }
        public ScannedFileData KnownFile { get; }
        public FilePath FoundFile { get; }

        public FileState State { get; }

        private FileState GetFilestate()
        {
            if (FoundFile == null && KnownFile == null)
                throw new ArgumentNullException(nameof(FoundFile), "at least one file must be set");
            if (FoundFile == null && KnownFile != null)
                return FileState.New;
            if (FoundFile != null && KnownFile == null)
                return FileState.Deleted;

            var foundFileInfo = FoundFile.GetFileInfo();

            if (foundFileInfo.LastWriteTime == KnownFile.LastWrite)
                return FileState.Unchanged;
            return FileState.Updated;
        }
    }
}
