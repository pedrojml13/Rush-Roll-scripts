using UnityEngine;
using UnityEngine.SceneManagement;

namespace PJML.RushAndRoll
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        private int coinCount = 0;
        private bool levelEnded = false;
        private float startTime;
        private bool counting = false;

        void Awake()
        {
            // Singleton: asegura una única instancia
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Time.timeScale = 1f;
            coinCount = 0;
            levelEnded = false;
            counting = false;

            UIManager.Instance.ShowUIPanel();
            UIManager.Instance.UpdateCoinText(coinCount);
            UIManager.Instance.UpdateTimeText(0);
        }

        void Update()
        {
            if (counting)
            {
                float elapsed = Time.time - startTime;
                UIManager.Instance.UpdateTimeText(elapsed);
            }
        }

        // Control de tiempo
        public void PauseTime() => Time.timeScale = 0f;
        public void ResumeTime() => Time.timeScale = 1f;

        // Fin de nivel con victoria
        public void WinLevel()
        {
            if (levelEnded) return;
            levelEnded = true;

            UIManager.Instance.ShowVictoryPanel();
            GameManager.Instance.AddCoins(coinCount);

            int levelIndex = SceneManager.GetActiveScene().buildIndex - 1;
            GameManager.Instance.SaveLevelData(levelIndex, 3, GetElapsedTime());

            PauseTime();
        }

        // Fin de nivel por derrota
        public void GameOver()
        {
            if (levelEnded) return;
            levelEnded = true;

            UIManager.Instance.ShowGameOverPanel();
            PauseTime();
        }

        // Monedas
        public void AddCoin()
        {
            coinCount++;
            UIManager.Instance.UpdateCoinText(coinCount);
        }

        // Cronómetro
        public void StartFlag()
        {
            startTime = Time.time;
            counting = true;
        }

        public void StopFlag()
        {
            counting = false;
        }

        public float GetElapsedTime()
        {
            return counting ? Time.time - startTime : 0f;
        }

        // Navegación
        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
    }
}