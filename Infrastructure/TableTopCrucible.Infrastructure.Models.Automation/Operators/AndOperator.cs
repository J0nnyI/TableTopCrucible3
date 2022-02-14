using System;
using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Infrastructure.Models.Automation.Filters;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Operators;

public class AndOperator : ListOperatorBase
{
    public AndOperator(params IFilter[] elements)
    {
        Elements = elements;
    }

    public IEnumerable<IFilter> Elements { get; set; }

    public override bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item)
        => Elements.All(element => element.Apply(files, item));
}