using UnityEngine;
using System.Collections.Generic;
using TrashRunner.Core;
using TrashRunner.Data;

namespace TrashRunner.Environment
{
    /// <summary>
    /// Manages the spawning and recycling of track segments, obstacles, and pickups.
    /// Uses object pooling for efficient memory management in an endless runner.
    /// </summary>
    public class SegmentSpawner : MonoBehaviour
    {
        [Header("Segment Settings")]
        [SerializeField] private int initialSegmentCount = 5;
        [SerializeField] private int segmentsAhead = 3;
        [SerializeField] private Transform segmentParent;
        [SerializeField] private GameObject segmentPrefab;

        [Header("References")]
        [SerializeField] private SpawnConfig spawnConfig;
        [SerializeField] private DifficultyController difficultyController;

        [Header("Player Reference")]
        [SerializeField] private Transform playerTransform;

        // Object pools
        private Queue<TrackSegment> segmentPool = new Queue<TrackSegment>();
        private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

        // Active segments tracking
        private List<TrackSegment> activeSegments = new List<TrackSegment>();
        private float nextSpawnZ = 0f;

        private void Awake()
        {
            // Pre-initialize segment pool
            InitializeSegmentPool();

            // Pre-initialize object pools for obstacles and pickups
            InitializeObjectPools();
        }

        private void Start()
        {
            // Spawn initial segments
            SpawnInitialSegments();
        }

        private void Update()
        {
            if (playerTransform == null)
                return;

            // Check if we need to spawn new segments ahead of the player
            float playerZ = playerTransform.position.z;
            
            // Spawn new segments if player is approaching the end
            while (activeSegments.Count < segmentsAhead || 
                   (activeSegments.Count > 0 && nextSpawnZ < playerZ + (segmentsAhead * 10f)))
            {
                SpawnNextSegment();
            }

            // Recycle old segments behind the player
            RecycleOldSegments(playerZ);
        }

        /// <summary>
        /// Initializes the segment pool with pre-instantiated segments.
        /// </summary>
        private void InitializeSegmentPool()
        {
            if (segmentPrefab == null)
            {
                Debug.LogError("SegmentSpawner: Segment prefab is not assigned!");
                return;
            }

            for (int i = 0; i < initialSegmentCount + 5; i++)
            {
                GameObject segmentObj = Instantiate(segmentPrefab, segmentParent);
                TrackSegment segment = segmentObj.GetComponent<TrackSegment>();
                
                if (segment != null)
                {
                    segmentObj.SetActive(false);
                    segmentPool.Enqueue(segment);
                }
            }
        }

        /// <summary>
        /// Initializes object pools for obstacles and pickups.
        /// </summary>
        private void InitializeObjectPools()
        {
            if (spawnConfig == null)
            {
                Debug.LogWarning("SegmentSpawner: SpawnConfig is not assigned!");
                return;
            }

            // Initialize obstacle pools
            foreach (var entry in spawnConfig.obstacleEntries)
            {
                if (entry.prefab != null && !objectPools.ContainsKey(entry.prefab))
                {
                    objectPools[entry.prefab] = new Queue<GameObject>();
                    
                    // Pre-instantiate some objects
                    for (int i = 0; i < 10; i++)
                    {
                        GameObject obj = Instantiate(entry.prefab, segmentParent);
                        obj.SetActive(false);
                        objectPools[entry.prefab].Enqueue(obj);
                    }
                }
            }

            // Initialize pickup pools
            foreach (var entry in spawnConfig.pickupEntries)
            {
                if (entry.prefab != null && !objectPools.ContainsKey(entry.prefab))
                {
                    objectPools[entry.prefab] = new Queue<GameObject>();
                    
                    // Pre-instantiate some objects
                    for (int i = 0; i < 15; i++)
                    {
                        GameObject obj = Instantiate(entry.prefab, segmentParent);
                        obj.SetActive(false);
                        objectPools[entry.prefab].Enqueue(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Spawns the initial set of segments at game start.
        /// </summary>
        private void SpawnInitialSegments()
        {
            nextSpawnZ = 0f;
            
            for (int i = 0; i < initialSegmentCount; i++)
            {
                SpawnNextSegment();
            }
        }

        /// <summary>
        /// Spawns the next segment in the sequence.
        /// </summary>
        private void SpawnNextSegment()
        {
            TrackSegment segment = GetSegmentFromPool();
            
            if (segment == null)
            {
                Debug.LogWarning("SegmentSpawner: No segments available in pool!");
                return;
            }

            // Position the segment
            segment.transform.position = new Vector3(0, 0, nextSpawnZ);
            segment.gameObject.SetActive(true);

            // Populate segment with obstacles and pickups
            PopulateSegment(segment);

            // Add to active segments
            activeSegments.Add(segment);

            // Update next spawn position
            nextSpawnZ += segment.Length;
        }

        /// <summary>
        /// Populates a segment with obstacles and pickups based on difficulty and spawn config.
        /// </summary>
        private void PopulateSegment(TrackSegment segment)
        {
            if (spawnConfig == null || difficultyController == null)
                return;

            // Get current difficulty densities
            float obstacleDensity = difficultyController.GetObstacleDensity();
            float pickupDensity = difficultyController.GetPickupDensity();

            Transform[] spawnPoints = segment.GetAllSpawnPoints();

            // Spawn obstacles
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint == null)
                    continue;

                // Check if we should spawn an obstacle based on density
                if (Random.value < obstacleDensity)
                {
                    GameObject obstaclePrefab = SelectRandomPrefab(spawnConfig.obstacleEntries);
                    
                    if (obstaclePrefab != null)
                    {
                        GameObject obstacle = GetFromPool(obstaclePrefab);
                        
                        if (obstacle != null)
                        {
                            obstacle.transform.position = spawnPoint.position;
                            obstacle.transform.rotation = spawnPoint.rotation;
                            obstacle.SetActive(true);
                            segment.AddSpawnedObject(obstacle);
                        }
                    }
                }
                // Otherwise, check if we should spawn a pickup
                else if (Random.value < pickupDensity)
                {
                    GameObject pickupPrefab = SelectRandomPrefab(spawnConfig.pickupEntries);
                    
                    if (pickupPrefab != null)
                    {
                        GameObject pickup = GetFromPool(pickupPrefab);
                        
                        if (pickup != null)
                        {
                            pickup.transform.position = spawnPoint.position;
                            pickup.transform.rotation = spawnPoint.rotation;
                            pickup.SetActive(true);
                            segment.AddSpawnedObject(pickup);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Selects a random prefab from spawn entries based on probability weights.
        /// </summary>
        private GameObject SelectRandomPrefab(List<SpawnConfig.SpawnEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                return null;

            float totalProbability = 0f;
            foreach (var entry in entries)
            {
                totalProbability += entry.probability;
            }

            float randomValue = Random.value * totalProbability;
            float currentSum = 0f;

            foreach (var entry in entries)
            {
                currentSum += entry.probability;
                if (randomValue <= currentSum)
                {
                    return entry.prefab;
                }
            }

            // Fallback to first entry
            return entries[0].prefab;
        }

        /// <summary>
        /// Recycles segments that are behind the player.
        /// </summary>
        private void RecycleOldSegments(float playerZ)
        {
            // Remove segments that are far behind the player
            for (int i = activeSegments.Count - 1; i >= 0; i--)
            {
                TrackSegment segment = activeSegments[i];
                
                if (segment.transform.position.z + segment.Length < playerZ - 20f)
                {
                    // Clear spawned objects
                    segment.ClearSpawnedObjects();
                    
                    // Return segment to pool
                    segment.gameObject.SetActive(false);
                    segmentPool.Enqueue(segment);
                    
                    // Remove from active list
                    activeSegments.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Gets a segment from the pool or creates a new one if needed.
        /// </summary>
        private TrackSegment GetSegmentFromPool()
        {
            if (segmentPool.Count > 0)
            {
                return segmentPool.Dequeue();
            }

            // Create new segment if pool is empty
            if (segmentPrefab != null)
            {
                GameObject segmentObj = Instantiate(segmentPrefab, segmentParent);
                return segmentObj.GetComponent<TrackSegment>();
            }

            return null;
        }

        /// <summary>
        /// Gets an object from the pool or creates a new one if needed.
        /// </summary>
        private GameObject GetFromPool(GameObject prefab)
        {
            if (prefab == null)
                return null;

            // Initialize pool if it doesn't exist
            if (!objectPools.ContainsKey(prefab))
            {
                objectPools[prefab] = new Queue<GameObject>();
            }

            // Get from pool or create new
            if (objectPools[prefab].Count > 0)
            {
                GameObject obj = objectPools[prefab].Dequeue();
                
                // Reset obstacle/pickup state
                Obstacle obstacle = obj.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    obstacle.ResetObstacle();
                }

                Pickup pickup = obj.GetComponent<Pickup>();
                if (pickup != null)
                {
                    pickup.ResetPickup();
                }

                return obj;
            }
            else
            {
                return Instantiate(prefab, segmentParent);
            }
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        private void ReturnToPool(GameObject obj, GameObject prefab)
        {
            if (obj == null || prefab == null)
                return;

            obj.SetActive(false);

            if (!objectPools.ContainsKey(prefab))
            {
                objectPools[prefab] = new Queue<GameObject>();
            }

            objectPools[prefab].Enqueue(obj);
        }

        /// <summary>
        /// Resets the track by clearing all segments and respawning initial segments.
        /// Called by GameManager when starting a new run.
        /// </summary>
        public void ResetTrack()
        {
            // Deactivate and return all active segments to pool
            foreach (TrackSegment segment in activeSegments)
            {
                segment.ClearSpawnedObjects();
                segment.gameObject.SetActive(false);
                segmentPool.Enqueue(segment);
            }

            activeSegments.Clear();
            nextSpawnZ = 0f;

            // Spawn initial segments
            SpawnInitialSegments();
        }

        /// <summary>
        /// Sets the player transform to track for segment spawning.
        /// </summary>
        public void SetPlayer(Transform player)
        {
            playerTransform = player;
        }
    }
}
