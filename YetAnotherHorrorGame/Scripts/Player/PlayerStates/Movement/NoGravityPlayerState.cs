using Godot;
using System;

namespace HorrorGame.Player.PlayerStates.Movement
{
    public class NoGravityPlayerState : MovementPlayerState
    {
		Vector3 _directionChange = Vector3.Zero;
		float _barrelChange = 0.0f;

		float _barrelRotation = 0.0f;
        bool _grounded = true;
        Vector3 _direction = Vector3.Zero;

        public NoGravityPlayerState(PlayerEntity player) : base(player)
        {
        }

        public override void OnEnter()
        {
            _player.ColShape.Disabled = true;
            _player.CrouchColShape.Disabled = true;
            _player.FlyColShape.Disabled = false;

			var temp = _player.HeadSpatial.Rotation;
			temp.x = 0;
			_player.HeadSpatial.Rotation = temp;
        }

        public override void OnExit()
        {
            
        }

        public override void PhysicsProcess(float delta)
        {
            CameraRotation(delta);
			BarrelRotation(delta);
            Fly(delta);
        }

        public override void ProcessInput(InputEvent e)
        {
            if (e is InputEventMouseMotion iemm)
            {
                var temp_rel = iemm.Relative;
                temp_rel.x = Mathf.Clamp(temp_rel.x, -PlayerEntity.MOUSE_CLAMP, PlayerEntity.MOUSE_CLAMP);
                temp_rel.y = Mathf.Clamp(temp_rel.y, -PlayerEntity.MOUSE_CLAMP, PlayerEntity.MOUSE_CLAMP);
                temp_rel /= PlayerEntity.MOUSE_CLAMP;

                _player.viewChange = temp_rel * _player.MouseSensitivity;
            }
        }

        public override void UpdateProcess(float delta)
        {
            _player.viewChange.x = Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left");
            _player.viewChange.y = Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up");

            _directionChange.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            _directionChange.y = Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward");
			_directionChange.z = Input.GetActionStrength("move_up") - Input.GetActionStrength("move_down");

			_barrelChange = Input.GetActionStrength("rotate_left") - Input.GetActionStrength("rotate_right");
        }

        void CameraRotation(float delta)
        {
            if (Input.GetMouseMode() != Input.MouseMode.Captured)
                return;
            
            if (_player.viewChange.LengthSquared() > 0)
            {
                float change_x = -_player.viewChange.x * 100 * delta;
                float change_y = -_player.viewChange.y * 100 * delta;

                _player.viewChange = _player.viewChange.LinearInterpolate(Vector2.Zero, PlayerEntity.VIEW_CHANGE_SMOOTHING);

                _player.RotateY(Mathf.Deg2Rad(change_x));
                _player.HeadSpatial.RotateX(Mathf.Deg2Rad(change_y));

                var temp_rot = _player.HeadSpatial.RotationDegrees;
                temp_rot.x = Mathf.Clamp(temp_rot.x, PlayerEntity.MIN_ANGLE, PlayerEntity.MAX_ANGLE);
                _player.HeadSpatial.RotationDegrees = temp_rot;
            }
        }

		void BarrelRotation(float delta)
		{
			_barrelRotation = Mathf.Lerp(_barrelRotation, _barrelChange * 2, PlayerEntity.FLY_ROTATION_ACCELERATION);
			_player.RotateObjectLocal(Vector3.Back, _barrelRotation * delta);
			_barrelChange = 0;
		}

        void Fly(float delta)
        {
            _direction = Vector3.Zero;
            var aim = _player.GlobalTransform.basis;

            _direction += aim.z * _directionChange.z;
            _direction += aim.x * _directionChange.x;
            _direction += aim.y * _directionChange.y;

            if (_directionChange.LengthSquared() > 1)
                _direction.Normalized();

            _directionChange = Vector3.Zero;

            var target = _direction * PlayerEntity.FLY_SPEED;

            if (target.LengthSquared() == 0)
			{
				if (_player.inVacuum)
					target = _player.velocity;
				_player.velocity = _player.velocity.LinearInterpolate(target, PlayerEntity.FLY_DEACCELERATION * delta);
			}
			else
				_player.velocity = _player.velocity.LinearInterpolate(target, PlayerEntity.FLY_ACCELERATION * delta);
			
			_player.velocity = _player.MoveAndSlide(_player.velocity);
        }
    }
}
