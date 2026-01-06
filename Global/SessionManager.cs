using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.Networking;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestor de sesión del jugador.
    /// Mantiene los datos del jugador en memoria, sincroniza con Firebase y controla la sesión.
    /// Singleton accesible desde cualquier parte del proyecto.
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del SessionManager.
        /// </summary>
        public static SessionManager Instance { get; private set; }

        /// <summary>
        /// Indica si los datos de la sesión están listos para ser usados.
        /// </summary>
        public bool IsReady { get; private set; } = false;

        private string username;
        private int coins;
        private int totalCollectedCoins;
        private List<LevelData> levels;
        private Dictionary<int, (string playerName, float bestTime)> levelRankings;
        private List<int> unlockedSkins;
        private int selectedSkinId;
        private bool isSupporter;
        private bool gameEnded;
        private bool hasInternet;
        private float totalPlayedTime;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
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
            hasInternet = (Application.internetReachability != NetworkReachability.NotReachable);
        }

        /// <summary>
        /// Carga todos los datos del jugador y rankings globales. Si no hay conexión carga datos locales.
        /// </summary>
        /// <param name="onComplete">Indica cuando está lista la sesión.</param>
        public void InitializeSession(Action onComplete)
        {

            //Caching.ClearCache();

            // Si no tiene internet, carga los datos locales.
            if (!hasInternet)
            {
                LoadLocalData();
                IsReady = true;
                onComplete?.Invoke();
            }

            // Si tiene internet carga los datos desde firestone a traves del PlayerDataManager.
            else
            {
                PlayerDataManager.Instance.LoadFullPlayerData(profile =>
                {
                    if (profile == null)
                    {
                        LoadLocalData();
                        IsReady = true;
                        onComplete?.Invoke();
                        return;
                    }

                    username = profile.username;
                    coins = profile.coins;
                    totalCollectedCoins = profile.totalCollectedCoins;

                    isSupporter = profile.isSupporter;

                    selectedSkinId = profile.currentSkinId;
                    unlockedSkins = profile.unlockedSkinIds;
                    levels = profile.levels;
                    totalPlayedTime = profile.totalPlayedTime;

                    // Si el dispositivo no es android, carga los rankings desde firestone.
                    #if !UNITY_ANDROID
                    PlayerDataManager.Instance.LoadGlobalRankings(rankings =>
                    {
                        levelRankings = rankings;
                        IsReady = true;
                        onComplete?.Invoke();
                    });
                    #else
                    IsReady = true;
                    onComplete?.Invoke();
                    #endif
                });
                return;
            }
        }

        /// <summary>
        /// Devuelve si tiene internet.
        /// </summary>
        public bool HasInternet()
        {
            return hasInternet = (Application.internetReachability != NetworkReachability.NotReachable);
        }

        /// <summary>
        /// Guarda que es Supporter a través de PlayerDataManager.
        /// </summary>
        public void SaveIsSupporter()
        {
            PlayerDataManager.Instance.SaveIsSupporter();
        }

        /// <summary>
        /// Devuelve si es Supporter.
        /// </summary>
        /// <returns>Si es supporter</returns>
        public bool IsSupporter()
        {
            return isSupporter;
        }

        /// <summary>
        /// Guarda que ha finalizado el juego a través de PlayerDataManager.
        /// </summary>
        public void SaveGameEnded()
        {
            PlayerDataManager.Instance.SaveGameEnded();
        }

        /// <summary>
        /// Obtiene si el usuario a finalizado el juego.
        /// </summary>
        /// <returns>Si ha finalizado el juego</returns>
        public bool IsGameEnded()
        {
            return gameEnded;
        }

        /// <summary>
        /// Devuelve el total del tiempo jugado.
        /// </summary>
        /// <returns>Total del tiempo jugado</returns>
        public float GetTotalPlayedTime()
        {
            return totalPlayedTime;
        }

        /// <summary>
        /// Guarda el tiempo total jugado.
        /// </summary>
        /// <param name="time">Tiempo a sumar al total jugado.</param>
        public void SaveTotalPlayedTime(float time)
        {
            totalPlayedTime += time;
            PlayerDataManager.Instance.SaveTotalPlayedTime(totalPlayedTime);
        }


        /// <summary>
        /// Carga todos los datos locales del jugador.
        /// </summary>
        void LoadLocalData()
        {
            PlayerProfileData local = LocalSaveSystem.LoadLocal();

            username = local.username;
            coins = local.coins;
            totalCollectedCoins = local.totalCollectedCoins;
            selectedSkinId = local.currentSkinId;
            unlockedSkins = local.unlockedSkinIds;
            levels = local.levels;
            gameEnded = local.gameEnded;

            IsReady = true;
        }

        /// <summary>
        /// Guarda todos los datos locales del jugador.
        /// </summary>
        public void SaveLocalData()
        {
            PlayerProfileData localData = new PlayerProfileData()
            {
                username = this.username,
                coins = this.coins,
                totalCollectedCoins = this.totalCollectedCoins,
                currentSkinId = this.selectedSkinId,
                unlockedSkinIds = this.unlockedSkins,
                levels = this.levels,
                gameEnded = this.gameEnded,
                isSupporter = this.isSupporter
            };

            LocalSaveSystem.SaveLocal(localData);
        }

        #region Getters
        /// <summary>
        /// Devuelve el nombre de usuario.
        /// </summary>
        /// <returns>Nombre de usuario</returns>
        public string GetUsername()
        {
            return username;
        }
        
        /// <summary>
        /// Devuelve las monedas.
        /// </summary>
        /// <returns>Monedas.</returns>
        public int GetCoins(){
            return coins;
        }

        /// <summary>
        /// Devuelve el total de monedas recogidas.
        /// </summary>
        /// <returns>Total de monedas recogidas.</returns>
        public int GetTotalCollectedCoins(){
            return totalCollectedCoins;
        }

        /// <summary>
        /// Devuelve la lista de niveles.
        /// </summary>
        /// <returns>Lista de niveles.</returns>
        public List<LevelData> GetLevels(){
            return levels;
        }

        /// <summary>
        /// Devuelve el total de estrellas conseguidas en todos los niveles.
        /// </summary>
        /// <returns>total de estrellas.</returns>
        public int GetAllStars()
        {
            int totalStars = 0;
            foreach (var level in levels)
                totalStars += level.stars;
            return totalStars;
        }

        /// <summary>
        /// Devuelve los intentos de un nivel específico.
        /// </summary>
        /// <param name="index">Indice del nivel a consultar.</param>
        /// <returns>Intentos del nivel.</returns>
        public int GetTriesFromLevel(int index)
        {
            return levels[index].tries;
        }

        /// <summary>
        /// Devuelve un diccionario con los Rankings.
        /// </summary>
        /// <returns>Rankings.</returns>
        public Dictionary<int, (string playerName, float bestTime)> GetLevelRankings()
        {
            return levelRankings;
        }

        /// <summary>
        /// Devuelve una lista con las skins desbloqueadas.
        /// </summary>
        /// <returns>Skins desbloqueadas.</returns>
        public List<int> GetUnlockedSkins()
        {
            return unlockedSkins;
        }

        /// <summary>
        /// Devuelve el id de la skin seleccionada.
        /// </summary>
        /// <returns>Id de la skin seleccionada</returns>
        public int GetSelectedSkinId()
        {
            return selectedSkinId;
        }

        /// <summary>
        /// Devuelve el usuario actual.
        /// </summary>
        /// <returns>Usuario actual.</returns>
        public FirebaseUser GetCurrentUser()
        {
            return AuthManager.Instance.GetCurrentUser();
        }
        #endregion

        #region Setters / Updates

        /// <summary>
        /// Guarda el nombre de usuario a través de PlayerDataManager si hay internet o en local si no hay
        /// </summary>
        /// <param name="newUsername">Nombre de usuario a guardar.</param>
        public void SetUsername(string newUsername)
        {
            username = newUsername;
            if(hasInternet)
                PlayerDataManager.Instance.SaveUsername(newUsername);
            else
                SaveLocalData();
        }

        /// <summary>
        /// Comprueba si el nombre de usuario esta disponible a través de PlayerDataManager si hay internet.
        /// </summary>
        /// <param name="name">Nombre de usuario a comprobar.</param>
        /// <param name="callback">Callback que indica si el nombre está disponible.</param>
        public void CheckUsernameAvailability(string name, Action<bool> callback)
        {
            if(hasInternet)
                PlayerDataManager.Instance.CheckUsernameAvailability(name, callback);
            else
                SaveLocalData();
        }

        /// <summary>
        /// Guarda la skin seleccionada a través de PlayerDataManager si hay internet o en local si no hay.
        /// </summary>
        /// <param name="id">Id de la skin a seleccionar.</param>
        public void SetSelectedSkinId(int id)
        {
            selectedSkinId = id;
            if(hasInternet)
                PlayerDataManager.Instance.SelectSkin(id);
            else
                SaveLocalData();
        }

        /// <summary>
        /// Resta una cantidad de monedas al total y lo guarda a través de PlayerDataManager si hay internet o en local si no hay.
        /// </summary>
        /// <param name="amount">Cantidad de monedas a restar.</param>
        public void SpendCoins(int amount)
        {
            coins -= amount;
            if(hasInternet)
                PlayerDataManager.Instance.SaveCoins(coins);
            else
                SaveLocalData();
        }

        /// <summary>
        /// Añade monedas al total y lo guarda a través de PlayerDataManager si hay internet o en local si no hay.
        /// </summary>
        /// <param name="amount">Cantidad de monedas a sumar.</param>
        public void AddCoins(int amount)
        {
            coins += amount;
            totalCollectedCoins += amount;
            if(hasInternet)
                PlayerDataManager.Instance.SaveCoins(coins);
            else
                SaveLocalData();
        }

        /// <summary>
        /// Resta el precio de la skin al total de monedas, selecciona la skin, la desbloquea y lo guarda a través de PlayerDataManager
        /// si hay internet o en local si no hay.
        /// </summary>
        /// <param name="id">Id de la skin a comprar.</param>
        /// <param name="price">Precio de la skin.</param>
        public void BuySkin(int id, int price)
        {
            coins -= price;
            selectedSkinId = id;

            if (!unlockedSkins.Contains(id))
                unlockedSkins.Add(id);

            if(hasInternet)
                PlayerDataManager.Instance.BuySkin(id, coins);
            else
                SaveLocalData();
        }
        #endregion

        #region Level Data
        /// <summary>
        /// Devuelve los datos de un nivel.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        /// <returns>Datos del nivel a consultar.</returns>
        public LevelData GetLevelData(int index)
        {
            return levels[index];
        }

        /// <summary>
        /// Añade un intento a un nivel.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        public void AddTryToLevel(int index)
        {
            var level = GetLevelData(index);
            if (level != null)
                level.tries++;
        }

        /// <summary>
        /// Devuelve si se ha recogido el trofeo de un nivel.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel.</param>
        /// <returns>Si se ha recogido el trofeo.</returns>
        public bool TrophyCollected(int levelIndex)
        {
            return levels[levelIndex].trophyCollected;
        }

        /// <summary>
        /// Guarda los intentos de un nivel.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        public void SaveTries(int index)
        {
            var level = GetLevelData(index);
            if(hasInternet)
                PlayerDataManager.Instance.SaveLevelTries(index, level.tries);
            else
                SaveLocalData();
        }

        /// <summary>
        /// Guarda los datos de un nivel.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        /// <param name="stars">Estrellas conseguidas.</param>
        /// <param name="bestTime">Mejor tiempo.</param>
        /// <param name="coinsEarned">Monedas recolectadas.</param>
        /// <param name="trophyCollected">Si se ha recogido ek trofeo.</param>
        public void SaveLevelData(int index, int stars, float bestTime, int coinsEarned, bool trophyCollected)
        {
            var level = GetLevelData(index);
            if (level == null) return;

            if (stars > level.stars) level.stars = stars;
            if (level.bestTime == 0f || bestTime < level.bestTime) level.bestTime = bestTime;

            level.trophyCollected = trophyCollected;
            AddCoins(coinsEarned);

            int nextIndex = index + 1;
            if (nextIndex < levels.Count) levels[nextIndex].unlocked = true;

            if(hasInternet)
                PlayerDataManager.Instance.SaveLevelData(
                    levelIndex: index,
                    stars: level.stars,
                    bestTime: level.bestTime,
                    tries: level.tries,
                    totalLevels: levels.Count,
                    coins: coins,
                    totalCollectedCoins: totalCollectedCoins,
                    trophyCollected: trophyCollected
                );
            else
                SaveLocalData();
        }

        /// <summary>
        /// Actualiza el ranking global si el tiempo es mejor.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel.</param>
        /// <param name="bestTime">Mejor tiempo.</param>
        public void UpdateGlobalRankingIfBetter(int levelIndex, float bestTime)
        {
            if (levelRankings == null) return;

            if (levelRankings.TryGetValue(levelIndex, out var current))
            {
                if (bestTime < current.bestTime)
                {
                    SaveGlobalBestTime(levelIndex, bestTime, username);
                    levelRankings[levelIndex] = (username, bestTime);
                }
            }
            else
            {
                SaveGlobalBestTime(levelIndex, bestTime, username);
                levelRankings[levelIndex] = (username, bestTime);
            }
        }

        /// <summary>
        /// Guarda el mejor tiempo global.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel.</param>
        /// <param name="bestTime">Mejor tiempo.</param>
        /// <param name="playerName">Nombre del jugador.</param>
        private void SaveGlobalBestTime(int levelIndex, float bestTime, string playerName)
        {
            if(hasInternet)
                PlayerDataManager.Instance.SaveGlobalBestTime(levelIndex, bestTime, playerName, success =>
                {
                    if (success)
                        Debug.Log($"Nuevo récord global guardado para nivel {levelIndex}: {bestTime}");
                    else
                        Debug.LogWarning("No se pudo guardar el récord global.");
                });
            else
                SaveLocalData();
        }

        /// <summary>
        /// Obtiene los rankings globales de niveles si la sesión ya está lista.
        /// </summary>
        /// <param name="callback">Devuelve los rankings.</param>
        public void GetRankingLevelData(Action<Dictionary<int, (string playerName, float bestTime)>> callback)
        {
            if (IsReady && levelRankings != null)
                callback(levelRankings);
            else
                Debug.LogWarning("Ranking global aún no disponible.");
        }
        #endregion

        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        public void LogOut()
        {
            AuthManager.Instance.LogOut();
        }
    }
}
