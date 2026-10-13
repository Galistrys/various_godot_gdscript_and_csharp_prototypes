using Godot;
using SpaceShooter.Entities;
using System;

namespace SpaceShooter.Pilots.AI.Behaviors.States
{
    public class AttackAIBehaviorState : AIBehaviorState
    {
		public const float DISTANCE_TO_TARGET = 20f;
		public const float SHOOT_RADIUS = 100f;

        public AttackAIBehaviorState(AIBehavior behavior, PilotAI pilot) : base(behavior, pilot)
        {

        }

        public override void Process(float delta)
        {
            base.Process(delta);

			float closestDistance = float.PositiveInfinity;
			foreach (IEntity detected in _pilotAi.TargetsDetected)
			{
				float dist = detected.GlobalPosition.DistanceTo(_pilotAi.ShipController.GlobalPosition);
				if (dist < closestDistance)
				{
					_pilotAi.target = detected;
					closestDistance = dist;
				}
			}

			if (_pilotAi.target == null)
				return;
			else
			{
				float distance = _pilotAi.target.GlobalPosition.DistanceTo(_pilotAi.ShipController.GlobalPosition);

				if (distance > DISTANCE_TO_TARGET)
					_pilotAi.FollowTargetPoint(_pilotAi.target.GlobalPosition);
				else
				{
					_pilotAi.RotateTowardsPoint(_pilotAi.target.GlobalPosition);
				}
				
				if (_pilotAi.target.GlobalPosition.DistanceTo(_pilotAi.ShipController.GlobalPosition) < SHOOT_RADIUS)
				{
					Vector3 intersectionPoint = _pilotAi.ShipController.WeaponController.GetIntersectionPoint(_pilotAi.ShipController.WeaponController.GetProjectileSpeed(true), _pilotAi.target.GlobalPosition, _pilotAi.target.Velocity);
					Vector3 local = _pilotAi.ShipController.ToLocal(intersectionPoint);
					if (local.z < 0 && local.AngleTo(Vector3.Forward) < Mathf.Pi / 4)
					{
						_pilotAi.Shoot(true);
					}
				}
			}
        }

        public override void OnExit()
        {
            base.OnExit();

			_pilotAi.target = null;
        }
    }
}