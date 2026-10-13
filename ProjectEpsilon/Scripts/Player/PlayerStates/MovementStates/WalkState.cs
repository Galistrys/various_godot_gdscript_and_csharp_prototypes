using Godot;
using System;

namespace KayEth.Player.PlayerStates.MovementStates
{
    public class WalkState : MovementPlayerState
    {
        Vector3 _directionChange = Vector3.Zero;

        bool _grounded = true;
        Vector3 _direction = Vector3.Zero;
        bool _canSprint = true;

        // bool _spriting = false;
        bool _crouching = false;

        //TEMP

        Vector3 _velocity = Vector3.Zero;
        Vector3 _movement = Vector3.Zero;

        //TEMP

        public WalkState(PlayerController player) : base(player)
        {
        }

        public override void OnEnter()
        {
            Player.ColShape.Disabled = false;
            Player.CrouchColShape.Disabled = true;
            Player.FlyingColShape.Disabled = true;

            Player.HeadSpatial.Rotation = Vector3.Zero;

            Player.Transform = Player.GlobalTransform.LookingAt(
                Player.GlobalTransform.origin - Player.GlobalTransform.basis.z,
                -Player.GravityVector.Normalized()
            );
        }

        public override void OnExit()
        {
            
        }

        public override void PhysicsProcess(float delta)
        {
            CameraRotation(delta);
            Walk(delta);
        }

        public override void Process(float delta)
        {
            Vector2 view_change = Vector2.Zero;
            view_change.x = Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left");
            view_change.y = Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up");
            ViewChange = view_change;

            _directionChange.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
            _directionChange.z = Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward");

            // var fps = Engine.GetFramesPerSecond();
            // var leprInterval = _direction / fps;
            // var lerpPosition = Player.GlobalTransform.origin + leprInterval;

            // if (fps > 60)
            // {
            //     Player.Model.SetAsToplevel(true);
            //     var t = Player.Model.GlobalTransform;
            //     t.origin = Player.Model.GlobalTransform.origin.LinearInterpolate(lerpPosition, 20 * delta);
            //     Player.Model.GlobalTransform = t;
            // }
            // else
            // {
            //     Player.Model.GlobalTransform = Player.GlobalTransform;
            //     Player.Model.SetAsToplevel(false);
            // }
        }

        public override void ProcessInput(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion iemm)
            {
                Vector2 temp_rel = iemm.Relative;

                // temp_rel.x = Mathf.Clamp(
                //     temp_rel.x,
                //     -PlayerController.MOUSE_CLAMP,
                //     PlayerController.MOUSE_CLAMP
                // );
                // temp_rel.y = Mathf.Clamp(
                //     temp_rel.y,
                //     -PlayerController.MOUSE_CLAMP,
                //     PlayerController.MOUSE_CLAMP
                // );
                // temp_rel /= PlayerController.MOUSE_CLAMP;

                // ViewChange = ;
                ViewChange = ViewChange.LinearInterpolate(
                    temp_rel * Player.MouseSensitivity,
                    PlayerController.VIEW_CHANGE_SMOOTHING
                );
            }
        }

        void CameraRotation(float delta)
        {
            if (Input.GetMouseMode() != Input.MouseMode.Captured)
                return;
            
            if (ViewChange.LengthSquared() > 0)
            {
                float change_x = -ViewChange.x * delta;
                float change_y = -ViewChange.y * delta;

                Player.RotateY(change_x);
                Player.HeadSpatial.RotateX(change_y);

                ViewChange = ViewChange.LinearInterpolate(
                    Vector2.Zero,
                    PlayerController.VIEW_CHANGE_SMOOTHING
                );
                
                var temp_rot = Player.HeadSpatial.RotationDegrees;
                temp_rot.x = Mathf.Clamp(
                    temp_rot.x,
                    PlayerController.MIN_ANGLE,
                    PlayerController.MAX_ANGLE
                );
                Player.HeadSpatial.RotationDegrees = temp_rot;
            }
        }

        void NewWalk(float delta)
        {
            
        }

        void Walk(float delta)
        {
            _direction = Vector3.Zero;
            Basis aim = Player.GlobalTransform.basis;

            Vector3 snap = Vector3.Zero;
            _grounded = Player.IsOnFloor();

            _canSprint = !(_directionChange.z > 0);

            _direction += aim.z * _directionChange.z;
            _direction += aim.x * _directionChange.x;
            _direction.y = 0;

            if (_directionChange.LengthSquared() > 1)
                _direction = _direction.Normalized();

            _directionChange = Vector3.Zero;

            float speed = 0;

            if (Input.IsActionPressed("crouch") && _grounded)
            {
                speed = PlayerController.CROUCH_SPEED;
                if (Player.PlayerCamera.Fov != Player.CameraFov)
                {
                    Player.PlayerCamera.Fov = Mathf.Lerp(
                        Player.PlayerCamera.Fov,
                        Player.CameraFov,
                        delta * PlayerController.FOV_CHANGE_SPEED
                    );
                }

                // _spriting = false;
                _crouching = true;

                Player.ColShape.Disabled = true;
                Player.CrouchColShape.Disabled = false;
            }
            else if (!_crouching && _canSprint && Input.IsActionPressed("sprint"))
            {
                speed = PlayerController.SPRINT_SPEED;
                Player.PlayerCamera.Fov = Mathf.Lerp(
                    Player.PlayerCamera.Fov,
                    Player.CameraFov * PlayerController.FOV_SPRINT_MOD,
                    delta * PlayerController.FOV_CHANGE_SPEED
                );

                var target_fov = Player.CameraFov * PlayerController.FOV_SPRINT_MOD;
                if (Player.PlayerCamera.Fov != target_fov)
                {
                    Player.PlayerCamera.Fov = Mathf.Lerp(
                        Player.PlayerCamera.Fov,
                        target_fov,
                        delta * PlayerController.FOV_CHANGE_SPEED
                    );
                }

                // _spriting = true;
                _crouching = false;

                Player.ColShape.Disabled = false;
                Player.CrouchColShape.Disabled = true;
            }
            else
            {
                speed = PlayerController.WALK_SPEED;
                if (Player.PlayerCamera.Fov != Player.CameraFov)
                {
                    Player.PlayerCamera.Fov = Mathf.Lerp(
                        Player.PlayerCamera.Fov,
                        Player.CameraFov,
                        delta * PlayerController.FOV_CHANGE_SPEED
                    );
                }

                // _spriting = false;
                _crouching = false;

                Player.ColShape.Disabled = false;
                Player.CrouchColShape.Disabled = true;
            }

            if (_crouching)
            {
                var temp_transform = Player.HeadSpatial.Transform;
                temp_transform.origin.y = Mathf.Lerp(
                    temp_transform.origin.y,
                    Player.InitialHeadY + PlayerController.CROUCH_HEIGHT_DIFF,
                    delta * PlayerController.HEIGHT_CHANGE_SPEED
                );
                Player.HeadSpatial.Transform = temp_transform;
            }
            else if (Player.HeadSpatial.Transform.origin.y != Player.InitialHeadY)
            {
                var temp_transform = Player.HeadSpatial.Transform;
                temp_transform.origin.y = Mathf.Lerp(
                    temp_transform.origin.y,
                    Player.InitialHeadY,
                    delta * PlayerController.HEIGHT_CHANGE_SPEED
                );
                Player.HeadSpatial.Transform = temp_transform;
            }

            if (_grounded)
            {
                
                // if (Input.IsActionJustPressed("jump"))
                // {
                //     snap = Vector3.Zero;
                //     Player.velocity.y = PlayerController.JUMP_STRENGTH;
                // }
                // else
                // {
                //     snap = Player.GravityVector.Normalized();
                // }

                snap = -Player.GetFloorNormal();
                Player.velocity.y = 0;
            }
            else
            {
                // snap = Player.GravityVector.Normalized();
                snap = Vector3.Zero;
                Player.velocity.y += -Player.GravityVector.Length() * 2 * delta;
            }

            // Player.velocity.y += -Player.GravityVector.Length() * 2 * delta;

            if (_grounded && Input.IsActionJustPressed("jump"))
            {
                snap = Vector3.Zero;
                Player.velocity.y = PlayerController.JUMP_STRENGTH;
            }

            Vector3 temp_vel = Player.velocity;
            temp_vel.y = 0;
            Vector3 target = _direction * speed;

            float temp_accel = 0;
            if (!_grounded)
                temp_accel = PlayerController.MID_AIR_ACCELERATION;
            else if (_direction.Dot(temp_vel) > 0)
                temp_accel = PlayerController.ACCELERATION;
            else
                temp_accel = PlayerController.DEACCELERATION;

            temp_vel = temp_vel.LinearInterpolate(target, temp_accel * delta);
            Player.velocity.x = temp_vel.x;
            Player.velocity.z = temp_vel.z;

            if (_direction.Dot(Player.velocity) == 0)
            {
                if (Player.velocity.x < PlayerController.VELOCITY_CLAMP && Player.velocity.x > -PlayerController.VELOCITY_CLAMP)
                    Player.velocity.x = 0;
                if (Player.velocity.z < PlayerController.VELOCITY_CLAMP && Player.velocity.z > -PlayerController.VELOCITY_CLAMP)
                    Player.velocity.z = 0;
            }

            Player.velocity.y = Player.MoveAndSlideWithSnap(
                Player.velocity,
                snap,
                -Player.GravityVector.Normalized(),
                true,
                4,
                Mathf.Deg2Rad(PlayerController.MAX_ANGLE),
                false
            ).y;

            // Player.velocity = Player.MoveAndSlideWithSnap(
            //     Player.velocity,
            //     snap,
            //     -Player.GravityVector.Normalized(),
            //     true,
            //     4,
            //     Mathf.Deg2Rad(PlayerController.MAX_ANGLE),
            //     false
            // );
        }
    }
}
