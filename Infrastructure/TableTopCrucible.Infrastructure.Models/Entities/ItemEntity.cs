using System;
using System.Collections.ObjectModel;
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
        public FileHashKey FileHashKey { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ItemId Id { get; init; }

        public ObservableCollection<Tag> Tags { get; } = new();
    }
}
