using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private int coinCount = 0;
    private bool levelEnded = false;
    private float startTime;
    private bool counting = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        Time.timeScale = 1f;
        UIManager.Instance.ShowUIPanel();
        coinCount = 0;
        UIManager.Instance.UpdateCoinText(coinCount);
        UIManager.Instance.UpdateTimeText(0);
    }


    private void Update()
    {
        if (counting)
        {
            float elapsed = Time.time - startTime;
            UIManager.Instance.UpdateTimeText(elapsed);
        }
    }

    public void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void WinLevel()
    {
        if (levelEnded) return;
        levelEnded = true;

        UIManager.Instance.ShowVictoryPanel();
        GameManager.Instance.AddCoins(coinCount);

        GameManager.Instance.SaveLevelData(SceneManager.GetActiveScene().buildIndex - 1, 3, GetElapsedTime());

        PauseTime();
    }
    public void GameOver()
    {
        if (levelEnded) return;
        levelEnded = true;

        UIManager.Instance.ShowGameOverPanel();
        PauseTime();
    }

    public void AddCoin()
    {
        coinCount++;

        UIManager.Instance.UpdateCoinText(coinCount);
    }

    public void StartFlag()
    {
        startTime = Time.time;
        counting = true;
    }

    public void StopFlag()
    {
        counting = false;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public float GetElapsedTime()
    {
        if (counting)
        {
            return Time.time - startTime;
        }
        return 0f;
    }

}