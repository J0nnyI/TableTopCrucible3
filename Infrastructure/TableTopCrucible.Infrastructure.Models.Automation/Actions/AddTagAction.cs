using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Actions;

public class AddTagAction : ActionBase
{
    private IEnumerable<Tag> TagsToAdd { get; set; }

    public override void Apply(Lazy<IEnumerable<FileData>> files, Item item)
        => item.Tags.AddRange(TagsToAdd.ToArray());
}