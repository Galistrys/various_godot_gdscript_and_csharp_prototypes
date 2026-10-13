using Godot;
using System.Collections.Generic;
using System;

namespace SpaceShooter.Data
{
	public class GameConfiguration
	{
		List<ShipConfiguration> _playersList = new List<ShipConfiguration>();
		public List<ShipConfiguration> PlayersList { get { return _playersList; } }

		List<ShipConfiguration> _aiList = new List<ShipConfiguration>();
		public List<ShipConfiguration> AIList { get { return _aiList; } }

		public float TeamsSpawnRadius { get; private set; }
		public float ShipsSpawnRadius { get; private set; } = 30;

		public Color[] TeamColors { get; private set; }

		public GameConfiguration(float teamsSpawnRadius, Color[] teamColors)
		{
			TeamsSpawnRadius = teamsSpawnRadius;
			TeamColors = teamColors;
		}

		public void AddPlayerShipConfiguration(ShipConfiguration configuration)
		{
			_playersList.Add(configuration);
		}

		public void AddAIShipConfiguration(ShipConfiguration configuration)
		{
			_aiList.Add(configuration);
		}

        public override string ToString()
        {
            string result = "GAME CONFIG: ";

			result += "Players: ";
			foreach (var shipConfig in PlayersList)
			{
				result += "[" + shipConfig.ToString() + "]";
			}

			result += " | AIs: ";
			foreach (var shipConfig in AIList)
			{
				result += "[" + shipConfig.ToString() + "]";
			}

			return result + "\n";
        }
    
	}
}