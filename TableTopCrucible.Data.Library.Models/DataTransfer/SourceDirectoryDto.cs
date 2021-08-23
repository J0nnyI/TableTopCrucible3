using System;
using System.Collections.Generic;
using System.Text;

using TableTopCrucible.Core.DataAccess.Models;
using TableTopCrucible.Data.Library.Models.DataSource;
using TableTopCrucible.Data.Library.ValueTypes.IDs;

namespace TableTopCrucible.Data.Library.Models.DataTransfer
{
    public class SourceDirectoryDto : IEntityDto<SourceDirectoryId, SourceDirectory>
    {
        public string DirectoryValue { get; set; }
        /// <summary>
        /// the path where thumbnails are stored
        /// </summary>
        public string ThumbnailPathValue { get; set; }
        public string NameValue { get; set; }
        public Guid IdValue { get; set; }
    }
}
