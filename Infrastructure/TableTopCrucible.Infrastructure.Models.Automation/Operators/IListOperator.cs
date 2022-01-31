using System.Collections.Generic;
using TableTopCrucible.Infrastructure.Models.Automation.Filters;

namespace TableTopCrucible.Infrastructure.Models.Automation.Operators
{
    public interface IListOperator:IFilter
    {
        IEnumerable<IFilter> Elements { get; set; }
    }

    public abstract class ListOperatorBase:FilterBase, IListOperator
    {
        public IEnumerable<IFilter> Elements { get; set; }
    }
}
