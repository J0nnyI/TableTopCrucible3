using System;
using System.Collections.Generic;
using System.Linq;
using TableTopCrucible.Infrastructure.Models.Automation.Filters;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Operators;

public class OrOperator : ListOperatorBase
{
    public OrOperator(params IFilter[] elements)
    {
        Elements = elements;
    }

    public IEnumerable<IFilter> Elements { get; set; }

    public override bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item)
        => Elements.Any(element => element.Apply(files, item));
}