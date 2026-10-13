using Godot;
using SpaceShooter.Entities;
using SpaceShooter.Entities.Ships;
using SpaceShooter.Managers;
using System;

namespace SpaceShooter.Weapons
{
	public abstract class Bullet
	{
		public static World world;

		public abstract float Speed { get; }
		public abstract float Timelife {get; }
		public abstract float Damage { get; }
		protected abstract Mesh Mesh { get; }
		public ShipController MyShip { get; set; }

		private float _lifeTime = 0;

		protected RID _instance;
		private Transform _transform;

		// public abstract SpatialMaterial BulletMaterials { get; }
		public static SpatialMaterial[] GlowingMaterials { get; set; }

		Vector3 _oldPos;

		public Bullet(Transform transform)
		{
			_instance = VisualServer.InstanceCreate();
			VisualServer.InstanceSetScenario(_instance, world.Scenario);

			VisualServer.InstanceSetBase(_instance, Mesh.GetRid());

			_transform = transform;
			_oldPos = transform.origin;
			VisualServer.InstanceSetTransform(_instance, _transform);
		}

		public Bullet(Transform transform, ShipController myShip)
		{
			MyShip = myShip;
			_instance = VisualServer.InstanceCreate();
			VisualServer.InstanceSetScenario(_instance, world.Scenario);

			VisualServer.InstanceSetBase(_instance, Mesh.GetRid());

			_transform = transform;
			_oldPos = transform.origin;
			VisualServer.InstanceSetTransform(_instance, _transform);
		}

		public void Process(float delta)
		{
			Vector3 distance = Vector3.Forward * Speed * delta;
			_transform = _transform.Translated(distance);
			VisualServer.InstanceSetTransform(_instance, _transform);

			_lifeTime += delta;
			if (_lifeTime > Timelife)
				Destroy(_transform.origin);
		}

		public void PhysicsProcess(float delta)
		{
			var spaceState = world.DirectSpaceState;
			var result = spaceState.IntersectRay(_oldPos, _transform.origin, null, 3u);
			if (result.Count > 0)
				OnCollision(result["collider"], (Vector3)result["position"]);
			else
				_oldPos = _transform.origin;
		}

		protected virtual void OnCollision(object collider, Vector3 point)
		{
			if (collider is IEntity ie)
			{
				if (ie == MyShip)
					return;
				else
				{
					if (ie.GetTeamNumber() == MyShip.PilotOwner.TeamNumber)
						return;
					else
						ie.TakeDamage(Damage);
				}
			}

			Destroy(point);
		}

		void Destroy(Vector3 point)
		{
			Free();
			BulletManager.Instance.RemoveBullet(this);
			OnDeath(point);
		}

		public void Free()
		{
			VisualServer.FreeRid(_instance);
		}

		protected virtual void OnDeath(Vector3 point)
		{
			
		}
	}
}