using Godot;
using System;

namespace SpaceShooter.Weapons
{
    public class LaserGun : Weapon
    {
        public override float RateOfFire { get; protected set; } = 8;
        public override float ProjectileSpeed { get { return LaserBullet.speed; } }

        public override Bullet CreateBullet(Transform transform)
        {
            return new LaserBullet(transform, WeaponController.MyShip);
        }
    }
}