using Godot;
using SpaceShooter.Pilots.AI.Behaviors.States;
using System;

namespace SpaceShooter.Pilots.AI.Behaviors
{
    public class WanderAndAttackAIBehavior : AIBehavior
    {
        public WanderAndAttackAIBehavior(PilotAI pilotAI) : base(pilotAI)
        {
			ChangeCurrentState(new WanderAIBehaviorState(this, pilotAI));
        }

        public override void Process(float delta)
        {
            base.Process(delta);

			if (CurrentState is WanderAIBehaviorState && _pilotAI.TargetsDetected.Count > 0)
			{
				ChangeCurrentState(new AttackAIBehaviorState(this, _pilotAI));
			}
			else if (CurrentState is AttackAIBehaviorState && _pilotAI.TargetsDetected.Count == 0)
			{
				ChangeCurrentState(new WanderAIBehaviorState(this, _pilotAI));
			}

			CurrentState.Process(delta);
        }
    }
}