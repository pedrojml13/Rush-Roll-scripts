using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System;
using Firebase.Auth;
using CandyCoded.HapticFeedback;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestor principal del juego. 
    /// Controla skins, monedas, niveles, intentos, victorias consecutivas y sesión del jugador.
    /// Singleton accesible desde cualquier parte del proyecto.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del GameManager.
        /// </summary>
        public static GameManager Instance { get; private set; }

        [SerializeField] private List<BallSkin> skins = new();
        [SerializeField] private int inGameAdsFrecuency;
        [SerializeField] private float inGameAdsMinTime;
        private BallSkin selectedBallSkin;
        private int currentLevelTries = 0; 
        private int totalSessionTries = 0;
        private int winStreak = 0;
        private bool triesChanged = false;
        private bool isPaused = false;
        private float sessionStartTime, lastAdTimestamp, lastInGameAdTimestamp;
        private int triesSinceLastAd = 0;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            sessionStartTime = Time.unscaledTime;
            lastAdTimestamp = Time.unscaledTime;
            lastInGameAdTimestamp = Time.unscaledTime;
        }

        /// <summary>
        /// Espera hasta que la sesión del jugador esté inicializada y carga la skin seleccionada y las skins desbloqueadas.
        /// </summary>
        /// <param name="onComplete">Indica si se ha completado con éxito.</param>
        public void WaitForSessionReady(Action onComplete)
        {
            SessionManager.Instance.InitializeSession(() =>
            {
                selectedBallSkin = skins[SessionManager.Instance.GetSelectedSkinId()];

                List<int> unlockedIds = SessionManager.Instance.GetUnlockedSkins();

                foreach (var skin in skins)
                {
                    skin.isUnlocked = unlockedIds.Contains(skin.id);
                }

                skins[0].isUnlocked = true; // La primera desbloqueada

                onComplete?.Invoke();
            });

        }

        /// <summary>
        /// Devuelve el tiempo de la sesión actual.
        /// </summary>
        /// <returns>Tiempo de la sesión.</returns>
        public float GetSessionTime()
        {
            return Time.unscaledTime - sessionStartTime;
        }
        
        /// <summary>
        /// Devuelve el tiempo transcurrido desde el útimo anuncio de la tienda.
        /// </summary>
        /// <returns>Tiempo transcurrido desde el último anuncio.</returns>
        public float GetLastAdTime()
        {
            return Time.unscaledTime - lastAdTimestamp;
        }

        /// <summary>
        /// Resetea el tiempo en el que se vió el último anuncio de la tienda.
        /// </summary>
        public void ResetLastAdTime()
        {
            lastAdTimestamp = Time.unscaledTime;
        }

        /// <summary>
        /// Devuelve el tiempo transcurrido desde el útimo anuncio en el nivel.
        /// </summary>
        /// <returns>Tiempo transcurrido desde el último anuncio en el nivel.</returns>
        public float GetLastInGameAdTime()
        {
            return Time.unscaledTime - lastInGameAdTimestamp;
        }

        /// <summary>
        /// Resetea el tiempo en el que se vió el último anuncio en el nivel.
        /// </summary>
        public void ResetLastInGameAdTime()
        {
            lastInGameAdTimestamp = Time.unscaledTime;
        }


        /// <summary>
        /// Devuelve el numero de intentos desde que salió un anuncio.
        /// </summary>
        /// <returns>Número de intentos desde que salió un anuncio.</returns>
        public int GetTriesSinceLastAd()
        {
            return triesSinceLastAd;
        }

        /// <summary>
        /// Resetea el contador de intentos desde que salió un anuncio.
        /// </summary>
        public void ResetTriesSiceLastAd()
        {
            triesSinceLastAd = 0;
        }

        public bool CanShowInGameAd()
        {
            if (IsSupporter()) return false;
            if (triesSinceLastAd < inGameAdsFrecuency) return false;
            if (GetLastInGameAdTime() < inGameAdsMinTime) return false;
            return true;
        }

        /// <summary>
        /// Llama al Session Manager para que guarde que el usuario finalizó el juego
        /// </summary>
        public void SaveGameEnded()
        {
            SessionManager.Instance.SaveGameEnded();
        }

        /// <summary>
        /// Devuelve si el usuario finalizó el juego
        /// </summary>
        /// <returns>Si el usuario finalizó el juego.</returns>
        public bool IsGameEnded()
        {
            return SessionManager.Instance.IsGameEnded();
        }

        /// <summary>
        /// Comprueba si tiene conexión a internet
        /// </summary>
        /// <returns>Si hay conexión a internet.</returns>
        public bool HasInternet()
        {
            return SessionManager.Instance.HasInternet();
        }

        /// <summary>
        /// Devuelve el tiempo total jugado
        /// </summary>
        /// <returns>Tiempo total jugado.</returns>
        public float GetTotalPlayedTime()
        {
            return SessionManager.Instance.GetTotalPlayedTime();
        }

        /// <summary>
        /// Al cerrar la aplicacion guarda el tiempo de la sesión actual
        /// </summary>
        /*void OnApplicationQuit()
        {
            SessionManager.Instance.SaveTotalPlayedTime(sessionTime);
        }*/

        /// <summary>
        /// Devuelve si el juego está pausado
        /// </summary>
        /// <returns>Si el juego está pausado.</returns>
        public bool IsPaused()
        {
            return isPaused;
        }

        /// <summary>
        /// Pausa o reanuda el juego.
        /// </summary>
        /// <param name="paused">Si hay que pausar.</param>
        public void SetPaused(bool paused)
        {
            isPaused = paused;
        }

        /// <summary>
        /// Obtiene el usuario actual autenticado a través del Session Manager.
        /// </summary>
        /// <returns>Usuario actual.</returns>
        public FirebaseUser GetCurrentUser()
        {
            return SessionManager.Instance.GetCurrentUser();
        }

        /// <summary>
        /// Obtiene la cantidad actual de monedas del jugador a traves del SessionManager.
        /// </summary>
        /// <returns>Cantidad de monedas.</returns>
        public int GetCoins(){
            return SessionManager.Instance.GetCoins();
        }

        /// <summary>
        /// Obtiene el total de monedas recogidas a lo largo del juego a través del SessionManager.
        /// </summary>
        /// <returns>Cantidad total de monedas recolectadas.</returns>
        public int GetTotalCollectedCoins(){
            return SessionManager.Instance.GetTotalCollectedCoins();
        }

        /// <summary>
        /// Comprueba si un trofeo fue recogido en un nivel dado a través del SessionManager.
        /// </summary>
        /// <returns>Si el trofeo se ha recogido.</returns>
        public bool TrophyCollected(int levelIndex)
        {
            return SessionManager.Instance.TrophyCollected(levelIndex);
        }

        /// <summary>
        /// Obtiene la racha de victorias consecutivas.
        /// </summary>
        /// <returns>Racha de victorias.</returns>
        public int GetWinStreak(){
            return  winStreak;
        }

        /// <summary>
        /// Incrementa la racha de victorias consecutivas.
        /// </summary>
        public void IncrementWinStreak(){
            winStreak++;
        }

        /// <summary>
        /// Reinicia la racha de victorias consecutivas.
        /// </summary>
        public void ResetWinStreak()
        {
            winStreak = 0;
        }

        /// <summary>
        /// Comprueba si una skin está desbloqueada.
        /// </summary>
        /// <returns>Si la skin está desbloqueada.</returns>
        public bool IsSkinUnloked(int id)
        {
            return skins[id].isUnlocked;
        }

        /// <summary>
        /// Devuelve todas las skins disponibles.
        /// </summary>
        /// <returns>Lista de skins.</returns>
        public List<BallSkin> GetAllSkins(){
            return skins;
        }

        /// <summary>
        /// Devuelve el número total de skins.
        /// </summary>
        /// <returns>Número de skins.</returns>
        public int GetTotalSkinsAvailable(){
            return skins.Count;
        }

        /// <summary>
        /// Devuelve el número de skins desbloqueadas.
        /// </summary>
        /// <returns>Numero de skins desbloqueadas.</returns>
        public int GetTotalPurchasedSkins()
        {
            int count = 0;
            foreach (var skin in skins)
                if (skin.isUnlocked) count++;
            return count;
        }

        /// <summary>
        /// Compra una skin, actualiza monedas, selecciona la skin comprada y notifica al SessionManager.
        /// </summary>
        /// <param name="id">Id de la skin.</param>
        /// <param name="price">Precio de la skin.</param>
        public void BuySkin(int id, int price)
        {
            if (id < 0 || id >= skins.Count) return;

            BallSkin skin = skins[id];
            if (skin.isUnlocked)
            {
                Debug.Log("La bola ya está desbloqueada.");
                SelectSkin(id);
                return;
            }

            if (SessionManager.Instance.GetCoins() < price)
            {
                Debug.LogWarning("No tienes suficientes monedas para comprar esta bola.");
                return;
            }

            SessionManager.Instance.BuySkin(id, price);
            skin.isUnlocked = true;
            selectedBallSkin = skin;
        }

        /// <summary>
        /// Selecciona una skin ya desbloqueada y notifica al SessionManager.
        /// </summary>
        /// <param name="id">Id de la skin.</param>
        public void SelectSkin(int id)
        {
            if (id < 0 || id >= skins.Count) return;

            BallSkin skin = skins[id];
            if (!skin.isUnlocked)
            {
                Debug.LogWarning("No puedes seleccionar una bola que no has desbloqueado.");
                return;
            }

            SessionManager.Instance.SetSelectedSkinId(id);
            selectedBallSkin = skin;
        }

        /// <summary>
        /// Establece el nombre de usuario mediante el SessionManager.
        /// </summary>
        /// <param name="newUsername">Nombre de usuario.</param>
        public void SetUsername(string newUsername)
        {
            SessionManager.Instance.SetUsername(newUsername);
        }
           
        /// <summary>
        /// Obtiene el nombre de usuario actual mediante el SessionManager.
        /// </summary>
        /// <returns>Nombre de usuario.</returns>
        public string GetUsername()
        {
            return  SessionManager.Instance.GetUsername();
        }

        /// <summary>
        /// Intenta establecer un nombre de usuario verificando disponibilidad a través del SessionManager.
        /// </summary>
        /// <param name="desiredName">Nombre de usuario.</param>
        /// <param name="onComplete">Indica si se ha completado con éxito.</param>
        public void TrySetUsername(string desiredName, Action<bool> onComplete)
        {
            SessionManager.Instance.CheckUsernameAvailability(desiredName, isAvailable =>
            {
                if (!isAvailable)
                {
                    Debug.LogWarning("Nombre de usuario ya en uso.");
                    onComplete?.Invoke(false);
                    return;
                }

                SetUsername(desiredName);
                onComplete?.Invoke(true);
                Debug.Log("Nombre guardado correctamente: " + desiredName);
            });
        }

        /// <summary>
        /// Obtiene la skin actualmente seleccionada.
        /// </summary>
        /// <returns>Skin seleccionada.</returns>
        public BallSkin GetSelectedBallSkin()
        {
            return  selectedBallSkin;
        }

        /// <summary>
        /// Obtiene la lista de niveles del jugador a través del SessionManager.
        /// </summary>
        /// <returns>Lista de niveles.</returns>
        public List<LevelData> GetLevels()
        {
            return SessionManager.Instance.GetLevels();
        }

        /// <summary>
        /// Obtiene los datos de un nivel específico a través del SessionManager.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        /// <returns>Datos del nivel.</returns>
        public LevelData GetLevelData(int index)
        {
            return SessionManager.Instance.GetLevelData(index);
        }

        /// <summary>
        /// Devuelve el total de estrellas conseguidas por el jugador a través del SessionManager.
        /// </summary>
        /// /// <returns>Total de estrellas.</returns>
        public int GetAllStars()
        {
            return SessionManager.Instance.GetAllStars();
        }

        /// <summary>
        /// Obtiene los intentos realizados en un nivel a través del SessionManager.
        /// </summary>
        /// <returns>Intentos del nivel.</returns>
        public int GetTriesFromLevel(int index)
        {
            return SessionManager.Instance.GetTriesFromLevel(index);
        }

        /// <summary>
        /// Incrementa los intentos realizados en un nivel y notifica al SessionManager.
        /// </summary>
        /// <param name="index">Indice del nivel</param>
        public void AddTryToLevel(int index)
        {
            triesChanged = true;
            SessionManager.Instance.AddTryToLevel(index);
            currentLevelTries++;
            totalSessionTries++;
            triesSinceLastAd++;
        }

        /// <summary>
        /// Obtiene los intentos del nivel actual.
        /// </summary>
        /// <returns>Intentos del nivel actual.</returns>
        public int GetCurrentLevelTries()
        {
            return currentLevelTries;
        }

        /// <summary>
        /// Obtiene el total de intentos de la sesión actual.
        /// </summary>
        /// <returns>Total de intentos.</returns>
        public int GetTotalSessionTries()
        {
            return totalSessionTries;
        }

        /// <summary>
        /// Reinicia el contador de muertes del nivel actual.
        /// </summary>
        public void ResetCurrentLevelTries(){
            currentLevelTries = 0;
        }

        /// <summary>
        /// Si los intentos han cambiado, guarda los intentos del nivel y notifica que los intentos no han cambiado.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel donde guardar los intentos.</param>
        public void SaveTriesIfNeeded(int levelIndex)
        {
            if (!triesChanged) return;

            SaveTries(levelIndex);
            triesChanged = false;
        }

        /// <summary>
        /// Guarda los intentos de un nivel a través del SessionManager.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        public void SaveTries(int index)
        {
            SessionManager.Instance.SaveTries(index);
        }

        /// <summary>
        /// Guarda que es Supporter a través del SessionManager.
        /// </summary>
        public void SaveIsSupporter()
        {
            SessionManager.Instance.SaveIsSupporter();
        }

        /// <summary>
        /// Obtiene si es Supporter a través del SessionManager.
        /// </summary>
        /// <returns>Si es Supporter.</returns>
        public bool IsSupporter()
        {
            return SessionManager.Instance.IsSupporter();
        }

        /// <summary>
        /// Guarda los datos de un nivel y actualiza ranking global si el dispositivo no es Android mediante el SessionManager.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        /// <param name="stars">Total de estrellas.</param>
        /// <param name="bestTime">Mejor tiempo.</param>
        /// <param name="coinsEarned">Monedas recolectadas.</param>
        /// <param name="trophyCollected">Si el trofeo se ha recogido.</param>
        public void SaveLevelData(int index, int stars, float bestTime, int coinsEarned, bool trophyCollected)
        {
            SessionManager.Instance.SaveLevelData(index, stars, bestTime, coinsEarned, trophyCollected);

            #if !UNITY_ANDROID
            SessionManager.Instance.UpdateGlobalRankingIfBetter(index, bestTime);
            #endif
        }

        /// <summary>
        /// Añade monedas al jugador a través del SessionManager.
        /// </summary>
        /// <param name="amount">Cantidad de monedas a añadir.</param>
        public void AddCoins(int amount)
        {
            SessionManager.Instance.AddCoins(amount);
        }

        /// <summary>
        /// Obtiene los datos de ranking global de niveles a traves del SessionManager.
        /// </summary>
        /// <param name="callback">Indica si se ha completado con éxito.</param>
        public void GetRankingLevelData(Action<Dictionary<int, (string playerName, float bestTime)>> callback)
        {
            SessionManager.Instance.GetRankingLevelData(callback);
        }
            

        /// <summary>
        /// Cierra sesión del jugador a traves del SessionManager.
        /// </summary>
        public void LogOut()
        {
            SessionManager.Instance.LogOut();
        }
    }
}
