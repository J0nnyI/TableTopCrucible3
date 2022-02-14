using System;
using Newtonsoft.Json;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities;

[JsonObject(MemberSerialization.OptIn)]
public sealed class FileData : DataEntity<FileDataId>
{
    public FileData()
    {
    }

    public FileData(FilePath path, FileHashKey hashKey, DateTime lastWrite)
    {
        Path = path;
        HashKey = hashKey;
        LastWrite = lastWrite;
    }

    private FilePath _path;

    [JsonProperty]
    public FilePath Path
    {
        get => _path;
        set => SetRequiredValue(ref _path, value);
    }

    private FileHashKey _hashKey;

    [JsonProperty]
    public FileHashKey HashKey
    {
        get => _hashKey;
        set => SetRequiredValue(ref _hashKey, value, nameof(HashKey));
    }

    private DateTime _lastWrite;

    [JsonProperty]
    public DateTime LastWrite
    {
        get => _lastWrite;
        set => SetRequiredValue(ref _lastWrite, value);
    }
}