using Godot;
using Godot.Collections;
using SpaceShooter.Entities;
using SpaceShooter.GUI.HUD;
using System;

namespace SpaceShooter.Pilots.Player
{
	public class PlayerCamera : ClippedCamera
	{
		public Position3D CameraPosition3D { get; private set; }

		PlayerPilot PilotOwner { get { return GetParent().GetParent<PlayerPilot>(); } }

        public override void _Ready()
        {
            
        }

		public void Init()
		{
			CameraPosition3D = PilotOwner.ShipController.GetCameraPosition();
		}

        public override void _Process(float delta)
        {
            this.GlobalTransform = this.GlobalTransform.InterpolateWith(CameraPosition3D.GlobalTransform, delta * 10);
        }

		public Vector3 GetTargetPoint()
		{
			var from = this.ProjectRayOrigin(HUDController.Instance.Pointer.GlobalPosition);
			var to = from + this.ProjectRayNormal(HUDController.Instance.Pointer.GlobalPosition) * 1000;

			var result = ShootRay(from, to, 3u);

			if (result.Count == 0)
				return to;
			else
				return (Vector3)result["position"];
		}

		public IEntity GetTarget()
		{
			var from = this.ProjectRayOrigin(HUDController.Instance.Pointer.GlobalPosition);
			var to = from + this.ProjectRayNormal(HUDController.Instance.Pointer.GlobalPosition) * 1000;

			var result = ShootRay(from, to, 3u);

			if (result.Count == 0)
				return null;

			if (result["collider"] is IEntity ie)
			{
				if (ie.GetTeamNumber() != PilotOwner.TeamNumber)
					return ie;
			}
			
			return null;
		}

		public Dictionary ShootRay(Vector3 from, Vector3 to, uint collisionMask)
		{
			var spaceState = GetWorld().DirectSpaceState;
			return spaceState.IntersectRay(from, to, new Godot.Collections.Array() { PilotOwner.ShipController }, collisionMask);
		}
	}
}