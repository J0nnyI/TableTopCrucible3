using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using TableTopCrucible.Core.ValueTypes;

namespace TableTopCrucible.Shared.ValueTypes
{
    public class CameraView:ValueType<Point3D, Vector3D, Vector3D, CameraView>
    {
        public Point3D Position
        {
            get => ValueA;
            init => ValueA = value;
        }

        public Vector3D Direction
        {
            get => ValueB;
            init => ValueB = value;
        }

        public Vector3D UpDirection
        {
            get => ValueC;
            init => ValueC = value;
        }

        // same as default but with proper parameter names
        public new static CameraView From(Point3D position, Vector3D direction, Vector3D upDirection)
            => new() { Position = position, Direction = direction, UpDirection = upDirection };
    }
}
