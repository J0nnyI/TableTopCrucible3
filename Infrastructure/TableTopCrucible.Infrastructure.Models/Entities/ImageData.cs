using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities;

[JsonObject(MemberSerialization.OptIn)]
public class ImageData : DataEntity<ImageDataId>
{
    [Reactive]
    [JsonProperty]
    public Name Name { get; set; }

    [Reactive]
    [JsonProperty]
    public FileHashKey HashKey { get; set; }

    [Reactive]
    [JsonProperty]
    public bool IsThumbnail { get; set; }

    [JsonProperty]
    public ItemId ItemId { get; init; }

    [JsonProperty]
    public ItemGroupId ItemGroupId { get; init; }

    public ImageData()
    {
    }

    public ImageData(Name name, FileHashKey hashKey)
    {
        HashKey = hashKey;
        Name = name;
    }
}