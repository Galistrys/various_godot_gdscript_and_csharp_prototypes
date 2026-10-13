using Godot;
using Godot.Collections;
using SpaceShooter.Data;
using SpaceShooter.Entities.Ships;
using SpaceShooter.GUI.HUD;
using SpaceShooter.Pilots.AI;
using SpaceShooter.Pilots.Player;
using SpaceShooter.Weapons;
using System;

namespace SpaceShooter.Managers
{
	public class GameManager : Node
	{
		PackedScene PlayerPilotScene { get; set; }
		PackedScene AIPilotScene { get; set; }

		public static GameManager Instance { get; private set; }

		public Spatial VFXSpatial { get; private set; }

		public Node TeamsNode { get; private set; }

		public GameConfiguration GameConfiguration { get; private set; }

		public GameManager()
		{
			
		}

        public override void _Ready()
        {
			GetTree().Paused = true;
            Instance = this;

			PlayerPilotScene = ResourceLoader.Load<PackedScene>("res://Resources/Pilots/Player/Player.res");
			AIPilotScene = ResourceLoader.Load<PackedScene>("res://Resources/Pilots/PilotAI/PilotAI.res");

			TeamsNode = GetNode<Node>("World/Teams");
			VFXSpatial = GetNode<Spatial>("World/VFXSpatial");

			if (MainManager.Instance.GameConfiguration != null)
				Init(MainManager.Instance.GameConfiguration);
			else
			{
				Camera cam = new Camera();
				cam.Translate(Vector3.Back * 5);
				AddChild(cam);
				HUDController.Instance.Visible = false;
				GetTree().Paused = false;
			}
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
                        GetTree().Paused = true;
                    }
					else
                    {
                        Input.SetMouseMode(Input.MouseMode.Captured);
                        GetTree().Paused = false;
                    }
				}
			}
        }

        public override void _ExitTree()
        {
            base._ExitTree();
			Instance = null;
        }

		string GetTeamName(int team)
		{
			return GD.Str("Team_", team);
		}

		void CreateTeamNode(int team)
		{
			TeamsNode.AddChild(new Node() { Name = GetTeamName(team) } );
		}
		
		Node GetTeamNode(int team)
		{
			return TeamsNode.GetNode<Node>(GetTeamName(team));
		}

		void Init(GameConfiguration gameConfig)
		{
			GameConfiguration = gameConfig;
			Dictionary<int, int> numberOfShipsInTeam = new Dictionary<int, int>();

			foreach (ShipConfiguration shipConfig in gameConfig.PlayersList)
			{
				if (numberOfShipsInTeam.ContainsKey(shipConfig.TeamNumber))
					numberOfShipsInTeam[shipConfig.TeamNumber]++;
				else
					numberOfShipsInTeam.Add(shipConfig.TeamNumber, 1);
				// PlayerPilot player = PlayerPilotScene.Instance() as PlayerPilot;
				// player.StartingPosition = new Vector3(0, 0, 100);
				// PlayerSpatial.AddChild(player);
				// player.Init(shipConfig);
			}

			foreach (ShipConfiguration shipConfig in gameConfig.AIList)
			{
				if (numberOfShipsInTeam.ContainsKey(shipConfig.TeamNumber))
					numberOfShipsInTeam[shipConfig.TeamNumber]++;
				else
					numberOfShipsInTeam.Add(shipConfig.TeamNumber, 1);
			}

			ShipController.ShipsMaterials = new SpatialMaterial[numberOfShipsInTeam.Count];
			Bullet.GlowingMaterials = new SpatialMaterial[numberOfShipsInTeam.Count];

			foreach (int key in numberOfShipsInTeam.Keys)
			{
				CreateTeamNode(key);
				Color color = GameConfiguration.TeamColors[key];
				ShipController.ShipsMaterials[key] = new SpatialMaterial() { AlbedoColor = color };

				float r = Mathf.Pow(color.r, 3);
				float g = Mathf.Pow(color.g, 3);
				float b = Mathf.Pow(color.b, 3);
				Color custom = new Color(r, g, b);
				Bullet.GlowingMaterials[key] = new SpatialMaterial() { AlbedoColor = custom, Emission = custom, EmissionEnabled = true, EmissionEnergy = 16, ParamsCullMode = SpatialMaterial.CullMode.Disabled };
			}

			Dictionary<int, Vector3> teamCenters = new Dictionary<int, Vector3>();
			float centerAngleStep = 2.0f * Mathf.Pi / numberOfShipsInTeam.Count;
			float centerCount = 0;

			Dictionary<int, int> teamsCount = new Dictionary<int, int>();
			Dictionary<int, float> teamsAngles = new Dictionary<int, float>();

			foreach (var key in numberOfShipsInTeam.Keys)
			{
				float angle = centerAngleStep * centerCount++;
				teamCenters.Add(key, new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * gameConfig.TeamsSpawnRadius);

				teamsCount.Add(key, 0);
				teamsAngles.Add(key, 2.0f * Mathf.Pi / numberOfShipsInTeam[key]);
			}

			foreach (ShipConfiguration shipConfig in gameConfig.PlayersList)
			{
				PlayerPilot player = PlayerPilotScene.Instance() as PlayerPilot;
				int team = shipConfig.TeamNumber;

				player.TeamNumber = team;
				GetTeamNode(team).AddChild(player);
				player.Init(shipConfig);

				Transform tempTransform = Transform.Identity;
				tempTransform.origin = teamCenters[team];
				tempTransform = tempTransform.LookingAt(Vector3.Zero, Vector3.Up);
				
				float angle = teamsAngles[team] * teamsCount[team]++;
				Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * gameConfig.ShipsSpawnRadius;
				player.ShipController.Transform = tempTransform.Translated(direction);
			}

			foreach (ShipConfiguration shipConfig in gameConfig.AIList)
			{
				PilotAI ai = AIPilotScene.Instance() as PilotAI;
				int team = shipConfig.TeamNumber;

				ai.TeamNumber = team;
				GetTeamNode(team).AddChild(ai);
				ai.Init(shipConfig);

				Transform tempTransform = Transform.Identity;
				tempTransform.origin = teamCenters[team];
				tempTransform = tempTransform.LookingAt(Vector3.Zero, Vector3.Up);
				
				float angle = teamsAngles[team] * teamsCount[team]++;
				Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * gameConfig.ShipsSpawnRadius;
				ai.ShipController.Transform = tempTransform.Translated(direction);
			}

			GetTree().Paused = false;
		}
	}
}