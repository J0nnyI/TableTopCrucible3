using TableTopCrucible.Core.ValueTypes;
using TableTopCrucible.Infrastructure.Models.Entities;
using TableTopCrucible.Infrastructure.Models.Entities;

namespace TableTopCrucible.Core.Jobs.ValueTypes
{
    public class WeightedCurrentProgress : ValueType<double, WeightedCurrentProgress>
    {
        public static explicit operator WeightedCurrentProgress(double value) => From(value);

        public static WeightedCurrentProgress operator
            +(WeightedCurrentProgress valueA, WeightedCurrentProgress valueB) =>
            (WeightedCurrentProgress)(valueA.Value + valueB.Value);
    }
}