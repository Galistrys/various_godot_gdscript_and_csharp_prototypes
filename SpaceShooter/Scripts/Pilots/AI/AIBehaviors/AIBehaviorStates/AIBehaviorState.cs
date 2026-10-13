using Godot;
using System;

namespace SpaceShooter.Pilots.AI.Behaviors.States
{
	public abstract class AIBehaviorState
	{
		protected readonly AIBehavior _aiBehavior;
		protected readonly PilotAI _pilotAi;

		public AIBehaviorState(AIBehavior behavior, PilotAI pilot)
		{
			_aiBehavior = behavior;
			_pilotAi = pilot;
		}

		public virtual void OnEnter() {}
		public virtual void Process(float delta) {}
		public virtual void PhysicsProcess(float delta) {}
		public virtual void OnExit() {}
	}
}