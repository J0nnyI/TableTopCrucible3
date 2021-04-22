using AutoMapper;

using System;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Serialization;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Data.Library.Models.ValueTypes;
using TableTopCrucible.Data.Library.ValueTypes.IDs;
using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.DataTransfer.Models
{
    [DataContract]
    [AutoMap(typeof(FileData))]
    public class FileDataDTO
    {

        public string Path { get; set; }
        public byte[] FileHash { get; set; }
        public DateTime MostRecentUpdate { get; set; }
        public long FileSize { get; set; }


    }
}
