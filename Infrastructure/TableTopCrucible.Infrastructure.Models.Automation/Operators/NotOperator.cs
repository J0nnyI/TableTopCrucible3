using System;
using System.Collections.Generic;
using TableTopCrucible.Infrastructure.Models.Automation.Filters;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Operators;

public class NotOperator : SingleOperatorBase
{
    public NotOperator(IFilter element)
    {
        Element = element;
    }

    public override bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item) => !Element.Apply(files, item);
}