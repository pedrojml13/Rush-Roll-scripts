using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el sistema de logros de Google Play Games para Android.
    /// </summary>
    public class AchievementManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del LevelManager.
        /// </summary>
        public static AchievementManager Instance { get; private set; }

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
        private void Awake()
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

        /// <summary>
        /// Evalúa y reporta logros relacionados con la finalización de un nivel.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel.</param>
        /// <param name="stars">Cantidad de estrellas obtenidas.</param>
        /// <param name="firstTime">Indica si es la primera vez que se completa el nivel.</param>
        /// <param name="levelCoins">Cantidad de monedas recolectadas.</param>
        public void OnLevelCompletedAchievements(int levelIndex, int stars, bool firstTime, int levelCoins)
        {
            if (!GameManager.Instance.IsOffline())
            {
                // --- Logros de Avance ---
                if (firstTime && levelIndex == 0)
                    ReportProgress(GPGSIds.achievement_first_roll);

                if (firstTime)
                {
                    IncrementAchievement(GPGSIds.achievement_getting_serious, 1);
                    IncrementAchievement(GPGSIds.achievement_halfway_there, 1);
                    IncrementAchievement(GPGSIds.achievement_roll_master, 1);

                    if (levelIndex == 8) ReportProgress(GPGSIds.achievement_beach_explorer);
                    if (levelIndex == 17) ReportProgress(GPGSIds.achievement_forest_runner);
                    if (levelIndex == 26) ReportProgress(GPGSIds.achievement_desert_survivor);
                    if (levelIndex == 35) ReportProgress(GPGSIds.achievement_deep_diver);
                    if (levelIndex == 44) ReportProgress(GPGSIds.achievement_ice_breaker);
                }
                    
                if (levelIndex == 44 && GameManager.Instance.GetAllStars() == 135)
                {
                    ReportProgress(GPGSIds.achievement_ultimate_roller);
                }

                // --- Logros de Estrellas ---
                if(stars == 3 && GameManager.Instance.GetLevelData(levelIndex).stars < 3)
                {
                    IncrementAchievement(GPGSIds.achievement_star_rookie, 1);
                    IncrementAchievement(GPGSIds.achievement_star_pro, 1);
                    IncrementAchievement(GPGSIds.achievement_star_legend, 1);
                }                

                // --- Logros de Racha de Victorias ---
                int currentStreak = GameManager.Instance.GetWinStreak();
                if(currentStreak == 3)
                    IncrementAchievement(GPGSIds.achievement_rising_roller, 100);
                else if (currentStreak == 25)
                    IncrementAchievement(GPGSIds.achievement_fearless_roller, 100);
                else if (currentStreak == 50)
                    ReportProgress(GPGSIds.achievement_hardcore_roller);

                // --- Logros de Monedas ---
                if (GameManager.Instance.GetTotalCollectedCoins() >= 5)
                    ReportProgress(GPGSIds.achievement_first_fortune);

                IncrementAchievement(GPGSIds.achievement_coin_collector, levelCoins);
                IncrementAchievement(GPGSIds.achievement_rolling_rich, levelCoins);

                // --- Logros de Trofeos ---
                if (GameManager.Instance.TrophyCollected(4)) ReportProgress(GPGSIds.achievement_beach_trophy);
                else if (GameManager.Instance.TrophyCollected(13)) ReportProgress(GPGSIds.achievement_forest_trophy);
                else if (GameManager.Instance.TrophyCollected(22)) ReportProgress(GPGSIds.achievement_desert_trophy);
                else if (GameManager.Instance.TrophyCollected(31)) ReportProgress(GPGSIds.achievement_underwater_trophy);
                else if (GameManager.Instance.TrophyCollected(40)) ReportProgress(GPGSIds.achievement_ice_trophy);
            }
        }

        /// <summary>
        /// Reporta progreso en logros relacionados con fallar niveles.
        /// </summary>
        public void onLevelFailedAchievements()
        {
            if (!GameManager.Instance.IsOffline())
            {
                IncrementAchievement(GPGSIds.achievement_hardheaded, 100);
            }
        }

        /// <summary>
        /// Incrementa el progreso de logros basados en la eliminación de enemigos específicos.
        /// </summary>
        /// <param name="type">Tipo de enemigo derrotado.</param>
        public void OnEnemyKilledAchievements(EnemyType type)
        {
            if (!GameManager.Instance.IsOffline())
            {
                switch (type)
                {
                    case EnemyType.Crab:
                        IncrementAchievement(GPGSIds.achievement_crab_crusher, 1);
                        break;
                    case EnemyType.Spider:
                        IncrementAchievement(GPGSIds.achievement_arachnophobia, 1);
                        break;
                    case EnemyType.Scorpion:
                        IncrementAchievement(GPGSIds.achievement_scorpion_slayer, 1);
                        break;
                    case EnemyType.Jellyfish:
                        IncrementAchievement(GPGSIds.achievement_jelly_hunter, 1);
                        break;
                    case EnemyType.Snowman:
                        IncrementAchievement(GPGSIds.achievement_snow_breaker, 1);
                        break;
                }

                IncrementAchievement(GPGSIds.achievement_monster_mayhem, 1);
            }
        }

        /// <summary>
        /// Evalúa logros relacionados con la adquisición de skins en la tienda.
        /// </summary>
        public void OnSkinPurchasedAchievements()
        {
            if (!GameManager.Instance.IsOffline())
            {
                int purchased = GameManager.Instance.GetTotalPurchasedSkins();
                
                if(purchased == 1)
                    ReportProgress(GPGSIds.achievement_window_shopper);

                IncrementAchievement(GPGSIds.achievement_style_roller, 1);

                if(purchased == GameManager.Instance.GetTotalSkinsAvailable())
                    IncrementAchievement(GPGSIds.achievement_fashion_icon, 1);
            }
        }

        /// <summary>
        /// Desbloquea un logro al 100% de forma inmediata.
        /// </summary>
        /// <param name="key">ID del logro.</param>
        public void ReportProgress(string key)
        {
            if (!GameManager.Instance.IsOffline())
            {
                if (!GPGSManager.Instance.IsAuthenticated) {
                    Debug.LogWarning("User is not authenticated. Cannot unlock achievement.");
                    return;
                }

                PlayGamesPlatform.Instance.ReportProgress(key, 100.0f, success =>
                {
                    Debug.Log(success ? $"Unlocked {key}" : $"Failed to unlock {key}");
                });
            }
        }

        /// <summary>
        /// Incrementa el progreso de un logro incremental.
        /// </summary>
        /// <param name="key">ID del logro.</param>
        /// <param name="steps">Cantidad de pasos a sumar al progreso actual.</param>
        public void IncrementAchievement(string key, int steps)
        {
            if (!GameManager.Instance.IsOffline())
            {
                if (!GPGSManager.Instance.IsAuthenticated)
                {
                    Debug.LogWarning("User is not authenticated. Cannot increment achievement.");
                    return;
                }

                PlayGamesPlatform.Instance.IncrementAchievement(key, steps, success =>
                {
                    Debug.Log(success ? $"Incremented {key} by {steps}" : $"Failed to increment {key}");
                });
            }
        }

        /// <summary>
        /// Abre la interfaz de usuario nativa de logros de Google Play Games.
        /// </summary>
        public void ShowAchievementsUI()
        {
            if (!GameManager.Instance.IsOffline())
            {
                if (GPGSManager.Instance.IsAuthenticated)
                {
                    PlayGamesPlatform.Instance.ShowAchievementsUI();
                }
                else
                {
                    Debug.LogWarning("User is not authenticated. Cannot show achievements UI.");
                }
            }
        }
    }
}
