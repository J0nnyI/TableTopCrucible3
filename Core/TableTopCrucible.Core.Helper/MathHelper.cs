using System;

namespace TableTopCrucible.Core.Helper;

public static class MathHelper
{
    public static int FloorInt(double value) => Convert.ToInt32(Math.Floor(value));

    public static int CeilingInt(double value) => Convert.ToInt32(Math.Ceiling(value));
    public static double Min(double value, double min) => value < min ? min : value;
    public static double Max(double value, double max) => value > max ? max : value;
}