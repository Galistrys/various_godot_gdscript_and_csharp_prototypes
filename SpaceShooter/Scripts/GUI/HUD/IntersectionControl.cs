using Godot;
using System;

namespace SpaceShooter.GUI.HUD
{
	public class IntersectionControl : Control
	{
		Line2D _line;
		Sprite _point;
		Sprite _targetDirection;
		TargetHealthControl _targetHealthControl;

		Vector2 _from;
		Vector2 _to;
		Vector2 _direction;

		[Export] public float IntersectionRadius { get; private set; }
		[Export] public float TargetDirectionRadius { get; private set; }

		bool _lineMode = true;

        public override void _Ready()
        {
			_line = GetNode<Line2D>("LineToIntersectionPoint");
			_point = GetNode<Sprite>("IntersectionPoint");
			_targetDirection = GetNode<Sprite>("TargetDirection");
			_targetHealthControl = GetNode<TargetHealthControl>("TargetHealth");
        }

		public void HideIntersection()
		{
			this.Visible = false;
			_line.Visible = false;
			_point.Visible = false;
			_targetDirection.Visible = false;
			_targetHealthControl.Visible = false;
			_line.ClearPoints();
		}

		public bool IsPointerInIntersectionPoint()
		{
			return HUDController.Instance.Pointer.GlobalPosition.DistanceTo(_point.GlobalPosition) < IntersectionRadius;
		}

        public override void _Process(float delta)
        {
            if (Visible)
			{
				if (_lineMode)
				{
					_point.Position = _to;
					Vector2 dir = _from.DirectionTo(_to);
					float length = _from.DistanceTo(_to) - IntersectionRadius;

					_line.ClearPoints();
					_line.AddPoint(_from);
					_line.AddPoint(_from + dir * length);

					_targetHealthControl.RectPosition = _from;

					_line.Visible = true;
					_point.Visible = true;
				}
				else
				{
					Vector2 circlePos = HUDController.Instance.CirclePosition;
					_targetDirection.Position = HUDController.Instance.CirclePosition + _direction * (HUDController.Instance.MaxPointerDistanceToCircle + TargetDirectionRadius);
					_targetDirection.Rotation = _direction.Angle();

					_targetDirection.Visible = true;
				}
			}
        }

		public void ShowLine(Vector2 from, Vector2 to, float health, float maxHealth, float shield, float maxShield)
		{
			_from = from;
			_to = to;
			_lineMode = true;
			_targetHealthControl.Visible = true;
			_targetHealthControl.SetHealthAndShieldValue(health, maxHealth, shield, maxShield);
			_targetDirection.Visible = false;
			this.Visible = true;
		}

		public void ShowDirection(Vector2 direction)
		{
			_direction = direction;
			_lineMode = false;
			_line.Visible = false;
			_targetHealthControl.Visible = false;
			_point.Visible = false;
			this.Visible = true;
		}
	}
}