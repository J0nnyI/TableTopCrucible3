using AutoMapper;

using System;
using System.Runtime.Serialization;

using TableTopCrucible.Data.Models.Sources;

namespace TableTopCrucible.Data.Library.DataTransfer.Models
{
    [DataContract]
    [AutoMap(typeof(FileData), ReverseMap = true)]
    public class FileDataDTO
    {
        public string PathValue { get; set; }
        public byte[] HashValue { get; set; }
        public long SizeValue { get; set; }
        public DateTime MostRecentUpdate { get; set; }

    }
}
