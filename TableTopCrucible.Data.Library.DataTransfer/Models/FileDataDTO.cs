using System;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Serialization;

using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes.General;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Library.DataTransfer.Models
{
    [DataContract]
    public class FileDataDTO
    {

        public FileDataHashKey HashKey { get; set; }
        public FilePath Path { get; set; }
        // the time when the file (not the model) was created
        public FileHash FileHash { get; set; }
        public DateTime MostRecentUpdate { get; set; }
        public SourceDirectoryId DirectorySetupId { get; set; }
        // identifies this item in this specific state
        public FileSize FileSize { get; set; }
        public PathType Type { get; set; }


    }
}
