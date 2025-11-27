using UnityEngine;
using TrashRunner.Core;
using TrashRunner.Player;

namespace TrashRunner.Environment
{
    /// <summary>
    /// Represents a collectible pickup item (coin or trash).
    /// Handles collection effects and object pooling/recycling.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        /// <summary>
        /// Type of pickup item.
        /// </summary>
        public enum PickupType
        {
            Coin,
            Trash
        }

        [Header("Pickup Settings")]
        [SerializeField] private PickupType pickupType = PickupType.Coin;

        [Header("References")]
        [SerializeField] private AudioManager audioManager;

        // Prevent double-collection
        private bool effectApplied = false;

        /// <summary>
        /// Applies the pickup effect to the player (add coin or trash).
        /// Called by PlayerCollisionHandler when collision occurs.
        /// </summary>
        public void ApplyEffect(PlayerRunnerController player, ScoreManager scoreManager)
        {
            // Prevent multiple applications
            if (effectApplied)
                return;

            effectApplied = true;

            // Apply pickup effect based on type
            if (scoreManager != null)
            {
                switch (pickupType)
                {
                    case PickupType.Coin:
                        scoreManager.AddCoin();
                        
                        // Play coin sound
                        if (audioManager != null)
                        {
                            audioManager.PlayPickupCoin();
                        }
                        break;

                    case PickupType.Trash:
                        scoreManager.AddTrash();
                        
                        // Play trash sound
                        if (audioManager != null)
                        {
                            audioManager.PlayPickupTrash();
                        }
                        break;
                }
            }

            // Deactivate pickup (return to pool)
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Resets the pickup state for pool recycling.
        /// Called when the pickup is returned to the pool or reused.
        /// </summary>
        public void ResetPickup()
        {
            effectApplied = false;
        }

        /// <summary>
        /// Sets the type of this pickup.
        /// </summary>
        public void SetPickupType(PickupType type)
        {
            pickupType = type;
        }

        /// <summary>
        /// Gets the current pickup type.
        /// </summary>
        public PickupType GetPickupType()
        {
            return pickupType;
        }

        private void OnEnable()
        {
            // Reset state when activated from pool
            ResetPickup();
        }

        private void OnDisable()
        {
            // Ensure state is clean when deactivated
            effectApplied = false;
        }
    }
}
