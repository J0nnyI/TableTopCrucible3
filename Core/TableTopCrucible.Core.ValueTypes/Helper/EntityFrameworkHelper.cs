using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TableTopCrucible.Core.ValueTypes.Helper;

public static class EntityFrameworkHelper
{
    public static PropertyBuilder<TVt> HasValueTypeConversion<TV, TVt>(this PropertyBuilder<TVt> builder)
        where TVt : ValueType<TV, TVt>, new()
        => builder.HasConversion(
            x => x.Value,
            x => new TVt { Value = x });
}