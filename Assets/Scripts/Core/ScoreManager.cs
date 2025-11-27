using UnityEngine;

namespace TrashRunner.Core
{
    public class ScoreManager : MonoBehaviour
    {
        // Events
        public event System.Action<int> OnCoinsChanged;
        public event System.Action<int> OnTrashChanged;
        public event System.Action<int> OnLivesChanged;
        public event System.Action<float> OnDistanceChanged;
        public event System.Action<int> OnScoreChanged;

        // Tunable Fields
        [SerializeField] private int scorePerMeter = 1;
        [SerializeField] private int scorePerCoin = 10;
        [SerializeField] private int scorePerTrash = 25;
        [SerializeField] private int initialLives = 3;

        // Properties
        public int Coins { get; private set; }
        public int Trash { get; private set; }
        public int Lives { get; private set; }
        public float Distance { get; private set; }
        public int Score { get; private set; }
        public int BestScore { get; private set; }

        // Constants
        private const string BEST_SCORE_KEY = "TrashRunner_BestScore";

        // Internal tracking
        private float lastScoredDistance;

        private void Awake()
        {
            LoadBestScore();
        }

        private void LoadBestScore()
        {
            BestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        }

        private void SaveBestScore()
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, BestScore);
            PlayerPrefs.Save();
        }

        public void ResetStats()
        {
            Coins = 0;
            Trash = 0;
            Lives = initialLives;
            Distance = 0f;
            Score = 0;
            lastScoredDistance = 0f;

            // Notify all listeners of reset values
            OnCoinsChanged?.Invoke(Coins);
            OnTrashChanged?.Invoke(Trash);
            OnLivesChanged?.Invoke(Lives);
            OnDistanceChanged?.Invoke(Distance);
            OnScoreChanged?.Invoke(Score);
        }

        public void AddDistance(float delta)
        {
            if (delta <= 0f)
                return;

            Distance += delta;

            // Add score for distance traveled (per meter)
            float distanceSinceLastScore = Distance - lastScoredDistance;
            if (distanceSinceLastScore >= 1f)
            {
                int metersToScore = Mathf.FloorToInt(distanceSinceLastScore);
                Score += metersToScore * scorePerMeter;
                lastScoredDistance += metersToScore;
                OnScoreChanged?.Invoke(Score);
            }

            OnDistanceChanged?.Invoke(Distance);
        }

        public void AddCoin()
        {
            Coins++;
            Score += scorePerCoin;

            OnCoinsChanged?.Invoke(Coins);
            OnScoreChanged?.Invoke(Score);
        }

        public void AddTrash()
        {
            Trash++;
            Score += scorePerTrash;

            OnTrashChanged?.Invoke(Trash);
            OnScoreChanged?.Invoke(Score);
        }

        public void LoseLife()
        {
            Lives--;
            OnLivesChanged?.Invoke(Lives);

            // Check if game over
            if (Lives <= 0)
            {
                HandleGameOver();
            }
        }

        private void HandleGameOver()
        {
            // Update best score if current score is higher
            if (Score > BestScore)
            {
                BestScore = Score;
                SaveBestScore();
            }
        }
    }
}
