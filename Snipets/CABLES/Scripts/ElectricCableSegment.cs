using Godot;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable.Cables
{
    public class ElectricCableSegment : KinematicBody
    {
        public ElectricCablePoint pointA, pointB;
        public float Length {
            get
            {
                return _shape.Extents.z * 2.0f;
            }
            set
            {
                var temp = _shape.Extents;
                temp.z = value / 2.0f;
                _shape.Extents = temp;
            }
        }

        BoxShape _shape;
        CapsuleShape _shape2;

        public override void _Ready()
        {
            _shape = GetNode<CollisionShape>("CollisionShape").Shape as BoxShape;
        }

        public void ChangePosition()
        {
            Translation = (pointA.position + pointB.position) / 2.0f;
        }

        public Vector3 CalculateDesiredPosition()
        {
            return (pointA.position + pointB.position) / 2.0f;
        }

        public void CalculateLength()
        {
            Length = pointA.position.DistanceTo(pointB.position);
        }

        public void CalculateRotation(Vector3 dir, Vector3 up)
        {
            var temp = Transform.Identity;
            temp.origin = pointA.position;
            temp = temp.LookingAt(pointB.position, up);
            temp.origin = Translation;
            Transform = temp;
        }

        public Vector3 GetUp(Vector3 dir)
        {
            Vector3 up = new Vector3(dir.z, dir.z, - dir.x - dir.y);
            if (up.LengthSquared() == 0)
                up = new Vector3(-dir.y - dir.z, dir.x, dir.x);
            
            up = up.Normalized();
            return up;
            // return Vector3.Up;
        }

        public Vector3 GetDir()
        {
            return (pointA.position - pointB.position).Normalized();
        }
    }
}
