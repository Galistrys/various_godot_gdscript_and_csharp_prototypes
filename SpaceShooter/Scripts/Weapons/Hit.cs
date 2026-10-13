using Godot;
using SpaceShooter.Managers;
using System;

namespace SpaceShooter.Weapons
{
	public abstract class Hit
	{
		public static World world;

		protected RID _instance;

		protected abstract Mesh Mesh { get; }
		protected abstract SpatialMaterial Material { get; }
		protected abstract AudioStreamSample Sound { get; }
		protected abstract float TimeLife { get; }
		protected abstract float StartScale { get; }
		protected abstract float EndScale { get; }

		private Vector3 _position;
		private float _lifeTime = 0;

		public Hit(Transform transform)
		{
			_instance = VisualServer.InstanceCreate();
			VisualServer.InstanceSetScenario(_instance, world.Scenario);

			VisualServer.InstanceSetBase(_instance, Mesh.GetRid());

			_position = transform.origin;
			VisualServer.InstanceSetTransform(_instance, new Transform(Basis.Identity, _position));

			VisualServer.InstanceGeometrySetMaterialOverride(_instance, Material.GetRid());
		}

		public void Process(float delta)
		{
			float step = (EndScale - StartScale) * _lifeTime + StartScale;
			Transform temp = new Transform(
				new Basis(
					new Vector3(step, 0, 0),
					Vector3.Up,
					Vector3.Back
				),
				_position
			);
			VisualServer.InstanceSetTransform(_instance, temp);

			_lifeTime += delta;
			if (_lifeTime > TimeLife)
				Destroy();
		}

		void Destroy()
		{
			BulletHitManager.Instance.RemoveHit(this);
			Free();
		}

		public void Free()
		{
			VisualServer.FreeRid(_instance);
		}
	}
}