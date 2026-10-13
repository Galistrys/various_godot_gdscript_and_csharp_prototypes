using Godot;
using SpaceShooter.Entities.Ships;
using SpaceShooter.Weapons;
using System;
using System.Collections.Generic;

namespace SpaceShooter.Managers
{
	public class BulletManager : Spatial
	{
		public static BulletManager Instance { get; private set; }

		int _bulletCount = 0;
		List<Bullet> _bullets = new List<Bullet>();

		int _processI = 0;
		int _physicsI = 0;

		public BulletManager()
		{
			Instance = this;
		}

        public override void _Ready()
        {
			Bullet.world = GetWorld();

			// Random rand = new Random();
			// for (int i = 0; i < 1200; i++)
			// {
			// 	var bullet = new LaserBullet(new Transform(Basis.Identity, new Vector3(rand.Next(-10, 10), rand.Next(-10, 10), rand.Next(-10, 10))));

			// 	AddBullet(bullet);
			// }
        }

        public override void _Process(float delta)
        {
            for (_processI = 0; _processI < _bulletCount; _processI++)
			{
				_bullets[_processI].Process(delta);
			}
        }

        public override void _PhysicsProcess(float delta)
        {
            for (int _physicsI = 0; _physicsI < _bulletCount; _physicsI++)
			{
				_bullets[_physicsI].PhysicsProcess(delta);
			}
        }

        public override void _ExitTree()
        {
			foreach (var bullet in _bullets)
			{
				bullet.Free();
			}
            Instance = null;
        }

		public void AddBullet(Bullet bullet)
		{
			_bullets.Add(bullet);
			_processI++;
			_physicsI++;
			_bulletCount++;
		}

		public void RemoveBullet(Bullet bullet)
		{
			_bullets.Remove(bullet);
			_processI--;
			_physicsI--;
			_bulletCount--;
		}
	}
}