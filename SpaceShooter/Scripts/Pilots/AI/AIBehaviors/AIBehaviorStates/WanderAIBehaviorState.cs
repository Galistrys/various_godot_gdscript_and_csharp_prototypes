using Godot;
using System;

namespace SpaceShooter.Pilots.AI.Behaviors.States
{
    public class WanderAIBehaviorState : AIBehaviorState
    {
		public const int WANDER_CUBE_AREA_SIZE = 150;
		public const float WANDER_POSITION_MIN_DISTANCE = 40;
		public const float WANDER_TARGET_POSITION_MARGIN = 2;

		Vector3 _targetPoint;

        public WanderAIBehaviorState(AIBehavior behavior, PilotAI pilot) : base(behavior, pilot)
        {
			RandomizeTargetPoint();
        }

        public override void Process(float delta)
        {
            base.Process(delta);
			
			if (_pilotAi.ShipController.GlobalPosition.DistanceTo(_targetPoint) < WANDER_TARGET_POSITION_MARGIN)
				RandomizeTargetPoint();

			_pilotAi.FollowTargetPoint(_targetPoint);
        }

		public void RandomizeTargetPoint()
		{
			Random rand = new Random();
			Vector3 result = new Vector3(
				rand.Next(-WANDER_CUBE_AREA_SIZE, WANDER_CUBE_AREA_SIZE),
				rand.Next(-WANDER_CUBE_AREA_SIZE, WANDER_CUBE_AREA_SIZE),
				rand.Next(-WANDER_CUBE_AREA_SIZE, WANDER_CUBE_AREA_SIZE)
			);

			if (result.DistanceTo(_pilotAi.ShipController.GlobalPosition) > WANDER_POSITION_MIN_DISTANCE)
				_targetPoint = result;
			else
			{
				if ((-result).DistanceTo(_pilotAi.ShipController.GlobalPosition) > WANDER_POSITION_MIN_DISTANCE)
					_targetPoint = -result;
				else
					_targetPoint = result.DirectionTo(_pilotAi.ShipController.GlobalPosition) * WANDER_POSITION_MIN_DISTANCE;
			}
		}
    }
}