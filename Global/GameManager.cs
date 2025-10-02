using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private List<LevelData> levels = new();
        [SerializeField] private List<BallSkin> skins = new();

        private BallSkin selectedBallSkin;
        private int coins;

        public string playerName;
        public bool isOffline;

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

        void Start()
        {
            //playerName = PlayerDataManager.LoadUsername();
        }

        public void OnLoginButton()
        {
            // Espera a que el usuario esté autenticado antes de cargar datos
            StartCoroutine(WaitForAuth());

            //BallController.Instance.RecalibrateTilt(); // Activar si lo necesitas
        }

        IEnumerator WaitForAuth()
        {
            while (AuthManager.currentUser == null)
                yield return null;

            PlayerDataManager.Instance.SaveUnlockedBall(0);

            // Carga nombre de usuario
            PlayerDataManager.Instance.LoadUsername(username =>
            {
                playerName = username;
                Debug.Log("Nombre de usuario cargado: " + username);
            });

            // Carga monedas
            PlayerDataManager.Instance.LoadCoins(loadedCoins =>
            {
                coins = loadedCoins;
                Debug.Log("Monedas cargadas: " + coins);
            });

            // Carga skins desbloqueadas
            PlayerDataManager.Instance.LoadUnlockedBalls(unlockedIds =>
            {
                foreach (var skin in skins)
                    skin.isUnlocked = unlockedIds.Contains(skin.id);

                Debug.Log("Skins desbloqueadas cargadas: " + unlockedIds.Count);
            });

            // Carga skin seleccionada
            PlayerDataManager.Instance.LoadCurrentSkin(loadedSkinId =>
            {
                if (loadedSkinId >= 0 && loadedSkinId < skins.Count)
                    SetSelectedBallSkin(skins[loadedSkinId]);

                Debug.Log("Skin seleccionada cargada: " + loadedSkinId);
            });

            // Carga datos de niveles
            PlayerDataManager.Instance.LoadLevels(loadedLevels =>
            {
                levels = loadedLevels;
                Debug.Log("Datos de niveles cargados: " + levels.Count);
            });
        }

        // Monedas
        public void AddCoins(int amount)
        {
            coins += amount;
            PlayerDataManager.Instance.SaveCoins(coins);
        }

        public void RemoveCoins(int amount)
        {
            coins -= amount;
            PlayerDataManager.Instance.SaveCoins(coins);
        }

        public int GetCoins() => coins;

        // Skins
        public void UnlockSkin(int id)
        {
            if (id >= 0 && id < skins.Count)
            {
                skins[id].isUnlocked = true;
                PlayerDataManager.Instance.SaveUnlockedBall(id);
            }
        }

        public bool IsSkinUnloked(int id)
        {
            return id >= 0 && id < skins.Count && skins[id].isUnlocked;
        }

        public List<BallSkin> GetAllSkins() => skins;

        public void SetSelectedBallSkin(BallSkin selectedSkin)
        {
            selectedBallSkin = selectedSkin;
            PlayerDataManager.Instance.SaveSkin(selectedSkin.id);
        }

        public BallSkin GetSelectedBallSkin() => selectedBallSkin;

        // Niveles
        public List<LevelData> GetLevels() => levels;

        public LevelData GetLevelData(int index)
        {
            return index >= 0 && index < levels.Count ? levels[index] : null;
        }

        public void SaveLevelData(int index, int stars, float bestTime)
        {
            if (index < 0 || index >= levels.Count) return;

            LevelData level = levels[index];
            level.unlocked = true;

            if (stars > level.stars)
                level.stars = stars;

            if (level.bestTime == 0f || bestTime < level.bestTime)
                level.bestTime = bestTime;

            PlayerDataManager.Instance.SaveLevel(level);

            // Desbloquea siguiente nivel si existe
            int nextIndex = index + 1;
            if (nextIndex < levels.Count && !levels[nextIndex].unlocked)
            {
                levels[nextIndex].unlocked = true;
                PlayerDataManager.Instance.SaveLevelUnlockedOnly(nextIndex);
            }
        }
    }
}