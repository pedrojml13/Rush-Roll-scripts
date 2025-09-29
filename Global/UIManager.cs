using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI coinCountText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI recalibrateText;

    public static UIManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowVictoryPanel()
    {
        victoryPanel.SetActive(true);
        UIPanel.SetActive(false);
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

    public void OnPauseButton()
    {
        pausePanel.SetActive(true);
        UIPanel.SetActive(false);
        LevelManager.Instance.PauseTime();
    }

    public void OnRecalibrateButton()
    {
        if (BallController.Instance != null)
        {
            BallController.Instance.RecalibrateTilt();
            recalibrateText.text = "Recalibrated!";
            StartCoroutine(ClearTextAfterDelay(1f));

        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        recalibrateText.text = "";
    }



    public void OnContinueButton()
    {
        pausePanel.SetActive(false);
        UIPanel.SetActive(true);
        LevelManager.Instance.ResumeTime();
    }
    public void OnNextLevelButton()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;

        currentLevel++;

        if (currentLevel > SceneManager.sceneCountInBuildSettings - 1)
        {
            currentLevel = 1;
        }

        HideAllPanels();
        SceneManager.LoadScene(currentLevel);
    }

    public void OnReplayButton()
    {
        HideAllPanels();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMenuButton()
    {
        HideAllPanels();
        //SceneManager.LoadScene("Menu"); --> Reemplazado por LevelManager porque se perdían las referencias al cambiar de escena ya que UIManager es DontDestroyOnLoad
        LevelManager.Instance.GoToMenu();
    }

    public void UpdateCoinText(int coins)
    {
        coinCountText.text = coins.ToString();
    }

    public void UpdateTimeText(float time)
    {
        timeText.text = $"Time: {time:F2}s";
    }
}
