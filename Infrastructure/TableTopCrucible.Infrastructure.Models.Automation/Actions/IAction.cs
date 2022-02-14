using System;
using System.Collections.Generic;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Actions;

public interface IAction
{
    public void Apply(Lazy<IEnumerable<FileData>> files, Item item);
}

public abstract class ActionBase : IAction
{
    public Description Description { get; set; }
    public abstract void Apply(Lazy<IEnumerable<FileData>> files, Item item);
}