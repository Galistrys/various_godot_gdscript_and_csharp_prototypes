using Godot;
using System;

namespace SpaceShooter.GUI.HUD
{
	public class ShipStatsControl : CenterContainer
	{
		[Export] float _minVal = 0;
		[Export] float _maxVal = 0;

		TextureProgress _healthBar;
		TextureProgress _shieldBar;

		float _valDiff;

        public override void _Ready()
        {
            base._Ready();

			_healthBar = GetNode<TextureProgress>("BigCircle/HealthBar");
			_shieldBar = GetNode<TextureProgress>("BigCircle/ShieldBar");

			_valDiff = _maxVal - _minVal;
        }
		
		/// <summary>
		/// Sets health and shield shield value
		/// </summary>
		/// <param name="health">Must be between 0 and 1</param>
		/// <param name="shield">Must be between 0 and 1</param>
		public void SetHealthAndShieldValue(float health, float shield)
		{
			_healthBar.Value = _minVal + _valDiff * health;
			_shieldBar.Value = _minVal + _valDiff * shield;
		}
	}
}