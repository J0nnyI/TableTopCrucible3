using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using Splat;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Repositories.Services;

namespace TableTopCrucible.Infrastructure.Models.Automation.Actions;

public class AddTagByFilePath : ActionBase
{
    public override void Apply(Lazy<IEnumerable<FileData>> files, Item item)
    {
        var dirRepo = Locator.Current.GetService<IDirectorySetupRepository>();
        var tags = files.Value.SelectMany(file =>
            file.Path.GetDirectoryPath()
                .MakeRelative(dirRepo!.SingleByFilepath(file.Path).Path)
                .GetDirectoryNames()
                .Select(name => (Tag)name.Value)
        ).Distinct();
        item.Tags.AddRange(tags);
    }
}