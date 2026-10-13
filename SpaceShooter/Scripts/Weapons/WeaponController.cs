using Godot;
using Godot.Collections;
using SpaceShooter.Pilots;
using SpaceShooter.Weapons;
using System;

namespace SpaceShooter.Entities.Ships
{
	public class WeaponController : Spatial
	{
		public ShipController MyShip { get; private set; }

		public Spatial Primary { get { return GetNode<Spatial>("Primary"); } }
		public Spatial Secondary { get { return GetNode<Spatial>("Secondary"); } }

		Array<Weapon> _primaries;
		Array<Weapon> _secondaries;

		int _currentPrimary = 0;
		int _primariesCount;

		float _primaryProjectileSpeed = 0;
		float _secondaryProjectileSpeed = 0;

		float _primaryTimeBetweenShoots;
		float _primaryTime;

		public bool shootPrimary = false;
		public bool shootSecondary = false;

        public override void _Ready()
        {
			MyShip = GetParent<ShipController>();
        }

        public override void _Process(float delta)
        {
			if (_primariesCount > 0)
			{
				if (_primaryTime > 0)
				{
					_primaryTime = Mathf.Max(_primaryTime - delta, 0);
				}

				bool primaryCanShoot = false;
				if (shootPrimary && _primaryTime == 0)
				{
					primaryCanShoot = true;
					_primaryTime = _primaryTimeBetweenShoots;
				}

				foreach (Weapon weapon in _primaries)
				{
					weapon.GlobalTransform = weapon.GlobalTransform.LookingAt(MyShip.PilotOwner.GetTargetPoint(), (weapon.GetParent() as Spatial).GlobalTransform.basis.Xform(Vector3.Up));
				}

				if (primaryCanShoot)
				{
					_primaries[_currentPrimary++].Shoot();
					if (_currentPrimary == _primariesCount)
						_currentPrimary = 0;
				}
			}

			shootPrimary = false;
			shootSecondary = false;
        }

        public override void _PhysicsProcess(float delta)
        {
			
        }

		public void Init()
		{
			_primaries = new Array<Weapon>();
			_secondaries = new Array<Weapon>();

			if (MyShip.PilotOwner.ShipConfiguration.PrimaryWeaponPackedScene != null)
			{
				foreach (Spatial socket in Primary.GetChildren())
				{
					Weapon weapon = MyShip.PilotOwner.ShipConfiguration.PrimaryWeaponPackedScene.Instance() as Weapon;
					if (_primaryProjectileSpeed == 0)
					{
						_primaryProjectileSpeed = weapon.ProjectileSpeed;
					}
					_primaryTimeBetweenShoots = 1.0f / weapon.RateOfFire;
					socket.AddChild(weapon);
					_primaries.Add(weapon);
				}
			}

			_primariesCount = _primaries.Count;

			if (MyShip.PilotOwner.ShipConfiguration.SecondaryWeaponPackedScene != null)
			{
				foreach (Spatial socket in Secondary.GetChildren())
				{
					Weapon weapon = MyShip.PilotOwner.ShipConfiguration.SecondaryWeaponPackedScene.Instance() as Weapon;
					if (_secondaryProjectileSpeed == 0)
					{
						_secondaryProjectileSpeed = weapon.ProjectileSpeed;
					}
					socket.AddChild(weapon);
					_secondaries.Add(weapon);
				}
			}
		}

		public float GetProjectileSpeed(bool primaryWeapon)
		{
			if (primaryWeapon)
				return _primaryProjectileSpeed;
			else
				return _secondaryProjectileSpeed;
		}

		public Vector3 GetIntersectionPoint(float projectileSpeed, Vector3 targetPosition, Vector3 targetVelocity)
		{
			Vector3 shipPosition = MyShip.GlobalPosition;

			Vector3 targetRelativePosition = targetPosition - shipPosition;
			float time = CalculateInterceptionTime(projectileSpeed, targetRelativePosition, targetVelocity);

			return targetPosition + time * targetVelocity;
		}

		float CalculateInterceptionTime(float projectileSpeed, Vector3 targetRelativePosition, Vector3 targetVelocity)
		{
			float velocitySquared = targetVelocity.LengthSquared();
			if (velocitySquared < 0.001f)
				return 0;
			
			float a = velocitySquared - projectileSpeed * projectileSpeed;

			if (Mathf.Abs(a) < 0.001f)
			{
				float t = -targetRelativePosition.LengthSquared() / 2f * targetVelocity.Dot(targetRelativePosition);
				return Mathf.Max(t, 0);
			}

			float b = 2f * targetVelocity.Dot(targetRelativePosition);
			float c = targetRelativePosition.LengthSquared();
			float determinant = b * b - 4f * a * c;

			if (determinant > 0f)
			{
				float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
				float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);

				if (t1 > 0)
				{
					if (t2 > 0)
					{
						return Mathf.Min(t1, t2);
					}
					else
					{
						return t1;
					}
				}
				else
				{
					return Mathf.Max(t2, 0);
				}
			}
			else if (determinant < 0)
			{
				return 0;
			}
			else
			{
				return Mathf.Max(-b / (2f * a), 0f);
			}
		}
	}
}