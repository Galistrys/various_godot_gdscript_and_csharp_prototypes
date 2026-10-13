using Godot;
using HorrorGame.Player.PlayerStates;
using HorrorGame.Managers;
using System.Collections.Generic;

namespace HorrorGame.Player
{
	public class PlayerController : KinematicBody
	{
		#region Camera

			public const float RAY_LENGTH = 2f;
			public const float MAX_DISTANCE = 3f;
			public const float MIN_DISTANCE = 1f;
			public const float CONTROLLER_MOUSE_DIFF = 100f;
			public const float CAMERA_STANDING_POS = 0.55f;
			public const float CAMERA_CROUCHING_POS = 0.2f;
			public const float CAMERA_MAX_ANGLE = 85;
			public const float CAMERA_MIN_ANGLE = -85f;
			public const float CAMERA_ACCELERATION = 10;

			public float MouseClamp { get; private set; } = 1000;

			public Spatial HeadSpatial { get; private set; }
			public Camera PlayerCamera { get; private set; }
			public RayCast InteractionRay { get; private set; }
			public float MouseSensitivity { get; private set; } = 2;
			public float CameraFov { get; private set; } = 70f;

			public Vector2 viewChange = Vector2.Zero;

			Vector3 _targetCameraPosition;
			readonly Vector3 _standCameraPosition;
			readonly Vector3 _crouchCameraPosition;

		#endregion
		
		#region Movement

			public const float GRAVITY = 20;
			public const float WALK_SPEED = 3.5f;
			public const float SPRINT_SPEED = 8;
			public const float CROUCH_SPEED = 1.5f;
			public const float ACCELERATION = 6;
			public const float DEACCELERATION = 10;
			public const float JUMP_MAGNITUDE = 7;
			public const float FLOOR_MAX_ANGLE = 45;
			public const float WEIGHT_ON_FLOOR = 4;

			public CollisionShape ColShape { get; private set; }
			public CollisionShape CrouchColShape { get; private set; }

			bool _crouching = false;
			public float verticalVelocity = 0;
			public Vector3 velocity = Vector3.Zero;
			Vector3 _direction = Vector3.Forward;
			Vector3 _targetDirection = Vector3.Forward;

			float _movementSpeed = 0;
		
		#endregion
		
		#region Interaction

			public const float PUSH_FACTOR = 0.5f;
			public const float PUSH_STRENGTH = 500;
			public const float MOVE_INTERACTABLE_SPEED = 10;

			public Position3D MovableGrabPosition { get; private set; }

		#endregion

		#region States

			public PlayerState CurrentState { get { return _currentState; } }

			Stack<PlayerState> _stateStack = new Stack<PlayerState>();
			PlayerState _currentState = null;

		#endregion
		
		#region Others

			public GameManager GameManager
			{
				get
				{
					return MainManager.Instance.GameManager;
				}
			}
		
		#endregion

		public PlayerController()
		{
			PlayerState.SetPlayerController(this);

			_standCameraPosition = new Vector3(0, CAMERA_STANDING_POS, 0);
			_crouchCameraPosition = new Vector3(0, CAMERA_CROUCHING_POS, 0);

			_targetCameraPosition = _standCameraPosition;
		}

		#region Node overrided methods
		public override void _Ready()
		{
			HeadSpatial = GetNode<Spatial>("Head");
			PlayerCamera = HeadSpatial.GetNode<Camera>("Camera");

			InteractionRay = PlayerCamera.GetNode<RayCast>("InteractionRay");
			InteractionRay.CastTo = new Vector3(0, 0, -RAY_LENGTH);
			MovableGrabPosition = PlayerCamera.GetNode<Position3D>("GrabPoint");

			ColShape = GetNode<CollisionShape>("Col");
			CrouchColShape = GetNode<CollisionShape>("CrouchCol");

			PlayerCamera.Fov = CameraFov;

			Input.SetMouseMode(Input.MouseMode.Captured);

			if (!Engine.EditorHint)
			{
				PushState(new WalkingPlayerState());
			}
		}

		public override void _Process(float delta)
		{
			if (Input.IsActionJustPressed("pause"))
			{
				GameManager.OpenPauseMenu();
				return;
			}
			else if (Input.IsActionJustPressed("inventory"))
			{
				GameManager.OpenInventory(GUI.HUD.Inventory.View.Items, null);
				return;
			}

			UpdateControllerViewChange();

			UpdateMovement(delta);
			CurrentState.Process(delta);
		}

		public override void _PhysicsProcess(float delta)
		{
			CurrentState.PhysicsUpdateLoop(delta);
			ApplyImpulsesToRigidBodies();
			Move(delta);
		}

        public override void _ExitTree()
        {
            base._ExitTree();
			PlayerState.SetPlayerController(null);
			_currentState = null;
			_stateStack.Clear();

			HeadSpatial = null;
			PlayerCamera = null;
			InteractionRay = null;
			MovableGrabPosition = null;
			ColShape = null;
			CrouchColShape = null;
        }

		public override void _Input(InputEvent inputEvent)
		{
			if (inputEvent is InputEventKey iek)
			{
				if (iek.Pressed && iek.Scancode == (int)KeyList.Escape)
				{
					// if (Input.GetMouseMode() == Input.MouseMode.Captured)
					// 	Input.SetMouseMode(Input.MouseMode.Visible);
					// else
					// 	Input.SetMouseMode(Input.MouseMode.Captured);
				}
				if (iek.Pressed && iek.Scancode == (int)KeyList.M)
				{
					Logger.Log("States in Queue: ");
					foreach (var st in _stateStack)
					{
						GD.Print(st.GetType().ToString());
					}

					MainManager.Instance.ExitGame(ExitCode.TestError);
					return;
				}
			}
			else if (inputEvent is InputEventMouseMotion iemm)
            {
                // Vector2 tempRel = iemm.Relative;
                // tempRel.x = Mathf.Clamp(tempRel.x, -_player.MouseClamp, _player.MouseClamp);
                // tempRel.y = Mathf.Clamp(tempRel.y, -_player.MouseClamp, _player.MouseClamp);
                // tempRel /= _player.MouseClamp;

                // _viewChange = tempRel * _player.MouseSensitivity;

                viewChange = iemm.Relative * MouseSensitivity;
            }

			CurrentState.ProcessInput(ref inputEvent);
		}

		#endregion

		#region StatesMethods

		public void PushState(PlayerState state)
		{
			if (_stateStack.Count > 0)
				_currentState.OnPause();
			_stateStack.Push(state);
			_currentState = state;
			state.OnEnter();
		}

		public PlayerState PopState()
		{
			if (_stateStack.Count > 1)
			{
				PlayerState state = _stateStack.Pop();
				state.OnExit();

				_currentState = _stateStack.Peek();
				_currentState.OnResume();

				return state;
			}
			else if (_stateStack.Count > 0)
			{
				PlayerState state = _stateStack.Pop();
				state.OnExit();

				_currentState = null;
				return state;
			}

			Logger.Log("There are no states left.", LogLevel.Warning);
			return null;
		}

		#endregion

		#region MovementMethods

		void ApplyImpulsesToRigidBodies()
		{
			for (int i = 0; i < GetSlideCount(); i++)
			{
				KinematicCollision collision = GetSlideCollision(i);
				if (collision.Collider is RigidBody rb)
					rb.ApplyCentralImpulse(-collision.Normal * velocity.Length() * PUSH_FACTOR);
			}
		}

		public void UpdateCameraRotation(float delta)
        {
            if (Input.GetMouseMode() != Input.MouseMode.Captured)
                return;
            
            if (viewChange.LengthSquared() > 0)
            {
                float changeX = -viewChange.x * delta;
                float changeY = -viewChange.y * delta;

                // _viewChange = _viewChange.LinearInterpolate(Vector2.Zero, _player.ViewChangeSmoothing);
                viewChange = Vector2.Zero;

                Vector3 tempPlayerRot = RotationDegrees;
                Vector3 tempHeadRot = HeadSpatial.RotationDegrees;

                tempPlayerRot.y += changeX;
                tempHeadRot.x += changeY;

                tempHeadRot.x = Mathf.Clamp(tempHeadRot.x, PlayerController.CAMERA_MIN_ANGLE, PlayerController.CAMERA_MAX_ANGLE);

                RotationDegrees = tempPlayerRot;
                HeadSpatial.RotationDegrees = tempHeadRot;
            }
        }

		public void SetMovementDirection(Vector3 targetDir)
		{
			_targetDirection = targetDir;
		}

		void UpdateControllerViewChange()
        {
            float viewChangeX = Input.GetActionStrength("look_right") - Input.GetActionStrength("look_left");
            float viewChangeY = Input.GetActionStrength("look_down") - Input.GetActionStrength("look_up");

            if (viewChangeX != 0)
            {
                viewChange.x = viewChangeX * PlayerController.CONTROLLER_MOUSE_DIFF;
            }
            if (viewChangeY != 0)
            {
                viewChange.y = viewChangeY * PlayerController.CONTROLLER_MOUSE_DIFF;
            }
        }

		void UpdateMovement(float delta)
        {
			if (IsOnFloor())
			{
				if (_targetDirection.LengthSquared() > 0)
				{
					_direction = _targetDirection.Rotated(Vector3.Up, Rotation.y).Normalized();

					_targetDirection = Vector3.Zero;

					if (_crouching)
						_movementSpeed = PlayerController.CROUCH_SPEED;
					else if (Input.IsActionPressed("sprint"))
						_movementSpeed = PlayerController.SPRINT_SPEED;
					else
						_movementSpeed = PlayerController.WALK_SPEED;
				}
				else
				{
					_movementSpeed = 0;
				}

				if (Input.IsActionPressed("crouch"))
					Crouch();
				else
					Stand();
			}

			HeadSpatial.Translation = HeadSpatial.Translation.LinearInterpolate(_targetCameraPosition, delta * CAMERA_ACCELERATION);
        }

		public void Jump()
		{
			if (IsOnFloor())
			{
				Stand();
				verticalVelocity = JUMP_MAGNITUDE;
			}
		}

        public void Crouch()
		{
			if (_crouching)
				return;
			_targetCameraPosition = _crouchCameraPosition;
			ColShape.Disabled = true;
			CrouchColShape.Disabled = false;
			_crouching = true;
		}

		public void Stand()
		{
			if (_crouching)
			{
				_targetCameraPosition = _standCameraPosition;
				ColShape.Disabled = false;
				CrouchColShape.Disabled = true;
				_crouching = false;
			}
		}

		void Move(float delta)
		{
			velocity = velocity.LinearInterpolate(_direction * _movementSpeed, ACCELERATION * delta);
            MoveAndSlide(velocity + Vector3.Up * verticalVelocity - GetFloorNormal() * WEIGHT_ON_FLOOR, Vector3.Up, infiniteInertia: false);

            if (IsOnFloor())
            {
                verticalVelocity = 0;
            }
            else
            {
                verticalVelocity -= GRAVITY * delta;
            }
		}

		#endregion
	}
}
