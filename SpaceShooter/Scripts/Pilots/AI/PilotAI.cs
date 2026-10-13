using Godot;
using System.Collections.Generic;
using SpaceShooter.Entities;
using SpaceShooter.Pilots.AI.Behaviors;
using SpaceShooter.Entities.Ships;
using SpaceShooter.Data;
using System;

namespace SpaceShooter.Pilots.AI
{
    public class PilotAI : Pilot
    {
		public float InaccuracyModifier = 25;

		[Export] public float shootRadius;
		public IEntity target = null;

		public List<IEntity> TargetsDetected { get; private set; } = new List<IEntity>();
		Area _detectArea;

		AIBehavior _pilotBehavior;
		
		public PilotAI() : base()
		{
			
		}

        public override void _ExitTree()
        {
            base._ExitTree();

			_pilotBehavior = null;
        }

        public override void _Ready()
        {
            base._Ready();
			_detectArea = GetNode<Area>("DetectTargetArea");
        }

        public override void Init(ShipConfiguration configuration)
        {
            base.Init(configuration);

			_pilotBehavior = new WanderAndAttackAIBehavior(this);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
			_detectArea.Translation = _shipController.Translation;
			_pilotBehavior.Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
			_pilotBehavior.PhysicsProcess(delta);
        }

		void OnDetectIEntity(Node node)
		{
			if (node is IEntity ie)
			{
				if (ie.GetTeamNumber() != TeamNumber)
				{
					(ie as Node).Connect("OnDeath", this, "OnIEntityDeath");
					TargetsDetected.Add(ie);
				}
			}
		}

		void OnLoseIEntity(Node node)
		{
			if (node is IEntity ie)
			{
				if (ie.GetTeamNumber() != TeamNumber)
				{
					TargetsDetected.Remove(ie);
					(ie as Node).Disconnect("OnDeath", this, "OnIEntityDeath");
				}
			}
		}

		void OnIEntityDeath(Node node)
		{
			// node.Disconnect("OnDeath", this, "OnIEntityDeath");
			TargetsDetected.Remove(node as IEntity);
		}

		public void FollowTargetPoint(Vector3 point)
		{
			Vector3 avoidance = ObstacleAvoidance();
			if (avoidance.LengthSquared() > 0)
			{
				ShipController.SetTargetMovementSpeedValues(avoidance);
			}
			else
			{
				MoveTowardsPoint(point);
			}

			RotateTowardsPoint(point);
		}

		public void RotateTowardsPoint(Vector3 position)
		{
			Transform targetTransform = ShipController.Transform.LookingAt(position, ShipController.Transform.basis.Xform(Vector3.Up));
			Vector3 rot = targetTransform.basis.GetEuler() - ShipController.Transform.basis.GetEuler();

			if (rot.Length() > 1)
			{
				float max = Mathf.Max(Mathf.Abs(rot[0]), Mathf.Abs(rot[1]));
				max = Mathf.Max(max, Mathf.Abs(rot[2]));
				if (max > 1)
					rot /= max;
			}
			
			ShipController.SetTargetRotationSpeedValues(rot);
		}

		public void MoveTowardsPoint(Vector3 position)
		{
			ShipController.SetTargetMovementSpeedValues(ShipController.ToLocal(position).Normalized());
		}

		public Vector3 ObstacleAvoidance()
		{
			if (ShipController.Velocity.LengthSquared() > 0)
			{
				Vector3 changeDir = Vector3.Zero;

				if (ShipController.TopRayCast.IsColliding())
					changeDir += Vector3.Down;
				else if (ShipController.BottomRayCast.IsColliding())
					changeDir += Vector3.Up;
				
				if (ShipController.RightRayCast.IsColliding())
					changeDir += Vector3.Left;
				else if (ShipController.LeftRayCast.IsColliding())
					changeDir += Vector3.Right;
				
				if (changeDir.LengthSquared() > 0)
				{
					return changeDir;
				}
			}

			return Vector3.Zero;
		}

        public override Vector3 GetTargetPoint()
        {
			Vector3 resultPoint = -ShipController.Transform.basis.z * 1000;
			if (target != null)
			{
				Random random = new Random();
				resultPoint = ShipController.WeaponController.GetIntersectionPoint(ShipController.WeaponController.GetProjectileSpeed(true), target.GlobalPosition, target.Velocity)
					+ new Vector3(random.Next(-1, 1), random.Next(-1, 1), random.Next(-1, 1)) * ShipController.GlobalPosition.DistanceTo(target.GlobalPosition) / InaccuracyModifier;
			}
			
            return resultPoint;
        }

        public override void OnShipDestroyed(Node node)
        {
			SetProcess(true);
			SetPhysicsProcess(true);
            QueueFree();
        }
    }
}