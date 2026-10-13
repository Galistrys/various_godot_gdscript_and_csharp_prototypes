using Godot;
using SpaceShooter.Entities.Ships;
using SpaceShooter.Managers;
using System;

namespace SpaceShooter.Weapons
{
    public class LaserBullet : Bullet
    {
        public static readonly float speed = 200;
        public override float Speed { get { return speed; } }
        public override float Timelife { get { return 2; } }
        public override float Damage { get { return 20; } }
		static Mesh _mesh = ResourceLoader.Load<Mesh>("res://Resources/Weapons/LaserGun/LaserBullet.mesh");
        protected override Mesh Mesh { get { return _mesh; } }
        static SpatialMaterial _mat = ResourceLoader.Load<SpatialMaterial>("res://Resources/Weapons/LaserGun/LaserBullet.material");

        // public const string LASER_HIT_PATH = "res://Resources/Weapons/LaserGun/LaserHit.res";
		// static PackedScene _laserHit = ResourceLoader.Load<PackedScene>(LASER_HIT_PATH);

        public LaserBullet(Transform transform) : base(transform)
        {
            
        }

        public LaserBullet(Transform transform, ShipController myShip) : base(transform, myShip)
        {
            VisualServer.InstanceGeometrySetMaterialOverride(_instance, Bullet.GlowingMaterials[MyShip.PilotOwner.TeamNumber].GetRid());
        }

        protected override void OnDeath(Vector3 pos)
        {
			// var lh = _laserHit.Instance() as Spatial;
            // GameManager.Instance.VFXSpatial.AddChild(lh);
			// lh.Translation = pos;

            BulletHitManager.Instance.AddHit(new LaserHit(new Transform(Basis.Identity, pos)));
        }
    }
}