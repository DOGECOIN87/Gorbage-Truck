using UnityEngine;

namespace TrashRunner.Data
{
    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "TrashRunner/DifficultyConfig")]
    public class DifficultyConfig : ScriptableObject
    {
        [Header("Speed Progression")]
        [Tooltip("X: elapsed time (seconds), Y: forward speed")]
        [SerializeField] private AnimationCurve speedOverTime = AnimationCurve.Linear(0f, 10f, 120f, 25f);

        [Header("Obstacle Density")]
        [Tooltip("X: elapsed time (seconds), Y: obstacle spawn density (0-1)")]
        [SerializeField] private AnimationCurve obstacleDensityOverTime = AnimationCurve.Linear(0f, 0.3f, 90f, 0.7f);

        [Header("Pickup Density")]
        [Tooltip("X: elapsed time (seconds), Y: pickup spawn density (0-1)")]
        [SerializeField] private AnimationCurve pickupDensityOverTime = AnimationCurve.Linear(0f, 0.5f, 90f, 0.3f);

        public AnimationCurve SpeedOverTime => speedOverTime;
        public AnimationCurve ObstacleDensityOverTime => obstacleDensityOverTime;
        public AnimationCurve PickupDensityOverTime => pickupDensityOverTime;
    }
}
