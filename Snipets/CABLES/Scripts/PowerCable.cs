using Godot;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable.Cables
{
    public class PowerCable : Spatial
    {
        class Stick
        {
            public PowerCablePoint pointA, pointB;
            public float length;

            public Stick(PowerCablePoint a, PowerCablePoint b, float len)
            {
                pointA = a;
                pointB = b;
                length = len;
            }
        }

        [Export] PackedScene _cablePointPackedScene = null;

        [Export] SpatialMaterial _cableMaterial = null;
        [Export] float _cableRadius = 0.025f;

        [Export] float _segmentLength = 0.1f;
        [Export] float _cableLength = 3f;

        [Export] int _numOfIterations = 3;

        PowerCablePlug _startPlug;
        PowerCablePlug _endPlug;

        PowerCablePoint[] _points;
        Stick[] _segments;

        MeshInstance[] _stickMeshes;

        Godot.Collections.Array _exludeArray = new Godot.Collections.Array();
        [Export(PropertyHint.Layers3dPhysics)] uint mask = 7;

        ImmediateGeometry _IM;

        public override void _Ready()
        {
            _IM = GetNode<ImmediateGeometry>("IM");
            _IM.MaterialOverride = _cableMaterial;

            _startPlug = GetNode<PowerCablePlug>("StartPlug");
            _endPlug = GetNode<PowerCablePlug>("EndPlug");

            _exludeArray.Add(_startPlug);
            _exludeArray.Add(_endPlug);

            int segments_count = Mathf.CeilToInt(_cableLength / _segmentLength);
            _points = new PowerCablePoint[segments_count + 1];
            _segments = new Stick[segments_count];
            _stickMeshes = new MeshInstance[segments_count];

            _points[0] = _cablePointPackedScene.Instance() as PowerCablePoint;
            _points[0].Locked = true;
            _points[0].CollisionMask = mask;
            _points[0].AddCollisionExceptionWith(_startPlug);
            _startPlug.AddCollisionExceptionWith(_points[0]);
            AddChild(_points[0]);

            for (int i = 0; i < segments_count; i++)
            {
                var point = _cablePointPackedScene.Instance() as PowerCablePoint;
                AddChild(point);
                point.CurrentPosition = _points[i].CurrentPosition + new Vector3(0, 0, -_segmentLength);
                point.PreviousPosition = point.CurrentPosition;
                point.CollisionMask = mask;
                
                _points[i + 1] = point;
                _segments[i] = new Stick(_points[i], _points[i + 1], _segmentLength);

                // _stickMeshes[i] = new MeshInstance();
                // AddChild(_stickMeshes[i]);
                // _stickMeshes[i].Mesh = new CubeMesh() { Size = new Vector3(_cableRadius, _cableRadius, _segmentLength), Material = _cableMaterial };
            }

            var temp = _endPlug.GlobalTransform;
            temp.origin = _points[segments_count].CurrentPosition;
            _endPlug.GlobalTransform = temp;
            _points[segments_count].Locked = true;
            _points[segments_count].AddCollisionExceptionWith(_endPlug);
            _endPlug.AddCollisionExceptionWith(_points[segments_count]);
        }

        public override void _PhysicsProcess(float delta)
        {
            float distance = _startPlug.Translation.DistanceTo(_endPlug.Translation);
            if (distance > _cableLength)
            {
                var dir = (_endPlug.Translation - _startPlug.Translation).Normalized();

                float strength = (distance - _cableLength) * delta * delta;

                _startPlug.ApplyCentralImpulse(dir * strength);
                _endPlug.ApplyCentralImpulse(-dir * strength);
            }
            Simulate(delta);
        }

        public override void _Process(float delta)
        {
            _points[0].CurrentPosition = _startPlug.GlobalTransform.origin;
            _points[_points.Length - 1].CurrentPosition = _endPlug.GlobalTransform.origin;

            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i].length = _startPlug.GlobalTransform.origin.DistanceTo(_endPlug.GlobalTransform.origin) / _segments.Length * 0.8f;
            }

            Draw();
        }

        void Simulate(float delta)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                PowerCablePoint point = _points[i];
                if (!point.Locked)
                {
                    Vector3 desired_pos = point.CurrentPosition;
                    desired_pos += point.CurrentPosition - point.PreviousPosition;
                    desired_pos += Vector3.Down * 9.81f * delta * delta;
                    point.PreviousPosition = point.CurrentPosition;

                    point.MoveAndCollide(desired_pos - point.CurrentPosition, false);
                }
            }

            for (int n = 0; n < _numOfIterations; n++)
            {
                for (int i = 0; i < _segments.Length; i++)
                {
                    Stick stick = _segments[i];
                    if (stick.pointA.CurrentPosition.DistanceTo(stick.pointB.CurrentPosition) > stick.length)
                    {
                        Vector3 stickCentre = (stick.pointA.CurrentPosition + stick.pointB.CurrentPosition) / 2.0f;
                        Vector3 stickDir = (stick.pointA.CurrentPosition - stick.pointB.CurrentPosition).Normalized();

                        // if (!stick.pointA.Locked)
                        //     stick.pointA.CurrentPosition = stickCentre + stickDir * stick.length / 2.0f;
                        // if (!stick.pointB.Locked)
                        //     stick.pointB.CurrentPosition = stickCentre - stickDir * stick.length / 2.0f;
                        
                        if (!stick.pointA.Locked)
                            stick.pointA.MoveAndCollide((stickCentre + stickDir * stick.length / 2.0f) - stick.pointA.CurrentPosition, false);
                        if (!stick.pointB.Locked)
                            stick.pointB.MoveAndCollide((stickCentre - stickDir * stick.length / 2.0f) - stick.pointB.CurrentPosition, false);
                    }
                }
            }
        }

        // void Draw()
        // {
        //     for (int i = 0; i < _stickMeshes.Length; i++)
        //     {
        //         PowerCablePoint left = _points[i];
        //         PowerCablePoint right = _points[i + 1];

        //         Transform temp = Transform.Identity;
        //         temp.origin = left.CurrentPosition;
        //         temp = temp.LookingAt(right.CurrentPosition, Vector3.Up);
        //         temp.origin = (left.CurrentPosition + right.CurrentPosition) / 2.0f;

        //         _stickMeshes[i].GlobalTransform = temp;

        //         // (_stickMeshes[i].Mesh as CapsuleMesh).MidHeight = left.CurrentPosition.DistanceTo(right.CurrentPosition);
        //         (_stickMeshes[i].Mesh as CubeMesh).Size = new Vector3(_cableRadius, _cableRadius, left.CurrentPosition.DistanceTo(right.CurrentPosition));
        //     }
        // }

        void Draw()
        {
            _IM.Clear();
            _IM.Begin(Mesh.PrimitiveType.Triangles);

            for (int i = 0; i < _segments.Length; i++)
            {
                PowerCablePoint start = _points[i], end = _points[i + 1];

                Vector3 dir = (end.CurrentPosition - start.CurrentPosition).Normalized();
                Vector3 top_y = new Vector3(dir.z, dir.z, - dir.x - dir.y);
                if (top_y.LengthSquared() == 0)
                    top_y = new Vector3(-dir.y - dir.z, dir.x, dir.x);
                
                top_y = top_y.Normalized() * _cableRadius;
                Vector3 bottom_y = -top_y;

                Vector3 right_x = dir.Cross(top_y);
                Vector3 left_x = -right_x;
                
                Vector3 back = start.CurrentPosition - dir * _cableRadius;
                Vector3 front = end.CurrentPosition + dir * _cableRadius;

                AddTriangle(back, right_x + start.CurrentPosition, top_y + start.CurrentPosition);
                AddTriangle(back, top_y + start.CurrentPosition, left_x + start.CurrentPosition);
                AddTriangle(back, left_x + start.CurrentPosition, bottom_y + start.CurrentPosition);
                AddTriangle(back, bottom_y + start.CurrentPosition, right_x + start.CurrentPosition);

                AddTriangle(front, top_y + end.CurrentPosition, right_x + end.CurrentPosition);
                AddTriangle(front, left_x + end.CurrentPosition, top_y + end.CurrentPosition);
                AddTriangle(front, bottom_y + end.CurrentPosition, left_x + end.CurrentPosition);
                AddTriangle(front, right_x + end.CurrentPosition, bottom_y + end.CurrentPosition);

                AddTriangle(
                    right_x + start.CurrentPosition,
                    top_y + end.CurrentPosition,
                    top_y + start.CurrentPosition
                );

                AddTriangle(
                    right_x + start.CurrentPosition,
                    right_x + end.CurrentPosition,
                    top_y + end.CurrentPosition
                );

                AddTriangle(
                    bottom_y + start.CurrentPosition,
                    right_x + end.CurrentPosition,
                    right_x + start.CurrentPosition
                );

                AddTriangle(
                    bottom_y + start.CurrentPosition,
                    bottom_y + end.CurrentPosition,
                    right_x + end.CurrentPosition
                );

                AddTriangle(
                    left_x + start.CurrentPosition,
                    bottom_y + end.CurrentPosition,
                    bottom_y + start.CurrentPosition
                );

                AddTriangle(
                    left_x + start.CurrentPosition,
                    left_x + end.CurrentPosition,
                    bottom_y + end.CurrentPosition
                );

                AddTriangle(
                    top_y + start.CurrentPosition,
                    left_x + end.CurrentPosition,
                    left_x + start.CurrentPosition
                );

                AddTriangle(
                    top_y + start.CurrentPosition,
                    top_y + end.CurrentPosition,
                    left_x + end.CurrentPosition
                );
            }

            _IM.End();
        }

        Vector3 GetGravityVector(float weight)
        {
            return Vector3.Down * 9.81f;
        }

        void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            var side1 = b - a;
            var side2 = c - a;
            var normal = side1.Cross(side2);
            _IM.SetNormal(normal);
            _IM.AddVertex(c);
            _IM.SetNormal(normal);
            _IM.AddVertex(b);
            _IM.SetNormal(normal);
            _IM.AddVertex(a);
        }
    }
}
