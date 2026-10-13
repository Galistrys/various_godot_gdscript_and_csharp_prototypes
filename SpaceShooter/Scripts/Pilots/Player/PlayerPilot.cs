using Godot;
using SpaceShooter.Data;
using SpaceShooter.Entities;
using SpaceShooter.Entities.Ships;
using SpaceShooter.GUI.HUD;

namespace SpaceShooter.Pilots.Player
{
	public class PlayerPilot : Pilot
	{
		IEntity _target = null;
		public const float CONTROLLER_SPEED = 10f;

		bool _controllerMode = false;

		[Export] NodePath PlayerCameraNodePath { get; set; }
		PlayerCamera _playerCamera;

		AudioStreamPlayer _healthHitPlayer;
		AudioStreamPlayer _shieldHitPlayer;

        public override void _Ready()
        {
			base._Ready();
			_playerCamera = GetNode<PlayerCamera>(PlayerCameraNodePath);
            Input.SetMouseMode(Input.MouseMode.Captured);

			_healthHitPlayer = GetNode<AudioStreamPlayer>("HealthHitSound");
			_shieldHitPlayer = GetNode<AudioStreamPlayer>("ShieldHitSound");
        }

        public override void Init(ShipConfiguration configuration)
        {
            base.Init(configuration);

			_playerCamera.Init();
			ShipController.Connect(nameof(ShipController.OnTakeDamage), this, nameof(PlayHitSound));
        }

		void PlayHitSound(bool shield)
		{
			if (shield)
				_shieldHitPlayer.Play();
			else
				_healthHitPlayer.Play();
		}

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion iemm)
			{
				_controllerMode = false;
				HUDController.Instance.MovePointer(iemm.Relative.Normalized(), iemm.Relative.Length());
			}
			else if (@event is InputEventJoypadMotion iejm)
			{
				_controllerMode = true;
			}
        }

        public override void _Process(float delta)
        {
			if (_controllerMode)
				ControllerInput(delta);
			
			float value;
			Vector2 dir;

			HUDController.Instance.GetTurn(out dir, out value);
			float roll = Input.GetActionStrength("rotate_left") - Input.GetActionStrength("rotate_right");
			
			_shipController.SetTargetRotationSpeedValues(new Vector3(-dir.y * value, -dir.x * value, roll));

			_shipController.SetTargetMovementSpeedValues(
				new Vector3(
					Input.GetActionStrength("thrust_right") - Input.GetActionStrength("thrust_left"),
					Input.GetActionStrength("thrust_up") - Input.GetActionStrength("thrust_down"),
					Input.GetActionStrength("thrust_backward") - Input.GetActionStrength("thrust_forward")
				)
			);

			HUDController.Instance.ShipStatsControl.SetHealthAndShieldValue(
				ShipController.CurrentHealth / ShipController.MaxHealth,
				ShipController.CurrentShield / ShipController.MaxShield
			);

			if (_target != null)
			{
				Vector3 intersectionPos = ShipController.WeaponController.GetIntersectionPoint(ShipController.WeaponController.GetProjectileSpeed(true), _target.GlobalPosition, _target.Velocity);
				Vector2 screenTargetPos = _playerCamera.UnprojectPosition(_target.GlobalPosition);
				Vector2 screenIntersectionPointPos = _playerCamera.UnprojectPosition(intersectionPos);
				Vector2 screenCenter = HUDController.Instance.PointerControl.RectPosition;

				if (_playerCamera.ToLocal(_target.GlobalPosition).z < 0)
					if (
						screenIntersectionPointPos.DistanceTo(screenCenter) < HUDController.Instance.MaxPointerDistanceToCircle
						|| screenTargetPos.DistanceTo(screenCenter) < HUDController.Instance.MaxPointerDistanceToCircle
					)
						HUDController.Instance.IntersectionControl.ShowLine(screenTargetPos, screenIntersectionPointPos, _target.CurrentHealth, _target.MaxHealth, _target.CurrentShield, _target.MaxShield);
					else
						HUDController.Instance.IntersectionControl.ShowDirection((screenTargetPos - screenCenter).Normalized());
				else
					HUDController.Instance.IntersectionControl.ShowDirection(-(screenTargetPos - screenCenter).Normalized());
				
				
			}
			else
			{
				HUDController.Instance.IntersectionControl.HideIntersection();
			}
			
			if (Input.IsActionPressed("shoot_primary"))
				Shoot(true);
			if (Input.IsActionPressed("shoot_secondary"))
				Shoot(false);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

			if (Input.IsActionJustPressed("change_target"))
				ChangeTarget();
        }

		void ControllerInput(float delta)
		{
			float viewChangeX = Input.GetActionStrength("turn_right") - Input.GetActionStrength("turn_left");
            float viewChangeY = Input.GetActionStrength("turn_down") - Input.GetActionStrength("turn_up");

			HUDController.Instance.MovePointer(delta * CONTROLLER_SPEED, new Vector2(viewChangeX, viewChangeY));
		}

		public override Vector3 GetTargetPoint()
		{
			if (_target != null)
			{
				if (HUDController.Instance.IntersectionControl.IsPointerInIntersectionPoint())
					return ShipController.WeaponController.GetIntersectionPoint(ShipController.WeaponController.GetProjectileSpeed(true), _target.GlobalPosition, _target.Velocity);
			}

			return _playerCamera.GetTargetPoint();
		}

		void ChangeTarget()
		{
			if (_target != null)
			{
				(_target as Node).Disconnect("OnDeath", this, nameof(OnTargetDestroyed));
			}
			_target = _playerCamera.GetTarget();
			if (_target != null)
			{
				(_target as Node).Connect("OnDeath", this, nameof(OnTargetDestroyed));
			}
		}

		void OnTargetDestroyed(Node target)
		{
			if (_target == target)
				_target = null;
		}

        public override void OnShipDestroyed(Node node)
        {
			GetTree().ChangeScene("res://Resources/Scenes/ChooseBattle.scn");
        }
    }
}