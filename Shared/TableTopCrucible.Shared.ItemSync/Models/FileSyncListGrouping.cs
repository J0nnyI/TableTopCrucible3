using System;
using System.Collections.Generic;
using System.Linq;

namespace TableTopCrucible.Shared.ItemSync.Models
{
    internal class FileSyncListGrouping
    {
        public FileSyncListGrouping(IEnumerable<RawSyncFileData> files)
        {
            var groups =
                files
                    .GroupBy(file => file.UpdateSource)
                    .ToArray();

            NewFiles = groups.FirstOrDefault(g => g.Key == FileUpdateSource.New)
                ?.DistinctBy(dir => dir.FoundFile)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();

            DeletedFiles = groups.FirstOrDefault(g => g.Key == FileUpdateSource.Deleted)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();

            UpdatedFiles = groups.FirstOrDefault(g => g.Key == FileUpdateSource.Updated)
                ?.DistinctBy(dir => dir.FoundFile)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();

            UnchangedFiles = groups.FirstOrDefault(g => g.Key == FileUpdateSource.Unchanged)
                ?.DistinctBy(dir => dir.FoundFile)
                ?.ToArray() ?? Array.Empty<RawSyncFileData>();
        }

        public IEnumerable<RawSyncFileData> NewFiles { get; }
        public IEnumerable<RawSyncFileData> UpdatedFiles { get; }
        public IEnumerable<RawSyncFileData> DeletedFiles { get; }
        public IEnumerable<RawSyncFileData> UnchangedFiles { get; }
    }
}