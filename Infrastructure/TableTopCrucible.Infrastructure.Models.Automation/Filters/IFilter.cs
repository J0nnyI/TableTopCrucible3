using System;
using System.Collections.Generic;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Filters;

public interface IFilter
{
    Description Description { get; set; }
    bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item);
}

public abstract class FilterBase : IFilter
{
    public Description Description { get; set; }
    public abstract bool Apply(Lazy<IEnumerable<FileData>> files, Lazy<Item> item);
}