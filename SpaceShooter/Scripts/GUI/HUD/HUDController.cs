using Godot;
using SpaceShooter.Managers;
using System;

namespace SpaceShooter.GUI.HUD
{
	public class HUDController : Control
	{
		public static HUDController Instance { get; private set; }
		public IntersectionControl IntersectionControl { get; private set; }
		public PointerControl PointerControl { get; private set; }
		public ShipStatsControl ShipStatsControl { get; private set; }

		[Export] NodePath PointerNodePath { get; set; }
		[Export] NodePath CircleNodePath { get; set; }

		public Sprite Pointer { get; private set; }
		public TextureRect Circle { get; private set; }

		public Vector2 PointerPosition { get { return Pointer.Position; } }
		public Vector2 CirclePosition { get { return Circle.RectPosition + Circle.RectSize / 2.0f; } }

		[Export] public readonly float MaxPointerDistanceToCircle;
		[Export] public readonly float PointerFromCircleCenterMargin;

		float _maxDistanceMinusMargin;

		public HUDController()
		{
			if (Instance != null)
			{
				MainManager.Instance.ExitGame(ExitCode.SingletonDuplication);
				return;
			}

			Instance = this;
		}

        public override void _ExitTree()
        {
            base._ExitTree();

			Instance = null;
        }

        /// <summary>
        /// Get turn values from pointer position.
        /// </summary>
        /// <param name="direction">Normalized vector2</param>
        /// <param name="value">Strenght [0.0f, 1.0f]</param>
        

		/// <summary>
		/// Get turn values from pointer position.
		/// </summary>
		/// <param name="direction">Normalized vector2</param>
		/// <param name="value">Strenght [0.0f, 1.0f]</param>
		public void GetTurn(out Vector2 direction, out float value)
		{
			direction = Pointer.Position.Normalized();
			value = Mathf.Max((PointerPosition.Length() - PointerFromCircleCenterMargin) / _maxDistanceMinusMargin, 0);
		}

        public override void _Ready()
        {
			_maxDistanceMinusMargin = MaxPointerDistanceToCircle - PointerFromCircleCenterMargin;

			Pointer = GetNode<Sprite>(PointerNodePath);
			Circle = GetNode<TextureRect>(CircleNodePath);
			IntersectionControl = GetNode<IntersectionControl>("IntersectionControl");
			PointerControl = GetNode<PointerControl>("CrossHair/PointerControl");
			ShipStatsControl = GetNode<ShipStatsControl>("ShipStatsControl");
        }

        public override void _Process(float delta)
        {

        }

		/// <summary>
		/// Move pointer in desired direction
		/// </summary>
		/// <param name="dir">Direction. Must be normalized!</param>
		/// <param name="value">Strength of the movement</param>
		public void MovePointer(Vector2 dir, float value)
		{
			Vector2 pointerPosition = Pointer.Position + dir * value;
			if (pointerPosition.Length() > MaxPointerDistanceToCircle)
			{
				pointerPosition = pointerPosition.Normalized() * MaxPointerDistanceToCircle;
			}

			Pointer.Position = pointerPosition;
		}

		/// <summary>
		/// Move pointer to desired location
		/// </summary>
		/// <param name="pos">Position. Must be normalized!</param>
		public void MovePointer(float speed, Vector2 pos)
		{
			Vector2 pointerPosition = pos * MaxPointerDistanceToCircle;
			if (pointerPosition.Length() > MaxPointerDistanceToCircle)
			{
				pointerPosition = pointerPosition.Normalized() * MaxPointerDistanceToCircle;
			}
			Pointer.Position = Pointer.Position.LinearInterpolate(pointerPosition, speed);
		}
	}
}