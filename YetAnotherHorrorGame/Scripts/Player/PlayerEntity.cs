using Godot;
using System;
using HorrorGame.Player.PlayerStates.Interaction;
using HorrorGame.Player.PlayerStates.Movement;
using Godot.Collections;

namespace HorrorGame.Player
{
    public class PlayerEntity : KinematicBody
    {
        public const float MOUSE_CLAMP = 500.0f;
        public const float VIEW_CHANGE_SMOOTHING = 0.4f;
        public const float CAMERA_FOV = 70.0f;

        public const float MAX_ANGLE = 85.0f;
        public const float MIN_ANGLE = -75.0f;

        public const float PUSH_FACTOR = 0.5f;

        public const float VELOCITY_CAP = 0.25f;
        public const float WALK_SPEED = 5.0f;
        public const float SPRINT_SPEED = 8.0f;
        public const float CROUCH_SPEED = 2.0f;
        public const float ACCELERATION = 8.0f;
        public const float DEACCELERATION = 10.0f;
        public const float MID_AIR_ACCEL = 0.75f;
        public const float JUMP_STRENGTH = 10.0f;
        public const float CROUCH_HEIGHT_DIFF = -1.0f;
        public const float HEIGHT_CHANGE_SPEED = 10.0f;

        public const float FLY_SPEED = 3.0f;
        public const float FLY_ACCELERATION = 0.4f;
        public const float FLY_DEACCELERATION = 1.6f;
        public const float FLY_ROTATION_SPEED = 0.5f;
        public const float FLY_ROTATION_ACCELERATION = 0.05f;
        public const float FLOOR_MAX_ANGLE = 45.0f;

        public const float MAX_DISTANCE = 2.0f;
        public const float MIN_DISTANCE = 1.0f;

        public const float MOVE_INTERACTABLE_SPEED = 10.0f;
        public const float MOUSE_INCREASE = 50.0f;

        public Vector3 gravityVector = Vector3.Down * 9.81f;

        public Vector3 velocity = Vector3.Zero;
        public Vector2 viewChange = Vector2.Zero;

        public float InitialHeadY { get; private set; }
        public bool inVacuum = false;

        MovementPlayerState _movementCurrentState = null;
        InteractionPlayerState _interactionCurrentState = null;

        Vector3 _headStartPosition;

        public Spatial HeadSpatial { get; private set; }
        public Camera PlayerCamera { get; private set; }
        public Position3D MovablePosition { get; private set; }

        public CollisionShape ColShape { get; private set; }
        public CollisionShape CrouchColShape { get; private set; }
        public CollisionShape FlyColShape { get; private set; }

        public Vector2 MouseSensitivity { get; set; } = new Vector2(10, 10);

        public override void _Ready()
        {
            HeadSpatial = GetNode<Spatial>("Head");
            PlayerCamera = GetNode<Camera>("Head/PlayerCamera");
            MovablePosition = GetNode<Position3D>("Head/PlayerCamera/Position3D");

            ColShape = GetNode<CollisionShape>("ColShape");
            CrouchColShape = GetNode<CollisionShape>("CrouchColShape");
            FlyColShape = GetNode<CollisionShape>("FlyingColShape");

            _headStartPosition = HeadSpatial.Translation;

            InitialHeadY = HeadSpatial.Transform.origin.y;
            PlayerCamera.Fov = CAMERA_FOV;

            Input.SetMouseMode(Input.MouseMode.Captured);

            if (!Engine.EditorHint)
            {
                ChangeInteractionState(new IdlePlayerState(this));
                ChangeMovementState(new WalkPlayerState(this));
            }
        }

        public override void _Process(float delta)
        {
            _interactionCurrentState.UpdateProcess(delta);
            if (_interactionCurrentState.CanMovementRecieveInput())
                _movementCurrentState.UpdateProcess(delta);
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
                        Input.SetMouseMode(Input.MouseMode.Visible);
                    else
                        Input.SetMouseMode(Input.MouseMode.Captured);
                }
            }

            _interactionCurrentState.ProcessInput(@event);
            if (_interactionCurrentState.CanMovementRecieveInput())
                _movementCurrentState.ProcessInput(@event);
        }

        public void ChangeInteractionState(InteractionPlayerState state)
        {
            Logger.Log($"Changing interaction state from {(_interactionCurrentState == null ? "Null" : _interactionCurrentState.GetType().Name)} to {state.GetType().Name}");
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
            Logger.Log($"Changing movement state from {(_movementCurrentState == null ? "Null" : _movementCurrentState.GetType().Name)} to {state.GetType().Name}");
            if (_movementCurrentState != null)
            {
                _movementCurrentState.OnExit();
                _movementCurrentState.Destroy();
            }
            _movementCurrentState = state;
            _movementCurrentState.OnEnter();
        }

        public Dictionary GetInteractionData(uint mask)
        {
            PhysicsDirectSpaceState space_state = GetWorld().DirectSpaceState;
            return space_state.IntersectRay(PlayerCamera.GlobalTransform.origin, PlayerCamera.GlobalTransform.origin - PlayerCamera.GlobalTransform.basis.z * MAX_DISTANCE, new Godot.Collections.Array() { this }, mask);
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
    }
}
