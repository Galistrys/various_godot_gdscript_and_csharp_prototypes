using Godot;
using System;

namespace SpaceShooter.Data
{
	public class ShipConfiguration
	{
		public PackedScene ShipScene { get; private set; }

		public int TeamNumber { get; private set; }

		public float MaxHealthModifier { get; set; } = 0;
		public float MaxShieldModifier { get; set; } = 0;

		public Vector3 MovementSpeedModifiers { get; set; } = Vector3.Zero;
		public Vector3 RotationSpeedModifiers { get; set; } = Vector3.Zero;

		public float MovementAccelerationModifier { get; set; } = 0;
		public float RotationAccelerationModifier { get; set; } = 0;

		public PackedScene PrimaryWeaponPackedScene { get; set; } = null;
		public PackedScene SecondaryWeaponPackedScene { get; set; } = null;

		public ShipConfiguration(PackedScene shipScene, int teamIdx)
		{
			ShipScene = shipScene;
			TeamNumber = teamIdx;
		}
	}
}