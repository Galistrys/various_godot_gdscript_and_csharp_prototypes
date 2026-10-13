using Godot;
using Godot.Collections;
using SpaceAdventure.Player;
using SpaceAdventure.Player.PlayerStates.Interaction;
using System;

namespace SpaceAdventure.Entites.Interactables.Grabbable
{
    public class GrabbableRigidBodyEntity : RigidBody, IEntity, IGrabbable, IMovable, IRotatable
    {
        public GrabbableRigidBodyEntity()
        {
            
        }

        [Export]
        public string InteractableName { get; private set; }

        public void SetNewAngularVelocity(Vector3 angularVelocity)
        {
            AngularVelocity = angularVelocity;
        }

        public void SetNewLinearVelocity(Vector3 linearVelocity)
        {
            LinearVelocity = linearVelocity;
        }

        public void AddCentralImpulse(Vector3 centralImpulse)
        {
            ApplyCentralImpulse(centralImpulse * this.Weight);
        }

        public Vector3 GetGlobalPosition()
        {
            return GlobalTranslation;
        }

        public bool LoadFromDictionary(Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public InteractionPlayerState OnInteraction(PlayerController playerController)
        {
            return new GrabbingPlayerState(playerController, this);
        }

        public Dictionary<string, object> SaveToDictionary()
        {
            throw new NotImplementedException();
        }
    }
}