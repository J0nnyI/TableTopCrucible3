using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities
{
    public class ItemEntity:IDataEntity<ItemId>
    {
        public Name Name { get; set; }
        public byte[] ModelFileHash { get; set; }
        public long ModelFileSize { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }
        public string Tags { get; set; } = "[]";
    }
}
