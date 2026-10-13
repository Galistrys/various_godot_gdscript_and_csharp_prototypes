using Godot;
using SpaceShooter.Data;
using SpaceShooter.Managers;
using SpaceShooter.Pilots;
using SpaceShooter.Pilots.AI;
using System;

namespace SpaceShooter.Entities.Ships
{
    public class ShipController : KinematicBody, IEntity
    {
		public const string SHIPS_DIRECTORY = "res://Resources/Entities/Ships";
		public const string SHIPS_EXT = ".res";

		public static PackedScene ShipExplosion = ResourceLoader.Load<PackedScene>("res://Resources/Entities/ShipExplosion.res");

		public static SpatialMaterial[] ShipsMaterials { get; set; }

		[Signal]
		public delegate void OnDeath(Node node);
		[Signal]
		public delegate void OnTakeDamage(bool shield);

        public string EntityId { get; private set; }

		[Export] public float VelocityAcceleration { get; private set; }

        [Export] public float MovementAcceleration { get; private set; }
		[Export] public Vector3 MaxMovementSpeed { get; private set; }

		Vector3 _targetMovementSpeedValues = Vector3.Zero;
		Vector3 _currentMovementSpeedValues = Vector3.Zero;

		[Export] public float RotationAcceleration { get; private set; }
		[Export] public Vector3 MaxRotationSpeed { get; private set; }

		Vector3 _targetRotationSpeedValues = Vector3.Zero;
		Vector3 _currentRotationSpeedValues = Vector3.Zero;

		Vector3 _velocity = Vector3.Zero;

		public Pilot PilotOwner { get; private set; }

		public WeaponController WeaponController { get; private set; }

        public Vector3 GlobalPosition { get { return GlobalTransform.origin; } }

        public Vector3 Velocity { get { return _velocity; } }

		public RayCast TopRayCast { get; private set; }
		public RayCast BottomRayCast { get; private set; }
		public RayCast LeftRayCast { get; private set; }
		public RayCast RightRayCast { get; private set; }

        [Export] public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }

		[Export] public float MaxShield { get; private set; }
		public float CurrentShield { get; private set; }

		[Export] public float ShieldRegenSpeed { get; private set; }

		Timer _shieldRegenTimer;

        public override void _Ready()
        {
			// PilotOwner = GetParent<Pilot>();
			WeaponController = GetNode<WeaponController>("WeaponController");
			_shieldRegenTimer = GetNode<Timer>("ShieldRegenTimer");
        }

		~ShipController()
		{
			GD.Print("Destroyed: " + Name);
		}

		public void Init(Pilot pilotOwner, ShipConfiguration configuration)
		{
			PilotOwner = pilotOwner;
			if (PilotOwner is PilotAI)
			{
				TopRayCast = GetNode<RayCast>("DetectRayCast/Top");
				BottomRayCast = GetNode<RayCast>("DetectRayCast/Bottom");
				LeftRayCast = GetNode<RayCast>("DetectRayCast/Left");
				RightRayCast = GetNode<RayCast>("DetectRayCast/Right");
			}
			else
			{
				GetNode<Spatial>("DetectRayCast").QueueFree();
			}

			SetColor();

			// _maxHealth = ShipConfiguration.MaxHealthModifier;
			// _currentHealth = _maxHealth;

			MaxHealth += configuration.MaxHealthModifier;
			CurrentHealth = MaxHealth;
			MaxShield += configuration.MaxShieldModifier;
			CurrentShield = MaxShield;

			// Apply modifiers

			WeaponController.Init();
		}

        public override void _Process(float delta)
        {
			_currentRotationSpeedValues = _currentRotationSpeedValues.LinearInterpolate(_targetRotationSpeedValues, delta * RotationAcceleration);

			RotateObjectLocal(Vector3.Right, _currentRotationSpeedValues.x * delta * MaxRotationSpeed.x);
			RotateObjectLocal(Vector3.Up, _currentRotationSpeedValues.y * delta * MaxRotationSpeed.y);
			RotateObjectLocal(Vector3.Back, _currentRotationSpeedValues.z * delta * MaxRotationSpeed.z);

			_currentMovementSpeedValues = _currentMovementSpeedValues.LinearInterpolate(_targetMovementSpeedValues, delta * MovementAcceleration);

			_velocity = _velocity.MoveToward(Transform.basis.Xform(_currentMovementSpeedValues * MaxMovementSpeed), delta * VelocityAcceleration);
			_velocity = MoveAndSlide(_velocity);

			if (CurrentShield < MaxShield && _shieldRegenTimer.TimeLeft == 0)
			{
				CurrentShield += delta * ShieldRegenSpeed;
				if (CurrentShield > MaxShield)
					CurrentShield = MaxShield;
			}
        }

        public override void _PhysicsProcess(float delta)
        {
			
        }

		public void SetColor()
		{
			GetNode<MeshInstance>("Model/Model").SetSurfaceMaterial(1, ShipsMaterials[PilotOwner.TeamNumber]);
		}

		/// <summary>
		/// Sets target movement speed values
		/// </summary>
		/// <param name="movements">Must be a normalized Vector3!</param>
		public void SetTargetMovementSpeedValues(Vector3 movements)
		{
			_targetMovementSpeedValues = movements;
		}

		/// <summary>
		/// Sets target rotation speed values
		/// </summary>
		/// <param name="rotations">Must be a normalized Vector3!</param>
		public void SetTargetRotationSpeedValues(Vector3 rotations)
		{
			_targetRotationSpeedValues = rotations;
		}

        public void Shoot(bool primary)
        {
            if (primary)
				WeaponController.shootPrimary = true;
			else
				WeaponController.shootSecondary = true;
        }

        public int GetTeamNumber()
        {
            return PilotOwner.TeamNumber;
        }

        public void TakeDamage(float damage)
        {
            // CurrentHealth -= damage;

			// EmitSignal(nameof(OnTakeDamage));

			// if (CurrentHealth <= 0)
			// 	Die();

			_shieldRegenTimer.Start();

			if (CurrentShield > 0)
			{
				CurrentShield -= damage;
				EmitSignal(nameof(OnTakeDamage), true);

				if (CurrentShield < 0)
					CurrentShield = 0;
			}
			else
			{
				CurrentHealth -= damage;

				EmitSignal(nameof(OnTakeDamage), false);

				if (CurrentHealth <= 0)
					Die();
			}
        }

		public void Die()
		{
			EmitSignal(nameof(OnDeath), this);
			var exp = ShipExplosion.Instance() as Spatial;
			exp.GlobalTransform = this.GlobalTransform;
			GameManager.Instance.VFXSpatial.AddChild(exp);
			PilotOwner.SetProcess(false);
			PilotOwner.SetPhysicsProcess(false);
			QueueFree();
		}

		public Position3D GetCameraPosition()
        {
            return GetNode<Position3D>("CameraPosition");
        }

		void OnShieldRegenTimerTimeout()
		{

		}
    }
}