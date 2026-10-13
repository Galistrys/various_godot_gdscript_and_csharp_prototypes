using Godot;
using System;

namespace SpaceShooter.Entities
{
	public interface IEntity
	{
		string EntityId { get; }

		Vector3 GlobalPosition { get; }
		Vector3 Velocity { get; }

		float MaxHealth { get; }
		float CurrentHealth { get; }

		float MaxShield { get; }
		float CurrentShield { get; }

		int GetTeamNumber();
		void TakeDamage(float damage);
		void Die();
	}
}