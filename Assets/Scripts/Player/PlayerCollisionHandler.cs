using UnityEngine;
using TrashRunner.Core;
using TrashRunner.Environment;

namespace TrashRunner.Player
{
    /// <summary>
    /// Handles collision detection between the player and game objects (obstacles and pickups).
    /// Delegates effect application to the respective object components.
    /// </summary>
    public class PlayerCollisionHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerRunnerController playerRunnerController;
        [SerializeField] private ScoreManager scoreManager;

        private void OnTriggerEnter(Collider other)
        {
            // Check for Obstacle component
            Obstacle obstacle = other.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.ApplyEffect(playerRunnerController, scoreManager);
                return;
            }

            // Check for Pickup component
            Pickup pickup = other.GetComponent<Pickup>();
            if (pickup != null)
            {
                pickup.ApplyEffect(playerRunnerController, scoreManager);
                return;
            }
        }
    }
}
