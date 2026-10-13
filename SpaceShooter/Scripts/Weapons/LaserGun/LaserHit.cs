using Godot;
using System;

namespace SpaceShooter.Weapons
{
    public class LaserHit : Hit
    {
        protected override Mesh Mesh { get { return _mesh; } }
        protected override SpatialMaterial Material { get { return _mat; } }

        protected override AudioStreamSample Sound { get { return _sample; } }

        protected override float TimeLife { get { return 0.5f; } }

        protected override float StartScale { get { return 0.1f; } }

        protected override float EndScale { get { return 2; } }

		static Mesh _mesh = ResourceLoader.Load<QuadMesh>("res://Resources/Weapons/LaserGun/LeserHitModel.res");
		static SpatialMaterial _mat = ResourceLoader.Load<SpatialMaterial>("res://Resources/Weapons/LaserGun/LaserHit.material");
		static AudioStreamSample _sample = ResourceLoader.Load<AudioStreamSample>("res://Assets/laser_hit.wav");

        public LaserHit(Transform transform) : base(transform)
        {
        }
    }
}