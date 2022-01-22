using HelixToolkit.Wpf;
using TableTopCrucible.Shared.ValueTypes;

namespace TableTopCrucible.Shared.Helper
{
    public static class ViewportHelper
    {
        public static CameraView GetCameraView(this HelixViewport3D viewport) =>
            CameraView.From(
                viewport.Camera.Position,
                viewport.Camera.LookDirection,
                viewport.Camera.UpDirection);

        public static void ApplyCameraView(this HelixViewport3D viewport, CameraView view)
        {
            viewport.CameraController.CameraUpDirection = view.UpDirection;
            viewport.CameraController.CameraLookDirection = view.Direction;
            viewport.CameraController.CameraPosition = view.Position;
        }
    }
}