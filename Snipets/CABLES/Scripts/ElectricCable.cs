using Godot;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable.Cables
{  
    public class ElectricCablePoint
    {
        public Vector3 position, prevPosition;
        public bool Locked;

        public ElectricCablePoint(Vector3 pos, bool isLocked = false)
        {
            position = pos;
            prevPosition = pos;
            Locked = isLocked;
        }
    }
    
    public class ElectricCable : Spatial
    {
        [Export] SpatialMaterial _cableMaterial = null;
        [Export] PackedScene _cableSegmentPackedScene = null;

        [Export] float _cableRadius = 0.025f;
        [Export] int _segmentCount = 16;
        [Export] float _cableLength = 3f;

        [Export] int _numOfIterations = 3;

        ElectricCablePlug _startPlug;
        ElectricCablePlug _endPlug;

        ElectricCablePoint[] _points;
        ElectricCableSegment[] _segments;

        [Export(PropertyHint.Layers3dPhysics)] uint mask = 7;

        ImmediateGeometry _IM;

        public override void _Ready()
        {
            _IM = GetNode<ImmediateGeometry>("IM");
            _IM.MaterialOverride = _cableMaterial;

            _startPlug = GetNode<ElectricCablePlug>("StartCablePlug");
            _endPlug = GetNode<ElectricCablePlug>("EndCablePlug");
            _endPlug.Translation = new Vector3(0, 0, _cableLength);

            _points = new ElectricCablePoint[_segmentCount + 1];
            _segments = new ElectricCableSegment[_segmentCount];

            for (int i = 0; i < _segmentCount + 1; i++)
            {
                _points[i] = new ElectricCablePoint(_startPlug.Translation.LinearInterpolate(_endPlug.Translation, (float)i / _segmentCount));
            }

            _points[0].Locked = true;
            _points[_segmentCount].Locked = true;

            for (int i = 0; i < _segmentCount; i++)
            {
                var segment = _cableSegmentPackedScene.Instance() as ElectricCableSegment;
                AddChild(segment);
                segment.pointA = _points[i];
                segment.pointB = _points[i + 1];
                segment.ChangePosition();
                segment.CalculateLength();
                segment.CollisionMask = mask;
                _segments[i] = segment;
            }

            // _startPlug.AddCollisionExceptionWith(_segments[0]);
            // _segments[0].AddCollisionExceptionWith(_startPlug);
            // _endPlug.AddCollisionExceptionWith(_segments[_segmentCount - 1]);
            // _segments[_segmentCount - 1].AddCollisionExceptionWith(_endPlug);
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

        void Simulate(float delta)
        {
            for (int i = 0; i < _points.Length; i++)
            {
                ElectricCablePoint point = _points[i];
                if (!point.Locked)
                {
                    Vector3 position_before_update = point.position;
                    point.position += point.position - point.prevPosition;
                    point.position += GetGravityVector(0) * delta * delta;
                    point.prevPosition = position_before_update;
                }
            }

            for (int n = 0; n < _numOfIterations; n++)
            {
                for (int i = 0; i < _segmentCount; i++)
                {
                    var segment = _segments[i];
                    Vector3 desired_pos = (segment.pointA.position + segment.pointB.position) / 2.0f;
                    Vector3 dir = (segment.pointA.position - segment.pointB.position).Normalized();
                    Vector3 up = segment.GetUp(dir);
                    segment.CalculateRotation(dir, up);
                    var info = segment.MoveAndCollide(desired_pos - segment.Translation, false);

                    if (!segment.pointA.Locked)
                        segment.pointA.position = (segment.Translation + dir * segment.Length / 2.0f);
                    if (!segment.pointB.Locked)
                        segment.pointB.position = (segment.Translation - dir * segment.Length / 2.0f);
                }
            }
        }

        public override void _Process(float delta)
        {
            _points[0].position = _startPlug.GlobalTransform.origin;
            _points[_segmentCount].position = _endPlug.GlobalTransform.origin;

            // float length = GetLength();
            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i].Length = _startPlug.Translation.DistanceTo(_endPlug.Translation) / _segmentCount;
                // _segments[i].Length = length / _segmentCount;
            }

            Draw();
        }

        void Draw()
        {
            _IM.Clear();
            _IM.Begin(Mesh.PrimitiveType.Triangles);

            for (int i = 0; i < _segments.Length; i++)
            {
                ElectricCablePoint start = _points[i], end = _points[i + 1];

                Vector3 dir = (end.position - start.position).Normalized();
                Vector3 top_y = new Vector3(dir.z, dir.z, - dir.x - dir.y);
                if (top_y.LengthSquared() == 0)
                    top_y = new Vector3(-dir.y - dir.z, dir.x, dir.x);
                
                top_y = top_y.Normalized() * _cableRadius;
                Vector3 bottom_y = -top_y;

                Vector3 right_x = dir.Cross(top_y);
                Vector3 left_x = -right_x;
                
                Vector3 back = start.position - dir * _cableRadius;
                Vector3 front = end.position + dir * _cableRadius;

                AddTriangle(back, right_x + start.position, top_y + start.position);
                AddTriangle(back, top_y + start.position, left_x + start.position);
                AddTriangle(back, left_x + start.position, bottom_y + start.position);
                AddTriangle(back, bottom_y + start.position, right_x + start.position);

                AddTriangle(front, top_y + end.position, right_x + end.position);
                AddTriangle(front, left_x + end.position, top_y + end.position);
                AddTriangle(front, bottom_y + end.position, left_x + end.position);
                AddTriangle(front, right_x + end.position, bottom_y + end.position);

                AddTriangle(
                    right_x + start.position,
                    top_y + end.position,
                    top_y + start.position
                );

                AddTriangle(
                    right_x + start.position,
                    right_x + end.position,
                    top_y + end.position
                );

                AddTriangle(
                    bottom_y + start.position,
                    right_x + end.position,
                    right_x + start.position
                );

                AddTriangle(
                    bottom_y + start.position,
                    bottom_y + end.position,
                    right_x + end.position
                );

                AddTriangle(
                    left_x + start.position,
                    bottom_y + end.position,
                    bottom_y + start.position
                );

                AddTriangle(
                    left_x + start.position,
                    left_x + end.position,
                    bottom_y + end.position
                );

                AddTriangle(
                    top_y + start.position,
                    left_x + end.position,
                    left_x + start.position
                );

                AddTriangle(
                    top_y + start.position,
                    top_y + end.position,
                    left_x + end.position
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

        float GetLength()
        {
            float sum = 0;
            for (int i = 0; i < _segmentCount; i++)
            {
                sum += _points[i].position.DistanceTo(_points[i + 1].position);
            }
            return sum;
        }
    }
}
