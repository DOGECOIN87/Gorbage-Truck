using UnityEngine;
using System.Collections.Generic;

namespace TrashRunner.Environment
{
    /// <summary>
    /// Represents a single segment of the endless runner track.
    /// Manages spawn points for obstacles and pickups, and tracks spawned objects for recycling.
    /// </summary>
    public class TrackSegment : MonoBehaviour
    {
        [Header("Segment Settings")]
        [SerializeField] private float length = 10f;

        [Header("Spawn Points")]
        [SerializeField] private Transform[] laneSpawnPoints = new Transform[3];

        // Track spawned objects for cleanup/recycling
        private List<GameObject> spawnedObjects = new List<GameObject>();

        /// <summary>
        /// Gets the length of this track segment.
        /// </summary>
        public float Length => length;

        /// <summary>
        /// Gets the spawn point transform for a specific lane.
        /// </summary>
        /// <param name="laneIndex">Lane index (0-2)</param>
        /// <returns>Transform of the spawn point, or null if invalid index</returns>
        public Transform GetLaneSpawnPoint(int laneIndex)
        {
            if (laneIndex >= 0 && laneIndex < laneSpawnPoints.Length)
            {
                return laneSpawnPoints[laneIndex];
            }

            Debug.LogWarning($"Invalid lane index: {laneIndex}. Must be between 0 and {laneSpawnPoints.Length - 1}");
            return null;
        }

        /// <summary>
        /// Gets all lane spawn points.
        /// </summary>
        public Transform[] GetAllSpawnPoints()
        {
            return laneSpawnPoints;
        }

        /// <summary>
        /// Adds a spawned object to the tracking list.
        /// </summary>
        public void AddSpawnedObject(GameObject obj)
        {
            if (obj != null && !spawnedObjects.Contains(obj))
            {
                spawnedObjects.Add(obj);
            }
        }

        /// <summary>
        /// Clears all spawned objects from this segment.
        /// Deactivates objects for pool recycling.
        /// </summary>
        public void ClearSpawnedObjects()
        {
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null && obj.activeInHierarchy)
                {
                    obj.SetActive(false);
                }
            }

            spawnedObjects.Clear();
        }

        /// <summary>
        /// Gets the end position of this segment (for spawning the next segment).
        /// </summary>
        public Vector3 GetEndPosition()
        {
            return transform.position + transform.forward * length;
        }

        /// <summary>
        /// Gets the start position of this segment.
        /// </summary>
        public Vector3 GetStartPosition()
        {
            return transform.position;
        }

        private void OnDrawGizmos()
        {
            // Visualize segment bounds in editor
            Gizmos.color = Color.green;
            Vector3 start = transform.position;
            Vector3 end = start + transform.forward * length;
            Gizmos.DrawLine(start, end);

            // Visualize spawn points
            if (laneSpawnPoints != null)
            {
                Gizmos.color = Color.yellow;
                foreach (Transform spawnPoint in laneSpawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                    }
                }
            }
        }
    }
}
