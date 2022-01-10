using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Helper
{
    public static class EntityFrameworkHelper
    {
        public static PropertyBuilder<TVt> HasValueTypeConversion<TV, TVt>(this PropertyBuilder<TVt> builder)
            where TVt : ValueType<TV, TVt>, new()
            => builder.HasConversion(
                x => x.Value,
                x => new TVt { Value = x });
    }
}
