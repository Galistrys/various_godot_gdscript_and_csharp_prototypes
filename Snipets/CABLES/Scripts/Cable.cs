using Godot;
using System;

namespace KayEth.Entities.Interactables.Rigid.Grabbable.Cables
{
    class CablePoint
    {
        public Vector3 position, previousPosition;
        public bool locked;

        public CablePoint(Vector3 pos, bool isLocked = false)
        {
            position = pos;
            previousPosition = pos;
            locked = isLocked;
        }
    }

    class CableSegment
    {
        public CablePoint pointA, pointB;
        public float length;
    }

    public class Cable : Spatial
    {
        // [Export] float _segmentLength = 0.05f;
        [Export] float _segmentRadius = 0.025f;
        [Export] int _segmentCount = 16;
        [Export] int _numOfSimulationIterations = 3;
        [Export] float _curveFactor = 0.9f;

        [Export] float _desiredLength = 3;

        CablePlug _startPlug;
        CablePlug _endPlug;

        ImmediateGeometry _IM;

        CablePoint[] _points;
        CableSegment[] _segments;

        public override void _Ready()
        {
            GetTree().Root.DebugDraw = Viewport.DebugDrawEnum.Wireframe;

            _startPlug = GetNode<CablePlug>("StartCablePlug");
            _endPlug = GetNode<CablePlug>("EndCablePlug");
            _IM = GetNode<ImmediateGeometry>("IM");

            _points = new CablePoint[_segmentCount + 1];
            _segments = new CableSegment[_segmentCount];

            for (int i = 0; i < _segmentCount + 1; i++)
            {
                _points[i] = new CablePoint(_startPlug.Translation.LinearInterpolate(_endPlug.Translation, (float)i / _segmentCount));
            }

            _points[0].locked = true;
            _points[_segmentCount].locked = true;

            for (int i = 0; i < _segmentCount; i++)
            {
                _segments[i] = new CableSegment() {
                    pointA = _points[i],
                    pointB = _points[i + 1],
                    length = _points[i].position.DistanceTo(_points[i + 1].position) * _curveFactor
                };
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            float distance = _startPlug.Translation.DistanceTo(_endPlug.Translation);
            if (distance > _desiredLength)
            {
                var dir = (_endPlug.Translation - _startPlug.Translation).Normalized();

                float strength = (distance - _desiredLength) * delta * delta;

                _startPlug.ApplyCentralImpulse(dir * strength);
                _endPlug.ApplyCentralImpulse(-dir * strength);
            }
        }

        public override void _Process(float delta)
        {
            _points[0].position = _startPlug.Translation;
            _points[_segmentCount].position = _endPlug.Translation;

            for (int i = 0; i < _segmentCount; i++)
            {
                _segments[i].length = _startPlug.GlobalTransform.origin.DistanceTo(_endPlug.GlobalTransform.origin) / _segmentCount * _curveFactor;
            }
            
            Simulate(delta);
            Draw();
        }

        void Simulate(float delta)
        {
            for (int i = 0; i < _segmentCount + 1; i++)
            {
                var point = _points[i];
                if (!point.locked)
                {
                    Vector3 position_before_update = point.position;
                    point.position += point.position - point.previousPosition;
                    // point.position += GetGravityVector(0) * delta * delta * .5f;
                    point.previousPosition = position_before_update;
                }
            }

            for (int n = 0; n < _numOfSimulationIterations; n++)
            {
                for (int i = 0; i < _segmentCount; i++)
                {
                    var segment = _segments[i];
                    Vector3 segment_centre = (segment.pointA.position + segment.pointB.position) / 2.0f;
                    Vector3 segment_dir = (segment.pointA.position - segment.pointB.position).Normalized();

                    if (!segment.pointA.locked)
                        segment.pointA.position = segment_centre + segment_dir * segment.length / 2.0f;
                    if (!segment.pointB.locked)
                        segment.pointB.position = segment_centre - segment_dir * segment.length / 2.0f;
                }
            }
        }

        void Draw()
        {
            _IM.Clear();
            _IM.Begin(Mesh.PrimitiveType.Triangles);

            for (int i = 0; i < _segmentCount; i++)
            {
                CablePoint start = _points[i], end = _points[i + 1];

                Vector3 dir = (end.position - start.position).Normalized();
                Vector3 top_y = new Vector3(dir.z, dir.z, - dir.x - dir.y);
                if (top_y.LengthSquared() == 0)
                    top_y = new Vector3(-dir.y - dir.z, dir.x, dir.x);
                
                top_y = top_y.Normalized() * _segmentRadius;
                Vector3 bottom_y = -top_y;

                Vector3 right_x = dir.Cross(top_y);
                Vector3 left_x = -right_x;
                
                Vector3 back = start.position - dir * _segmentRadius;
                Vector3 front = end.position + dir * _segmentRadius;

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
    }
}
