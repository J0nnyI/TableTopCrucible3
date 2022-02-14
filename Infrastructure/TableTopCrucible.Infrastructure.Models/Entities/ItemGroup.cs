using DynamicData;
using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Controller;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities;

public class ItemGroup : DataEntity<ItemGroupId>
{
    [Reactive]
    [JsonProperty]
    public Name Name { get; set; }

    [JsonProperty]
    public TagCollection Tags { get; } = new();

    [JsonProperty]
    public SourceList<ItemId> ItemIds { get; } = new();

    [JsonProperty]
    public SourceList<ItemGroupId> SubGroupIds { get; } = new();
}