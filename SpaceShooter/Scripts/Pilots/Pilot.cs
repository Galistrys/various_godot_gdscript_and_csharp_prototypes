using Godot;
using SpaceShooter.Data;
using SpaceShooter.Entities.Ships;
using System;

namespace SpaceShooter.Pilots
{
	public abstract class Pilot : Node
	{
		protected ShipController _shipController;
		public ShipController ShipController { get { return _shipController; } }
		public ShipConfiguration ShipConfiguration { get; private set; }

		[Export] public int TeamNumber;

        public override void _Ready()
        {
			
        }

		~Pilot()
		{
			GD.Print("Destroyed: " + Name);
		}

		public virtual void Init(ShipConfiguration configuration)
		{
			ShipConfiguration = configuration;
			TeamNumber = configuration.TeamNumber;

			_shipController = configuration.ShipScene.Instance() as ShipController;
			_shipController.Connect("OnDeath", this, nameof(OnShipDestroyed));
			AddChild(_shipController);
			_shipController.Init(this, configuration);
		}

		public abstract Vector3 GetTargetPoint();

		public void Shoot(bool primary)
		{
			ShipController.Shoot(primary);
		}

		public abstract void OnShipDestroyed(Node node);
	}
}