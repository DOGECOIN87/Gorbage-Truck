using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TrashRunner.Data
{
    [CreateAssetMenu(fileName = "SpawnConfig", menuName = "TrashRunner/SpawnConfig")]
    public class SpawnConfig : ScriptableObject
    {
        [System.Serializable]
        public class SpawnEntry
        {
            [Tooltip("Prefab to spawn")]
            public GameObject prefab;

            [Tooltip("Probability of spawning this (0-1)")]
            [Range(0f, 1f)]
            public float spawnProbability = 0.5f;
        }

        [System.Serializable]
        public class ObstacleSpawnEntry : SpawnEntry
        {
            [Tooltip("Minimum distance between spawns of this obstacle type")]
            public float minDistanceBetweenSpawns = 5f;
        }

        [System.Serializable]
        public class PickupSpawnEntry : SpawnEntry
        {
            [Tooltip("Minimum distance between spawns of this pickup type")]
            public float minDistanceBetweenSpawns = 3f;
        }

        [Header("Obstacle Configuration")]
        [SerializeField] private List<ObstacleSpawnEntry> obstacleEntries = new List<ObstacleSpawnEntry>();

        [Header("Pickup Configuration")]
        [SerializeField] private List<PickupSpawnEntry> pickupEntries = new List<PickupSpawnEntry>();

        public List<SpawnEntry> ObstacleEntries => obstacleEntries.Cast<SpawnEntry>().ToList();
        public List<SpawnEntry> PickupEntries => pickupEntries.Cast<SpawnEntry>().ToList();
    }
}
