using TableTopCrucible.Infrastructure.Models.Automation.Filters;

namespace TableTopCrucible.Infrastructure.Models.Automation.Operators
{
    internal interface ISingleOperator:IFilter
    {
        public IFilter Element { get; set; }
    }

    public abstract class SingleOperatorBase:FilterBase, ISingleOperator
    {
        public IFilter Element { get; set; }
    }
}
