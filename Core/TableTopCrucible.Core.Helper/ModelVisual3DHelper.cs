namespace TableTopCrucible.Core.Helper
{
    public static class ModelVisual3DHelper
    {
        public static void Add(this Visual3DCollection visuals, params Visual3D[] children)
        {
            visuals.AddRange(children);
        }
    }
}