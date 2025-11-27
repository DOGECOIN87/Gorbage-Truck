using UnityEngine;

namespace TrashRunner.Core
{
    public enum RunState
    {
        Menu,
        Running,
        Paused,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        // Events
        public event System.Action OnRunStarted;
        public event System.Action OnRunEnded;
        public event System.Action OnPause;
        public event System.Action OnResume;

        // Dependencies
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private DifficultyController difficultyController;
        [SerializeField] private SegmentSpawner segmentSpawner;
        [SerializeField] private Player.PlayerRunnerController playerController;

        // Properties
        public RunState CurrentState { get; private set; } = RunState.Menu;

        private void Start()
        {
            ValidateDependencies();
            CurrentState = RunState.Menu;
        }

        private void ValidateDependencies()
        {
            if (scoreManager == null)
                Debug.LogError($"{nameof(GameManager)}: ScoreManager reference not assigned!");
            if (difficultyController == null)
                Debug.LogError($"{nameof(GameManager)}: DifficultyController reference not assigned!");
            if (segmentSpawner == null)
                Debug.LogError($"{nameof(GameManager)}: SegmentSpawner reference not assigned!");
            if (playerController == null)
                Debug.LogError($"{nameof(GameManager)}: PlayerRunnerController reference not assigned!");
        }

        public void StartRun()
        {
            // 9-step reset sequence as per requirements
            
            // Step 1: Reset player position to start (center lane, z=0)
            if (playerController != null)
            {
                playerController.ResetToStart();
            }

            // Step 2: Reset track
            if (segmentSpawner != null)
            {
                segmentSpawner.ResetTrack();
            }

            // Step 3: Reset stats
            if (scoreManager != null)
            {
                scoreManager.ResetStats();
            }

            // Step 4: Reset difficulty timer
            if (difficultyController != null)
            {
                difficultyController.ResetTime();
            }

            // Step 5: Ensure player GameObject is active/enabled
            if (playerController != null)
            {
                playerController.gameObject.SetActive(true);
                playerController.enabled = true;
            }

            // Step 6: Reset player lane to center (index 1)
            if (playerController != null)
            {
                playerController.SetLane(1);
            }

            // Step 7: Return all pooled objects to inactive state (handled by ResetTrack)
            // Already done in step 2

            // Step 8: Set CurrentState to Running
            CurrentState = RunState.Running;
            Time.timeScale = 1f;

            // Step 9: Invoke OnRunStarted event
            OnRunStarted?.Invoke();
        }

        public void EndRun()
        {
            if (CurrentState != RunState.Running && CurrentState != RunState.Paused)
                return;

            CurrentState = RunState.GameOver;
            Time.timeScale = 1f;
            OnRunEnded?.Invoke();
        }

        public void PauseGame()
        {
            if (CurrentState != RunState.Running)
                return;

            CurrentState = RunState.Paused;
            Time.timeScale = 0f;
            OnPause?.Invoke();
        }

        public void ResumeGame()
        {
            if (CurrentState != RunState.Paused)
                return;

            CurrentState = RunState.Running;
            Time.timeScale = 1f;
            OnResume?.Invoke();
        }

        public void ReturnToMenu()
        {
            CurrentState = RunState.Menu;
            Time.timeScale = 1f;
        }
    }
}
