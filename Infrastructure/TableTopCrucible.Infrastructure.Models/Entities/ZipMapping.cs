using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.EntityIds;

namespace TableTopCrucible.Infrastructure.Models.Entities;

/// <summary>
/// contains info about the content of a zip archive with a given tag 
/// </summary>
public class ZipMapping:DataEntity<ZipMappingId>
{
    public ZipMapping()
    {

    }
    public ZipMapping(FileHashKey zipFileHash, IEnumerable<ZipContent> content)
    {
        ZipFileHash = zipFileHash ?? throw new ArgumentNullException(nameof(zipFileHash));
        Content = content ?? throw new ArgumentNullException(nameof(content));
    }

    public FileHashKey ZipFileHash { get; }
    public IEnumerable<ZipContent> Content { get; }
    public Description Source { get; init; }
    public override string ToString()
        => $"{Source} - {Content.Count()} files - {ZipFileHash}";

}

/// <summary>
/// a single file contained in a zip archive
/// </summary>
public class ZipContent
{
    public ZipContent(FileType fileType, FileHashKey hashKey, RelativeFilePath path)
    {
        FileType = fileType;
        HashKey = hashKey ?? throw new ArgumentNullException(nameof(hashKey));
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public FileType FileType { get; }
    public FileHashKey HashKey { get; }
    public RelativeFilePath Path { get; }
    public override string ToString()
        => $"{Path} - {FileType} - {HashKey}";
}