using UnityEngine;
using TrashRunner.Core;
using TrashRunner.Player;

namespace TrashRunner.Environment
{
    /// <summary>
    /// Represents an obstacle in the game that causes the player to lose a life when hit.
    /// Handles collision effects and object pooling/recycling.
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AudioManager audioManager;

        // Prevent double-hits
        private bool effectApplied = false;

        /// <summary>
        /// Applies the obstacle effect to the player (lose a life).
        /// Called by PlayerCollisionHandler when collision occurs.
        /// </summary>
        public void ApplyEffect(PlayerRunnerController player, ScoreManager scoreManager)
        {
            // Prevent multiple applications
            if (effectApplied)
                return;

            effectApplied = true;

            // Apply damage to player
            if (scoreManager != null)
            {
                scoreManager.LoseLife();
            }

            // Play hit sound
            if (audioManager != null)
            {
                audioManager.PlayHitObstacle();
            }

            // Deactivate obstacle (return to pool)
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets the obstacle state for pool recycling.
        /// Called when the obstacle is returned to the pool or reused.
        /// </summary>
        public void ResetObstacle()
        {
            effectApplied = false;
        }

        private void OnEnable()
        {
            // Reset state when activated from pool
            ResetObstacle();
        }

        private void OnDisable()
        {
            // Ensure state is clean when deactivated
            effectApplied = false;
        }
    }
}
