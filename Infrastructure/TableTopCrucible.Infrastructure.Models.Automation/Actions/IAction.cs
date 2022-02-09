using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using TableTopCrucible.Core.Engine.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Automation.Filters;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Infrastructure.Models.Automation.Actions
{
    public interface IAction
    {
        public void Apply(Lazy<IEnumerable<FileData>>files, Item item);
    }
    public abstract class ActionBase:IAction
    {
        public Description Description { get; set; }
        public abstract void Apply(Lazy<IEnumerable<FileData>> files, Item item);
    }
}
