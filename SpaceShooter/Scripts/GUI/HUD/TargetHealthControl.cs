using Godot;
using System;

namespace SpaceShooter.GUI.HUD
{
	public class TargetHealthControl : Control
	{
		ProgressBar _healthBar;
		ProgressBar _shieldBar;

        public override void _Ready()
        {
            base._Ready();

			_healthBar = GetNode<ProgressBar>("HealthBar");
			_shieldBar = GetNode<ProgressBar>("ShieldBar");
        }

		public void SetHealthAndShieldValue(float healthVal, float healthMax, float shieldVal, float shieldMax)
		{
			_healthBar.Value = healthVal;
			_healthBar.MaxValue = healthMax;
			_shieldBar.Value = shieldVal;
			_shieldBar.MaxValue = shieldMax;
		}
	}
}