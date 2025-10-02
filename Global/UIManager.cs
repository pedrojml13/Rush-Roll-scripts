using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace PJML.RushAndRoll
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Panels")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject UIPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI coinCountText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI recalibrateText;
        [SerializeField] private TextMeshProUGUI coinsCollectedText;
        [SerializeField] private TextMeshProUGUI finalTimeText;

        void Awake()
        {
            // Singleton: asegura una única instancia persistente
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Paneles de estado
        public void ShowVictoryPanel()
        {
            victoryPanel.SetActive(true);
            UIPanel.SetActive(false);
            coinsCollectedText.text = "Total coins:" + coinCountText.text;
            finalTimeText.text = timeText.text;
        }

        public void ShowGameOverPanel()
        {
            gameOverPanel.SetActive(true);
            UIPanel.SetActive(false);
        }

        public void ShowUIPanel()
        {
            UIPanel.SetActive(true);
            victoryPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        public void HideAllPanels()
        {
            UIPanel.SetActive(false);
            victoryPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            pausePanel.SetActive(false);
        }

        // Pausa y recalibración
        public void OnPauseButton()
        {
            pausePanel.SetActive(true);
            UIPanel.SetActive(false);
            LevelManager.Instance.PauseTime();
        }

        public void OnContinueButton()
        {
            pausePanel.SetActive(false);
            UIPanel.SetActive(true);
            LevelManager.Instance.ResumeTime();
        }

        public void OnRecalibrateButton()
        {
            if (BallController.Instance == null) return;

            BallController.Instance.RecalibrateTilt();
            recalibrateText.text = "Recalibrated!";
            StartCoroutine(ClearTextAfterDelay(1f));
        }

        IEnumerator ClearTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            recalibrateText.text = "";
        }

        // Navegación de niveles
        public void OnNextLevelButton()
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextLevel >= SceneManager.sceneCountInBuildSettings)
                nextLevel = 1;

            HideAllPanels();
            SceneManager.LoadScene(nextLevel);
        }

        public void OnReplayButton()
        {
            HideAllPanels();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnMenuButton()
        {
            HideAllPanels();
            LevelManager.Instance.GoToMenu(); // Evita perder referencias por cambio de escena
        }

        // Actualización de UI
        public void UpdateCoinText(int coins)
        {
            coinCountText.text = coins.ToString();
        }

        public void UpdateTimeText(float time)
        {
            timeText.text = $"{time:F2}s";
        }
    }
}