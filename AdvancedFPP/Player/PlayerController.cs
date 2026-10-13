using Godot;
using System;

using SpaceAdventure.Player.PlayerStates.Movement;
using SpaceAdventure.Player.PlayerStates.Interaction;
using SpaceAdventure.Player.Tools;

namespace SpaceAdventure.Player
{
    public class PlayerController : KinematicBody
    {
        public const float MOUSE_CLAMP = 500.0f;
        public const float VIEW_CHANGE_SMOOTHING = 0.4f;
        
        public const float CAMERA_NORMAL_FOV = 70.0f;
        public const float CAMERA_SPRINT_FOV = 80.0f;

        public const float MAX_ANGLE = 85f * Mathf.Pi / 180.0f;
        public const float MIN_ANGLE = -80f * Mathf.Pi / 180.0f;

        public const float PUSH_FACTOR = 0.5f;

        public const float WALK_SPEED = 2f;
        public const float SPRINT_SPEED = 6.0f;
        public const float CROUCH_SPEED = 0.75f;
        public const float ACCELERATION = 5.0f;
        public const float DEACCELERATION = 10.0f;
        public const float JUMP_STRENGTH = 5.0f;
        public const float JUMP_ACCELERATION = 1.0f;
        public const float HEIGHT_CHANGE_SPEED = 10.0f;
        
        public const float FLY_SPEED = 2.0f;
        public const float FLY_ACCELERATION = 0.75f;
        public const float FLY_DEACCELERATION = 1.0f;
        public const float FLY_ROTATION_SPEED = 0.5f;
        public const float FLY_ROTATION_ACCELERATION = 0.5f;
        public const float FLY_ROTATION_DEACCELERATION = 5f;
        
        public const float FLOOR_MAX_ANGLE = 45.0f;

        public const float MOVE_INTERACTABLE_SPEED = 10.0f;

        public Vector3 velocity = Vector3.Zero;
        public Vector2 ViewChange { get; private set; } = Vector2.Zero;

        public Spatial HeadSpatial { get; private set; }
        public Camera PlayerCamera { get; private set; }
        public RayCast InteractionRayCast { get; private set; }
        public Position3D MovableTargetPosition { get; private set; }

        public Position3D HeadStandPos { get; private set; }
        public Position3D HeadCrouchPos { get; private set; }
        public Position3D HeadFlyPos { get; private set; }

        public RayCast DownRayCast { get; private set; }
        public RayCast CrouchRayCast { get; private set; }

        public CollisionShape StandColShape { get; private set; }
        public CollisionShape CrouchColShape { get; private set; }
        public CollisionShape FlyColShape { get; private set; }

        public Pistol PistolTool { get; private set; }

        public Vector2 MouseSensitivity { get; set; } = new Vector2(0.1f, 0.1f);

        public float GravityAcceleration { get; set; } = 9.8f;

        bool _joypadMode = false;

        MovementPlayerState _movementState = null;
        InteractionPlayerState _interactionState = null;
        
        public PlayerController()
        {
            
        }

        public override void _Ready()
        {
            HeadSpatial = GetNode<Spatial>("Head");
            PlayerCamera = GetNode<Camera>("Head/PlayerCamera");
            InteractionRayCast = GetNode<RayCast>("Head/InteractionRayCast");
            MovableTargetPosition = GetNode<Position3D>("Head/MovableTargetPosition");
            PistolTool = GetNode<Pistol>("Head/Pistol");

            HeadStandPos = GetNode<Position3D>("HeadPos/StandHeadPos");
            HeadCrouchPos = GetNode<Position3D>("HeadPos/CrouchHeadPos");
            HeadFlyPos = GetNode<Position3D>("HeadPos/FlyHeadPos");

            DownRayCast = GetNode<RayCast>("RayCasts/DownRayCast");
            CrouchRayCast = GetNode<RayCast>("RayCasts/CrouchRayCast");

            StandColShape = GetNode<CollisionShape>("StandingCol");
            CrouchColShape = GetNode<CollisionShape>("CrouchCol");
            FlyColShape = GetNode<CollisionShape>("FlyCol");

            PlayerCamera.Fov = CAMERA_NORMAL_FOV;

            Input.MouseMode = Input.MouseModeEnum.Captured;

            if (!Engine.EditorHint)
            {
                _interactionState = new IdlePlayerState(this);
                // _interactionState = new PistolPlayerState(this);
                _interactionState.OnEnter();

                _movementState = new WalkPlayerState(this);
                _movementState.OnEnter();
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion iemm)
            {
                _joypadMode = false;
                ViewChange += iemm.Relative * MouseSensitivity;
            }
            else if (@event is InputEventKey iek)
            {
                _joypadMode = false;
                
                if (iek.Pressed && iek.Scancode == (int)KeyList.Escape)
                {
                    if (Input.MouseMode == Input.MouseModeEnum.Captured)
                        Input.MouseMode = Input.MouseModeEnum.Visible;
                    else
                        Input.MouseMode = Input.MouseModeEnum.Captured;
                }
                else if (iek.Pressed && iek.Scancode == (int)KeyList.P)
                {
                    if (_movementState is WalkPlayerState)
                    {
                        ChangeMovementState(new FlyPlayerState(this));
                    }
                    else
                    {
                        ChangeMovementState(new WalkPlayerState(this));
                    }
                }
                else if (iek.Pressed && iek.Scancode == (int)KeyList.F)
                {
                    if (_interactionState is IdlePlayerState)
                    {
                        ChangeInteractionState(new PistolPlayerState(this));
                    }
                    // else
                    // {
                    //     ChangeInteractionState(new IdlePlayerState(this));
                    // }
                }
            }
            else if (@event is InputEventJoypadButton)
            {
                _joypadMode = true;
            }
            else if (@event is InputEventJoypadMotion)
            {
                _joypadMode = true;
            }

            _interactionState.ProcessInput(@event);
            // if (_interactionState.CanMovementRecieveInput())
            //     _movementState.ProcessInput(@event);
        }

        public override void _Process(float delta)
        {
            if (_joypadMode)
                ProcessJoypadMovement();
            
            _interactionState.Process(delta);

            if (_interactionState.CanRotationRecieveInput())
                _movementState.UpdateRotationInputs();
            else
                _movementState.ZeroRotationInputs();
            
            if (_interactionState.CanMovementRecieveInput())
                _movementState.UpdateMovementInputs();
            else
                _movementState.ZeroRotationInputs();

            _movementState.Process(delta);

            ViewChange = Vector2.Zero;
        }

        public override void _PhysicsProcess(float delta)
        {
            _interactionState.PhysicsProcess(delta);
            _movementState.PhysicsProcess(delta);

            ApplyImpulsesToRigidBodies();
        }

        public Vector3 GetInputDirection()
        {
            return Transform.basis.Xform(new Vector3(
                Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
                Input.GetActionStrength("move_up") - Input.GetActionStrength("move_down"),
                Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward")
            ));
        }

        void ProcessJoypadMovement()
        {
            ViewChange = new Vector2(
                Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left"),
                Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up")
            );
            if (ViewChange.LengthSquared() > 1)
                ViewChange = ViewChange.Normalized();
            ViewChange *= 50;
        }

        void ApplyImpulsesToRigidBodies()
        {
            for (int i = 0; i < GetSlideCount(); i++)
            {
                KinematicCollision collision = GetSlideCollision(i);
                if (collision.Collider is RigidBody rb)
                    rb.ApplyCentralImpulse(-collision.Normal * velocity.Length() * PUSH_FACTOR);
            }
        }

        public void EnableStandColShape()
        {
            StandColShape.Disabled = false;
            CrouchColShape.Disabled = true;
            FlyColShape.Disabled = true;

            // ???
            // HeadSpatial.Translation = HeadSpatial.Translation.LinearInterpolate(HeadStandPos.Translation, VIEW_CHANGE_SMOOTHING);
        }

        public void EnableCrouchColShape()
        {
            StandColShape.Disabled = true;
            CrouchColShape.Disabled = false;
            FlyColShape.Disabled = true;
        }

        public void EnableFlyColShape()
        {
            StandColShape.Disabled = true;
            CrouchColShape.Disabled = true;
            FlyColShape.Disabled = false;
        }

        public void ChangeMovementState(MovementPlayerState newState)
        {
            _movementState.OnExit();
            _movementState = newState;
            _movementState.OnEnter();
        }

        public void ChangeInteractionState(InteractionPlayerState newState)
        {
            _interactionState.OnExit();
            _interactionState = newState;
            _interactionState.OnEnter();
        }

        public void ChangeToIdleInteractionState()
        {
            ChangeInteractionState(new IdlePlayerState(this));
        }
    }
}