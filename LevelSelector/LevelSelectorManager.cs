using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelSelectorManager : MonoBehaviour
{
    public static LevelSelectorManager Instance { get; private set; }

    [Header("Panels de escenarios")]
    [SerializeField] private GameObject sceneryBeach;
    [SerializeField] private GameObject sceneryForest;
    [SerializeField] private GameObject sceneryDesert;

    [Header("Prefab de botón de nivel")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform beachPanel;
    [SerializeField] private Transform forestPanel;
    [SerializeField] private Transform desertPanel;

    [Header("Música por escenario")]
    [SerializeField] private AudioClip beachMusic;
    [SerializeField] private AudioClip forestMusic;
    [SerializeField] private AudioClip desertMusic;

    private int sceneryNumber = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        sceneryBeach.SetActive(true);
        sceneryForest.SetActive(false);
        sceneryDesert.SetActive(false);

        GenerateLevelButtons();
        UpdateMusic();
    }

    private void GenerateLevelButtons()
    {
        List<LevelData> levels = GameManager.Instance.GetLevels();
        int totalLevels = levels.Count;

        // Activamos todos los paneles temporalmente
        sceneryBeach.SetActive(true);
        sceneryForest.SetActive(true);
        sceneryDesert.SetActive(true);

        for (int i = 0; i < totalLevels; i++)
        {
            Transform parentPanel = GetPanelForLevel(i);
            GameObject btnObj = Instantiate(levelButtonPrefab, parentPanel);
            LevelButton btn = btnObj.GetComponent<LevelButton>();

            LevelData level = levels[i];
            btn.Setup(level.levelIndex, level.unlocked, level.stars, level.bestTime);
        }

        // Al final mostramos solo el panel inicial
        sceneryBeach.SetActive(true);
        sceneryForest.SetActive(false);
        sceneryDesert.SetActive(false);
    }


    private Transform GetPanelForLevel(int levelIndex)
    {
        if (levelIndex < 9) return beachPanel;
        if (levelIndex < 18) return forestPanel;
        return desertPanel;
    }

    private void UpdateMusic()
    {
        if (sceneryBeach.activeSelf && sceneryNumber != 1)
        {
            AudioManager.Instance.PlayMusic(beachMusic);
            sceneryNumber = 1;
        }
        else if (sceneryForest.activeSelf && sceneryNumber != 2)
        {
            AudioManager.Instance.PlayMusic(forestMusic);
            sceneryNumber = 2;
        }
        else if (sceneryDesert.activeSelf && sceneryNumber != 3)
        {
            AudioManager.Instance.PlayMusic(desertMusic);
            sceneryNumber = 3;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelIndex+1);
    }

    public void OnMenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
