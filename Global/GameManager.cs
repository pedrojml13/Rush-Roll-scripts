using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private BallSkin selectedBallSkin;
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    [SerializeField] List<BallSkin> skins = new List<BallSkin>();

    private int coins;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {

        StartCoroutine(WaitForAuth()); //Espera a que el usuario esté autenticado antes de cargar las monedas





        //BallController.Instance.RecalibrateTilt();

    }

    IEnumerator WaitForAuth()
    {
        while (AuthManager.currentUser == null)
            yield return null;

        PlayerDataManager.Instance.SaveUnlockedBall(0);

        PlayerDataManager.Instance.LoadCoins(loadedCoins =>
        {
            coins = loadedCoins;
            Debug.Log("Monedas cargadas: " + coins);
        });
        PlayerDataManager.Instance.LoadUnlockedBalls(unlockedIds =>
        {
            foreach (var skin in skins)
            {
                skin.isUnlocked = unlockedIds.Contains(skin.id);
            }
            Debug.Log("Skins desbloqueadas cargadas: " + unlockedIds.Count);
        });


        PlayerDataManager.Instance.LoadCurrentSkin(loadedSkinId =>
        {
            if (loadedSkinId >= 0 && loadedSkinId < skins.Count)
            {
                SetSelectedBallSkin(skins[loadedSkinId]);
            }
            Debug.Log("Skin seleccionada cargada: " + loadedSkinId);
        });

        PlayerDataManager.Instance.LoadLevels(loadedLevels =>
        {
            levels = loadedLevels;
            Debug.Log("Datos de niveles cargados: " + levels.Count);
        });

    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerDataManager.Instance.SaveCoins(coins);
    }

    public void RemoveCoins(int amount)
    {
        this.coins -= amount;
        PlayerDataManager.Instance.SaveCoins(coins);
    }


    public void UnlockSkin(int id)
    {
        skins[id].isUnlocked = true;
        PlayerDataManager.Instance.SaveUnlockedBall(id);
    }

    public bool IsSkinUnloked(int id)
    {
        return skins[id].isUnlocked;
    }

    public List<BallSkin> GetAllSkins()
    {
        return skins;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void SetSelectedBallSkin(BallSkin selectedSkin)
    {
        selectedBallSkin = selectedSkin;
        PlayerDataManager.Instance.SaveSkin(selectedSkin.id);
    }

    public BallSkin GetSelectedBallSkin()
    {
        return selectedBallSkin;
    }

    public List<LevelData> GetLevels()
    {
        return levels;
    }

    public LevelData GetLevelData(int index)
    {
        if (index >= 0 && index < levels.Count)
            return levels[index];
        else
            return null;
    }

    public void SaveLevelData(int index, int stars, float bestTime)
    {
        if (index >= 0 && index < levels.Count)
        {
            LevelData level = levels[index];
            level.unlocked = true;

            if (stars > level.stars)
                level.stars = stars;

            if (level.bestTime == 0f || bestTime < level.bestTime)
                level.bestTime = bestTime;

            // Guardar datos completos del nivel actual
            PlayerDataManager.Instance.SaveLevel(level);

            // Desbloquear siguiente nivel si existe
            int nextIndex = index + 1;
            if (nextIndex < levels.Count && !levels[nextIndex].unlocked)
            {
                levels[nextIndex].unlocked = true;

                // Guardar solo el campo "unlocked" del siguiente nivel
                PlayerDataManager.Instance.SaveLevelUnlockedOnly(nextIndex);
            }
        }
    }   


}