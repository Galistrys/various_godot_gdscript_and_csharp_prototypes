using Godot;
using System;

namespace SpaceShooter.Entities.Ships
{
	public class ShipExplosion : Spatial
	{
		public override void _Ready()
        {
            base._Ready();
        }

		void OnAnimFinished(string name)
		{
			QueueFree();
		}
	}
}