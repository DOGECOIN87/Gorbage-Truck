using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TrashRunner.Core;

namespace TrashRunner.UI
{
    /// <summary>
    /// Controls the game over UI.
    /// Displays final stats and handles retry and return to main menu functionality.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI finalDistanceText;
        [SerializeField] private TextMeshProUGUI finalCoinsText;
        [SerializeField] private TextMeshProUGUI finalTrashText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Game References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreManager scoreManager;

        private void Start()
        {
            // Wire up buttons
            if (retryButton != null && gameManager != null)
            {
                retryButton.onClick.AddListener(OnRetryButtonClicked);
            }

            if (mainMenuButton != null && gameManager != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            }

            // Initially hide game over panel
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Subscribe to run ended event
            if (gameManager != null)
            {
                gameManager.OnRunEnded += ShowGameOver;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from run ended event
            if (gameManager != null)
            {
                gameManager.OnRunEnded -= ShowGameOver;
            }
        }

        private void OnDestroy()
        {
            // Clean up button listeners
            if (retryButton != null)
            {
                retryButton.onClick.RemoveListener(OnRetryButtonClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
            }
        }

        /// <summary>
        /// Called when the retry button is clicked.
        /// Starts a new run via GameManager.
        /// </summary>
        private void OnRetryButtonClicked()
        {
            if (gameManager != null)
            {
                // Hide game over panel
                if (gameOverPanel != null)
                {
                    gameOverPanel.SetActive(false);
                }

                gameManager.StartRun();
            }
        }

        /// <summary>
        /// Called when the main menu button is clicked.
        /// Returns to the main menu state.
        /// </summary>
        private void OnMainMenuButtonClicked()
        {
            if (gameManager != null)
            {
                // Hide game over panel
                if (gameOverPanel != null)
                {
                    gameOverPanel.SetActive(false);
                }

                // Transition to menu state
                gameManager.EndRun();
            }
        }

        /// <summary>
        /// Shows the game over screen and populates it with final stats.
        /// </summary>
        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Populate final stats from ScoreManager
            if (scoreManager != null)
            {
                if (finalScoreText != null)
                {
                    finalScoreText.text = $"Final Score: {scoreManager.Score}";
                }

                if (finalDistanceText != null)
                {
                    finalDistanceText.text = $"Distance: {Mathf.FloorToInt(scoreManager.Distance)}m";
                }

                if (finalCoinsText != null)
                {
                    finalCoinsText.text = $"Coins: {scoreManager.Coins}";
                }

                if (finalTrashText != null)
                {
                    finalTrashText.text = $"Trash: {scoreManager.Trash}";
                }
            }
        }
    }
}
