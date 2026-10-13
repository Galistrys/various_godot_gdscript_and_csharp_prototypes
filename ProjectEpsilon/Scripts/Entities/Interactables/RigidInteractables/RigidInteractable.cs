using Godot;
using Godot.Collections;
using KayEth.Items;
using KayEth.Player;
using System;

namespace KayEth.Entities.Interactables.Rigid
{
    public abstract class RigidInteractable : RigidBody, IInteractable
    {
        public const string TRANSLATION_KEY = "translation";
        public const string ROTATION_KEY = "rotation";
        public const string VELOCITY_KEY = "velocity";
        public const string ANGULAR_VELOCITY_KEY = "angular_velocity";

        public const int CONTACTS_REPORTED_COUNT = 4;

        [Export]
        public string EntityName { get; private set; }

        public abstract void OnInteraction(PlayerController player);
        public abstract bool UseItem(Item item);

        Vector3 _tempVel = Vector3.Zero;

        public virtual Dictionary SaveToDictionary()
        {
            return new Dictionary() {
                {
                    TRANSLATION_KEY,
                    GD.Var2Str(this.Translation)
                },
                {
                    ROTATION_KEY,
                    GD.Var2Str(this.Rotation)
                },
                {
                    VELOCITY_KEY,
                    GD.Var2Str(this.LinearVelocity)
                },
                {
                    ANGULAR_VELOCITY_KEY,
                    GD.Var2Str(this.AngularVelocity)
                }
            };
        }

        public virtual void LoadFromDictionary(Dictionary data)
        {
            this.Translation = (Vector3)GD.Str2Var(data[TRANSLATION_KEY] as string);
            this.Rotation = (Vector3)GD.Str2Var(data[ROTATION_KEY] as string);
            this.LinearVelocity = (Vector3)GD.Str2Var(data[VELOCITY_KEY] as string);
            this.AngularVelocity = (Vector3)GD.Str2Var(data[ANGULAR_VELOCITY_KEY] as string);
        }

        public virtual void OnHit(float force)
        {

        }

        public override void _Ready()
        {
            ContactMonitor = true;
            ContactsReported = CONTACTS_REPORTED_COUNT;
            Connect("body_entered", this, nameof(OnBodyEntered));
        }

        public override void _Process(float delta)
        {
            _tempVel = LinearVelocity;
        }

        public void OnBodyEntered(Node body)
        {
            OnHit(Mathf.Pow(_tempVel.Length(), 2.0f) * Weight);
        }
    }
}
