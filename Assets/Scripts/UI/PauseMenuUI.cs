using UnityEngine;
using UnityEngine.UI;
using TrashRunner.Core;

namespace TrashRunner.UI
{
    /// <summary>
    /// Controls the pause menu UI.
    /// Handles resume and return to main menu functionality.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private GameObject pausePanel;

        [Header("Game References")]
        [SerializeField] private GameManager gameManager;

        private void Start()
        {
            // Wire up buttons
            if (resumeButton != null && gameManager != null)
            {
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }

            if (mainMenuButton != null && gameManager != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
            }

            // Initially hide pause panel
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Subscribe to game state events
            if (gameManager != null)
            {
                gameManager.OnPause += ShowPauseMenu;
                gameManager.OnResume += HidePauseMenu;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from game state events
            if (gameManager != null)
            {
                gameManager.OnPause -= ShowPauseMenu;
                gameManager.OnResume -= HidePauseMenu;
            }
        }

        private void OnDestroy()
        {
            // Clean up button listeners
            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
            }
        }

        /// <summary>
        /// Called when the resume button is clicked.
        /// Resumes the game via GameManager.
        /// </summary>
        private void OnResumeButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.ResumeGame();
            }
        }

        /// <summary>
        /// Called when the main menu button is clicked.
        /// Ends the current run and returns to the main menu.
        /// </summary>
        private void OnMainMenuButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.EndRun();
            }
        }

        /// <summary>
        /// Shows the pause menu panel.
        /// </summary>
        private void ShowPauseMenu()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hides the pause menu panel.
        /// </summary>
        private void HidePauseMenu()
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }
    }
}
