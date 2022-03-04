using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTopCrucible.Core.ValueTypes;

/// <summary>
///     used for complex value types like FileHashKey
/// </summary>
/// <typeparam name="TThis"></typeparam>
public abstract class ValueType<TThis>
    where TThis : ValueType<TThis>
{
}
