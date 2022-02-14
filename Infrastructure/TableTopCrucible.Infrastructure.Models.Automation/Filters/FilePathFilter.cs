using System;
using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Infrastructure.Models.Automation.Enums;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Filters;

public enum PathSegmentFilterType
{
    DirectoryName,
    Path,
    FileName,
    DirectoryNameOrFileName
}

internal class FilePathFilter : FilterBase
{
    public PathSegmentFilterType FilterType { get; set; }
    public TextMatchType TextMatch { get; set; }
    public CaseMatchType CaseMatch { get; set; }
    public ListMatchType ListMatchType { get; set; }

    public override bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item)
    {
        var inputText = FilterType switch
        {
            PathSegmentFilterType.DirectoryName
                => files.Value
                    .SelectMany(file => file.Path.GetDirectoryPath().GetDirectoryNames())
                    .Select(name => name.Value)
                    .ToArray(),
            PathSegmentFilterType.Path
                => files.Value
                    .Select(file => file.Path.Value).ToArray(),
            PathSegmentFilterType.FileName
                => files.Value
                    .Select(file => file.Path.GetFilename().Value).ToArray(),
            PathSegmentFilterType.DirectoryNameOrFileName
                => files.Value
                    .SelectMany(file => file.Path.GetDirectoryPath().GetDirectoryNames())
                    .Select(dirName => dirName.Value)
                    .Concat(files.Value.Select(file => file.Path.GetFilename().Value))
                    .ToArray(),
            _ => throw new ArgumentOutOfRangeException()
        };
        TextFilter innerFilter = new("", TextMatch, CaseMatch);
        var results = inputText.Select(text =>
        {
            innerFilter.FilterText = text;
            return innerFilter.Apply(files, item);
        }).ToArray();

        return ListMatchType switch
        {
            ListMatchType.ContainsAll => results.All(x => x),
            ListMatchType.ContainsAny => results.Any(x => x),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}