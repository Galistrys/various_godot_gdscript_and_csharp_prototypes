using Godot;
using Godot.Collections;
using KayEth.Entities;
using KayEth.Player.PlayerStates.InteractionStates;
using KayEth.Player.PlayerStates.MovementStates;
using KayEth.Tools;
using System;

namespace KayEth.Player
{
    public class PlayerController : KinematicBody, IEntity
    {
        public const float MOUSE_CLAMP = 500;
        public const float MOUSE_MOVEMENT_MULTIPLYER = 100;
        public const float VIEW_CHANGE_SMOOTHING = 0.1f;
        public const float FOV_CHANGE_SPEED = 8;
        public const float FOV_SPRINT_MOD = 1.05f;

        public const float MAX_ANGLE = 85.0f;
        public const float MIN_ANGLE = -75.0f;

        public const float PUSH_FACTOR = 0.5f;

        public const float WALK_SPEED = 5;
        public const float SPRINT_SPEED = 8;
        public const float CROUCH_SPEED = 2;
        public const float ACCELERATION = 8;
        public const float DEACCELERATION = 10;
        public const float MID_AIR_ACCELERATION = 1f;
        public const float JUMP_STRENGTH = 7;
        public const float VELOCITY_CLAMP = 0.25f;

        public const float FLOOR_MAX_ANGLE = 45f;

        public const float FLY_SPEED = 3;
        public const float FLY_ACCELERATION = 0.4f;
        public const float FLY_DEACCELERATION = 1.6f;
        public const float FLY_ROTATION_SPEED = 0.5f;
        public const float FLY_ROTATION_ACCELERATION = 0.05f;

        public const float CROUCH_HEIGHT_DIFF = -1;
        public const float HEIGHT_CHANGE_SPEED = 10;

        public const float MAX_DISTANCE = 2.0f;
        public const float MIN_DISTANCE = 1.0f;
        public const float MOUSE_INCREASE = 50.0f;

        private MovementPlayerState _movementCurrentState = null;
        private InteractionPlayerState _interactionCurrentState = null;

        public bool InVacuum = false;
        public Vector3 velocity = Vector3.Zero;

        public Vector2 MouseSensitivity { get; private set; } = new Vector2(0.5f, 0.5f);
        public float CameraFov { get; private set; } = 70;
        public Vector3 GravityVector { get; private set; } = Vector3.Down * 9.81f;

        public Camera PlayerCamera { get; private set; }
        public Spatial HeadSpatial { get; private set; }

        public float InitialHeadY { get; private set; }
        public Vector3 HeadStartPosition { get; private set; }

        public CollisionShape ColShape { get; private set; }
        public CollisionShape CrouchColShape { get; private set; }
        public CollisionShape FlyingColShape { get; private set; }

        public Position3D MovableGrabPosition { get; private set; }

        public Spatial Model { get; private set; }

        public string EntityName => "Player";

        public override void _Ready()
        {
            HeadSpatial = GetNode<Spatial>("Head");
            PlayerCamera = GetNode<Camera>("Head/PlayerCamera");

            ColShape = GetNode<CollisionShape>("ColShape");
            CrouchColShape = GetNode<CollisionShape>("CrouchColShape");
            FlyingColShape = GetNode<CollisionShape>("FlyingColShape");

            MovableGrabPosition = GetNode<Position3D>("Head/PlayerCamera/Position3D");

            Model = GetNode<Spatial>("Model");

            HeadStartPosition = HeadSpatial.Translation;
            InitialHeadY = HeadSpatial.Transform.origin.y;
            PlayerCamera.Fov = CameraFov;

            Input.SetMouseMode(Input.MouseMode.Captured);

            if (!Engine.EditorHint)
            {
                _interactionCurrentState = new IdleState(this);
                _interactionCurrentState.OnEnter();
                _movementCurrentState = new WalkState(this);
                _movementCurrentState.OnEnter();
            }
        }

        public override void _Process(float delta)
        {
            _interactionCurrentState.Process(delta);
            if (_interactionCurrentState.CanMovementRecieveInput())
                _movementCurrentState.Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            _interactionCurrentState.PhysicsProcess(delta);
            _movementCurrentState.PhysicsProcess(delta);
            ApplyImpulsesToRigidBodies();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventKey iek)
            {
                if (iek.Pressed && iek.Scancode == (int)KeyList.Escape)
                {
                    if (Input.GetMouseMode() == Input.MouseMode.Captured)
                    {
                        Input.SetMouseMode(Input.MouseMode.Visible);
                    }
                    else
                    {
                        Input.SetMouseMode(Input.MouseMode.Captured);
                    }
                }
                if (iek.Pressed && iek.Scancode == (int)KeyList.M)
                {

                }
            }

            _interactionCurrentState.ProcessInput(@event);
            if (_interactionCurrentState.CanMovementRecieveInput())
                _movementCurrentState.ProcessInput(@event);
        }

        public void ZeroViewChange()
        {
            _movementCurrentState?.ZeroViewChange();
        }

        public Vector2 GetViewChange()
        {
            return _movementCurrentState != null ? _movementCurrentState.ViewChange : Vector2.Zero;
        }

        public Godot.Collections.Dictionary GetRaycastData(uint mask)
        {
            var space_state = GetWorld().DirectSpaceState;
            return space_state.IntersectRay(
                PlayerCamera.GlobalTransform.origin,
                PlayerCamera.GlobalTransform.origin - PlayerCamera.GlobalTransform.basis.z * MAX_DISTANCE,
                new Godot.Collections.Array() { this },
                mask,
                true,
                false
            );
        }

        void ApplyImpulsesToRigidBodies()
        {
            for (int i = 0; i < GetSlideCount(); i++)
            {
                var kc = GetSlideCollision(i);
                if (kc.Collider is RigidBody rb)
                {
                    rb.ApplyCentralImpulse(-kc.Normal * velocity.Length() * PUSH_FACTOR);
                }
            }
        }

        public void ChangeInteractionState(InteractionPlayerState state)
        {
            // GD.Print($"Changing interaction state from {(_interactionCurrentState != null ? _interactionCurrentState.GetType().Name : "Null")} to {state.GetType().Name}");

            Logger.DebugS("Changing interaction state from", _interactionCurrentState.GetType().Name, "to", state.GetType().Name);

            if (_interactionCurrentState != null)
            {
                _interactionCurrentState.OnExit();
                _interactionCurrentState.Destroy();
            }
            _interactionCurrentState = state;
            _interactionCurrentState.OnEnter();
        }

        public void ChangeMovementState(MovementPlayerState state)
        {
            // GD.Print($"Changing movement state from {(_movementCurrentState != null ? _movementCurrentState.GetType().Name : "Null")} to {state.GetType().Name}");

            Logger.DebugS("Changing movement state from", _movementCurrentState.GetType().Name, "to", state.GetType().Name);

            if (_movementCurrentState != null)
            {
                _movementCurrentState.OnExit();
                _movementCurrentState.Destroy();
            }
            _movementCurrentState = state;
            _movementCurrentState.OnEnter();
        }

        public Dictionary SaveToDictionary()
        {
            throw new NotImplementedException();
        }

        public void LoadFromDictionary(Dictionary data)
        {
            throw new NotImplementedException();
        }
    }
}
