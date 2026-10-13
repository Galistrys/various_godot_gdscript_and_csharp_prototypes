using Godot;
using SpaceShooter.Entities.Ships;
using SpaceShooter.Managers;
using System;

namespace SpaceShooter.Weapons
{
	public abstract class Weapon : Spatial
	{
		[Export] NodePath ProjectilePositionSpawn { get; set; }
		Position3D _projectileSpawnPosition;

		public abstract float RateOfFire { get; protected set; }
		
		public WeaponController WeaponController { get; private set; }
		public AudioStreamPlayer3D PewPlayer { get; private set; }

		public abstract float ProjectileSpeed { get; }

        public override void _Ready()
        {
			WeaponController = GetParent().GetParent().GetParent<WeaponController>();
			PewPlayer = GetNode<AudioStreamPlayer3D>("PewPlayer");
            _projectileSpawnPosition = GetNode<Position3D>(ProjectilePositionSpawn);
        }

		public abstract Bullet CreateBullet(Transform transform);

		public void Shoot()
		{
			// var bullet = (Projectile)ProjectilePackedScene.Instance();
			// bullet.MyShip = WeaponController.MyShip;
			// bullet.SetColor(WeaponController.MyShip.ShipColor);
			// GameManager.Instance.VFXSpatial.AddChild(bullet);
			// bullet.GlobalTransform = _projectileSpawnPosition.GlobalTransform;

			var bullet = CreateBullet(_projectileSpawnPosition.GlobalTransform);
			// bullet.MyShip = WeaponController.MyShip;
			// bullet.SetColor(WeaponController.MyShip.ShipColor);
			BulletManager.Instance.AddBullet(bullet);

			PewPlayer.Play();
		}
	}
}