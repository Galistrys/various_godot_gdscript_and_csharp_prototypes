using Godot;
using SpaceShooter.Weapons;
using System;
using System.Collections.Generic;

namespace SpaceShooter.Managers
{
	public class BulletHitManager : Spatial
	{
		public static BulletHitManager Instance { get; private set; }

		int _hitCount = 0;
		List<Hit> _hits = new List<Hit>();

		int _processI = 0;

		public BulletHitManager()
		{
			Instance = this;
		}

        public override void _Ready()
        {
			Hit.world = GetWorld();
        }

        public override void _Process(float delta)
        {
            for (_processI = 0; _processI < _hitCount; _processI++)
			{
				_hits[_processI].Process(delta);
			}
        }

        public override void _ExitTree()
        {
			foreach (var hit in _hits)
			{
				hit.Free();
			}
			Instance = null;
        }

		public void AddHit(Hit hit)
		{
			_hits.Add(hit);
			_processI++;
			_hitCount++;
		}

		public void RemoveHit(Hit hit)
		{
			_hits.Remove(hit);
			_processI--;
			_hitCount--;
		}
	}
}