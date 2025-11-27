using UnityEngine;
using TMPro;
using TrashRunner.Core;

namespace TrashRunner.UI
{
    /// <summary>
    /// Controls the Heads-Up Display (HUD) during gameplay.
    /// Updates UI elements to reflect current game stats (distance, score, coins, trash, lives).
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("UI Text References")]
        [SerializeField] private TextMeshProUGUI distanceText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI trashText;
        [SerializeField] private TextMeshProUGUI livesText;

        [Header("References")]
        [SerializeField] private ScoreManager scoreManager;

        private void OnEnable()
        {
            // Subscribe to score manager events
            if (scoreManager != null)
            {
                scoreManager.OnDistanceChanged += UpdateDistance;
                scoreManager.OnScoreChanged += UpdateScore;
                scoreManager.OnCoinsChanged += UpdateCoins;
                scoreManager.OnTrashChanged += UpdateTrash;
                scoreManager.OnLivesChanged += UpdateLives;

                // Initialize display with current values
                UpdateDistance(scoreManager.Distance);
                UpdateScore(scoreManager.Score);
                UpdateCoins(scoreManager.Coins);
                UpdateTrash(scoreManager.Trash);
                UpdateLives(scoreManager.Lives);
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from score manager events
            if (scoreManager != null)
            {
                scoreManager.OnDistanceChanged -= UpdateDistance;
                scoreManager.OnScoreChanged -= UpdateScore;
                scoreManager.OnCoinsChanged -= UpdateCoins;
                scoreManager.OnTrashChanged -= UpdateTrash;
                scoreManager.OnLivesChanged -= UpdateLives;
            }
        }

        /// <summary>
        /// Updates the distance display.
        /// </summary>
        private void UpdateDistance(float distance)
        {
            if (distanceText != null)
            {
                distanceText.text = $"{Mathf.FloorToInt(distance)}m";
            }
        }

        /// <summary>
        /// Updates the score display.
        /// </summary>
        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        /// <summary>
        /// Updates the coins display.
        /// </summary>
        private void UpdateCoins(int coins)
        {
            if (coinsText != null)
            {
                coinsText.text = $"Coins: {coins}";
            }
        }

        /// <summary>
        /// Updates the trash display.
        /// </summary>
        private void UpdateTrash(int trash)
        {
            if (trashText != null)
            {
                trashText.text = $"Trash: {trash}";
            }
        }

        /// <summary>
        /// Updates the lives display.
        /// </summary>
        private void UpdateLives(int lives)
        {
            if (livesText != null)
            {
                livesText.text = $"Lives: {lives}";
            }
        }
    }
}
