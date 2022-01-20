using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ReactiveUI.Fody.Helpers;

using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class ImageData : DataEntity<ImageDataId>
    {
        [Reactive]
        public Name Name { get; set; }

        public FileHashKey HashKey { get; set; }

        public ItemId ItemId { get; init; }
        public ImageData()
        {

        }

        public ImageData(Name name, FileHashKey hashKey)
        {
            HashKey = hashKey;
            Name = name;
        }
    }
    
}
