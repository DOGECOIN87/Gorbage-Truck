using UnityEngine;

namespace TrashRunner.Core
{
    public class DifficultyController : MonoBehaviour
    {
        // Dependencies
        [SerializeField] private Data.DifficultyConfig config;

        // State
        private float elapsedRunTime;
        private GameManager gameManager;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        private void Start()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(DifficultyController)}: DifficultyConfig reference not assigned!");
            }
        }

        private void Update()
        {
            // Only increment time when game is running
            if (gameManager != null && gameManager.CurrentState == RunState.Running)
            {
                elapsedRunTime += Time.deltaTime;
            }
        }

        public float GetCurrentSpeed()
        {
            if (config == null || config.SpeedOverTime == null)
            {
                Debug.LogWarning($"{nameof(DifficultyController)}: Config or speedOverTime curve is null, returning default speed");
                return 10f;
            }

            return config.SpeedOverTime.Evaluate(elapsedRunTime);
        }

        public float GetObstacleDensity()
        {
            if (config == null || config.ObstacleDensityOverTime == null)
            {
                Debug.LogWarning($"{nameof(DifficultyController)}: Config or obstacleDensityOverTime curve is null, returning default density");
                return 0.3f;
            }

            return config.ObstacleDensityOverTime.Evaluate(elapsedRunTime);
        }

        public float GetPickupDensity()
        {
            if (config == null || config.PickupDensityOverTime == null)
            {
                Debug.LogWarning($"{nameof(DifficultyController)}: Config or pickupDensityOverTime curve is null, returning default density");
                return 0.5f;
            }

            return config.PickupDensityOverTime.Evaluate(elapsedRunTime);
        }

        public void ResetTime()
        {
            elapsedRunTime = 0f;
        }
    }
}
