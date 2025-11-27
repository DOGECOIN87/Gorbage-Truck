using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TrashRunner.Core;

namespace TrashRunner.UI
{
    /// <summary>
    /// Controls the main menu UI.
    /// Displays the best score and handles the play button to start a new run.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button playButton;
        [SerializeField] private TextMeshProUGUI bestScoreText;

        [Header("Game References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private ScoreManager scoreManager;

        private void Start()
        {
            // Wire up play button
            if (playButton != null && gameManager != null)
            {
                playButton.onClick.AddListener(OnPlayButtonClicked);
            }

            // Display best score
            UpdateBestScore();
        }

        private void OnEnable()
        {
            // Update best score when menu becomes active
            UpdateBestScore();
        }

        private void OnDestroy()
        {
            // Clean up button listener
            if (playButton != null)
            {
                playButton.onClick.RemoveListener(OnPlayButtonClicked);
            }
        }

        /// <summary>
        /// Called when the play button is clicked.
        /// Starts a new run via GameManager.
        /// </summary>
        private void OnPlayButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.StartRun();
            }
        }

        /// <summary>
        /// Updates the best score display from ScoreManager.
        /// </summary>
        private void UpdateBestScore()
        {
            if (bestScoreText != null && scoreManager != null)
            {
                int bestScore = scoreManager.BestScore;
                bestScoreText.text = $"Best Score: {bestScore}";
            }
        }
    }
}
