using Godot;
using Godot.Collections;
using System;

namespace HorrorGame.Levels
{
    public class Level : Spatial
    {
        Spatial SpawnPointsSpatial { get { return GetNodeOrNull<Spatial>("SpawnPoints"); } }

        public Array<Position3D> SpawnPoints
        {
            get
            {
                return new Array<Position3D>(SpawnPointsSpatial.GetChildren());
            }
        }

        public Position3D GetSpawnPoint(string previousLevel)
        {
            return SpawnPointsSpatial?.GetNodeOrNull<Position3D>(previousLevel);
        }
    }
}