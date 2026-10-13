using Godot;
using SpaceShooter.Pilots.AI.Behaviors.States;
using System;

namespace SpaceShooter.Pilots.AI.Behaviors
{
	public abstract class AIBehavior
	{
		protected readonly PilotAI _pilotAI;

		private AIBehaviorState _currentState = null;
		protected AIBehaviorState CurrentState { get { return _currentState; } }

		public AIBehavior(PilotAI pilotAI)
		{
			_pilotAI = pilotAI;
		}

		public virtual void Process(float delta) {}
		public virtual void PhysicsProcess(float delta) {}
		
		public void ChangeCurrentState(AIBehaviorState newState)
		{
			if (_currentState != null)
			{
				_currentState.OnExit();
			}
			_currentState = newState;
			_currentState.OnEnter();
		}

		
	}
}