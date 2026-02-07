using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el flujo de un nivel: inicio, cronómetro, monedas, trofeos, victoria y derrota.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del LevelManager.
        /// </summary>
        public static LevelManager Instance { get; private set; }

        private int coinCount = 0;
        private bool levelEnded = false;
        private float startTime;
        private bool counting = false;
        private int levelIndex;
        private bool trophyCollected;
        [SerializeField] private GameObject tutorialPanel = null, endGamePanel;
        [SerializeField] private AudioClip beachLevelMusic, forestLevelMusic, desertLevelMusic, underwaterLevelMusic, iceLevelMusic;
        [SerializeField] private LevelsConfig levelsConfig;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia.
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Inicializa el nivel, reproduce la música según el indice del nivel y muestra
        /// el tutorial si es la primera vez que se juega.
        /// </summary>
        private void Start()
        {
            InitializeLevel(); 

            if(levelIndex <= 8)
                AudioManager.Instance.PlayMusic(beachLevelMusic);
            else if(levelIndex <= 17)
                AudioManager.Instance.PlayMusic(forestLevelMusic);
                else if(levelIndex <= 26)
                AudioManager.Instance.PlayMusic(desertLevelMusic);
                else if(levelIndex <= 35)
                AudioManager.Instance.PlayMusic(underwaterLevelMusic);
                else
                    AudioManager.Instance.PlayMusic(iceLevelMusic);
        }

        /// <summary>
        /// Actualiza el contador de tiempo si está contando.
        /// </summary>
        private void Update()
        {
            if (counting)
                UIManager.Instance.UpdateTimeText(GetElapsedTimeFormatted());
        }

        /// <summary>
        /// Inicializa el nivel y reanuda el tiempo
        /// </summary>
        private void InitializeLevel()
        {

            coinCount = 0;
            levelEnded = false;
            counting = false;

            levelIndex = SceneManager.GetActiveScene().buildIndex - 2;
            GameManager.Instance.AddTryToLevel(levelIndex);

            
            // Mostrar anuncio intersticial si es posible, si no es el nivel 0 y si no se muestra el In App Review
            if (GameManager.Instance.CanShowInGameAd() && levelIndex != 0)
            {
                GameManager.Instance.ResetLastInGameAdTime();
                GameManager.Instance.ResetTriesSiceLastAd();
                LevelPlayManager.Instance.ShowInterstitial();
            }

            ResumeTime();

            // Inicializa UI
            UIManager.Instance.ShowUIPanel();
            UIManager.Instance.UpdateCoinText(coinCount);
            UIManager.Instance.UpdateTimeText("00:00:00");
            UIManager.Instance.UpdateTriesText(GetTriesFromCurrentLevel());
            UIManager.Instance.UpdateLevelNumberText(levelIndex);

            trophyCollected = GameManager.Instance.TrophyCollected(levelIndex);

            LevelPlayManager.Instance.HideBanner();

            // Si es la primera vez que jugamos, mostramos el panel
            if(tutorialPanel != null && GameManager.Instance.GetLevels()[0].tries == 1)
            {
                UIManager.Instance.ShowTutorialPanel();
                PauseTime();
            }

            

        }

        /// <summary>
        /// Empieza a contar el tiempo.
        /// </summary>
        public void StartFlag()
        {
            startTime = Time.time;
            counting = true;
        }

        /// <summary>
        /// Para de contar el tiempo.
        /// </summary>
        public void StopFlag()
        {
            counting = false;
        }

        /// <summary>
        /// Devuelve el tiempo transcurrido.
        /// </summary>
        /// <returns>Tiempo transcurrido.</returns>
        public float GetElapsedTime()
        {
            return counting ? (Time.time - startTime) : 0f;
        }

        /// <summary>
        /// Devuelve el tiempo transcurrido en milisegundos.
        /// </summary>
        /// <returns>Tiempo transcurrido en ms.</returns>
        public long GetElapsedTimeMilis()
        {
            return (long)(GetElapsedTime() * 1000f);
        }

        /// <summary>
        /// Devuelve el tiempo transcurrido formateado.
        /// </summary>
        /// <returns>Tiempo transcurrido formateado.</returns>
        public string GetElapsedTimeFormatted()
        {
            float time = GetElapsedTime();
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int centiseconds = Mathf.RoundToInt((time * 100f) % 100f);
            return $"{minutes:00}:{seconds:00}.{centiseconds:00}";
        }

        /// <summary>
        /// Pausa el tiempo y notifica al GameManager.
        /// </summary>
        public void PauseTime(){
            Time.timeScale = 0f;
            GameManager.Instance.SetPaused(true);
        }

        /// <summary>
        /// Reanuda el tiempo y notifica al GameManager.
        /// </summary>
        public void ResumeTime()
        {
            Time.timeScale = 1f;
            GameManager.Instance.SetPaused(false);
        } 

        /// <summary>
        /// Añade monedas y actualiza la UI.
        /// </summary>
        public void AddCoin()
        {
            coinCount++;
            UIManager.Instance.UpdateCoinText(coinCount);
        }

        /// <summary>
        /// Vibra y recoge el trofeo.
        /// </summary>
        public void CollectTrophy()
        {
            VibrationManager.Instance.Vibrate();
            trophyCollected = true;
        }

        /// <summary>
        /// Marca el nivel como completado con victoria.
        /// Calcula la cantidad de estrellas obtenidas según el tiempo de finalización,
        /// muestra el panel de victoria, actualiza el racha de victorias y resetea los deaths del nivel.
        /// Guarda los datos del nivel y reporta score y logros en Android.
        /// Finalmente pausa el tiempo del juego.
        /// </summary>
        public void WinLevel()
        {
            if (levelEnded) return;
            levelEnded = true;

            float levelTime = GetElapsedTime();

            PauseTime();

            // Mostrar review en niveles específicos
            if((levelIndex == 10 || levelIndex == 19 || levelIndex == 28 || levelIndex == 37) && GameManager.Instance.GetLevels()[levelIndex].bestTime == 0f)
            {
                InAppReviewManager.Instance.LaunchReview();
            }

            int starsCount = CalculateStars(levelTime, levelIndex);
            UIManager.Instance.ShowVictoryPanel(starsCount);

            GameManager.Instance.IncrementWinStreak();
            GameManager.Instance.ResetCurrentLevelTries();

            #if UNITY_ANDROID
            LeaderboardManager.Instance.ReportScore(levelIndex, GetElapsedTimeMilis());
            AchievementManager.Instance.OnLevelCompletedAchievements(levelIndex, starsCount, GameManager.Instance.GetLevels()[levelIndex].bestTime == 0f, coinCount);
            #endif

            if(levelIndex == 44)// && GameManager.Instance.GetLevels()[44].bestTime == 0f)
            {
                UIManager.Instance.ShowEndGamePanel();
                GameManager.Instance.SaveGameEnded();
            }

            GameManager.Instance.SaveLevelData(levelIndex, starsCount, levelTime, coinCount, trophyCollected);
        }

        /// <summary>
        /// Marca el nivel como terminado por derrota.
        /// Muestra el panel de Game Over, incrementa el contador de muertes del nivel,
        /// guarda los intentos cada 5 muertes y muestra anuncios intersticiales en Android.
        /// También dispara logros de fracaso si se alcanza un número determinado de muertes.
        /// Resetea la racha de victorias y pausa el tiempo.
        /// </summary>
        public void GameOver()
        {
            if (levelEnded) return;
            levelEnded = true;

            UIManager.Instance.ShowGameOverPanel();
            
            if (GameManager.Instance.GetCurrentLevelTries() % 5 == 0)
            {
                GameManager.Instance.SaveTriesIfNeeded(levelIndex);
            }

#if UNITY_ANDROID
            if (GameManager.Instance.GetCurrentLevelTries() >= 10)
            {
                AchievementManager.Instance.onLevelFailedAchievements();
            }
#endif
            GameManager.Instance.ResetWinStreak();
            PauseTime();
        }

        /// <summary>
        /// Calcula la cantidad de estrellas obtenidas en el nivel
        /// según el tiempo transcurrido desde el inicio del nivel.
        /// </summary>
        /// <returns>Número de estrellas obtenidas.</returns>
        public int CalculateStars(float time, int levelIndex)
        {
            LevelStarConfig cfg = levelsConfig.levels[levelIndex];

            if (time <= cfg.timeFor3Stars) return 3;
            if (time <= cfg.timeFor2Stars) return 2;
            return 1;
        }

        /// <summary>
        /// Obtiene la cantidad de intentos que el jugador ha realizado en el nivel actual.
        /// </summary>
        /// <returns>Número de intentos realizados en este nivel.</returns>
        public int GetTriesFromCurrentLevel()
        {
            return GameManager.Instance.GetTriesFromLevel(levelIndex);
        }

        /// <summary>
        /// Guarda los intentos si es necesario.
        /// </summary>
        private void OnApplicationQuit()
        {
            GameManager.Instance.SaveTriesIfNeeded(levelIndex);
        }

        /// <summary>
        /// Guarda los intentos si es necesario.
        /// </summary>
        /// <param name="pauseStatus">Indica si la aplicación está pausada.</param>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                GameManager.Instance.SaveTriesIfNeeded(levelIndex);
            }
        }

        /// <summary>
        /// Guarda los intentos si es necesario y va al menú.
        /// </summary>
        public void GoToMenu()
        {
            GameManager.Instance.SaveTriesIfNeeded(levelIndex);
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
    }
}