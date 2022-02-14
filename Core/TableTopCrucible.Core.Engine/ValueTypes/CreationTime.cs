using System;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Core.Engine.ValueTypes;

public class CreationTime : ComparableValueType<DateTime, CreationTime>
{
    public static CreationTime Now => From(DateTime.Now);
}