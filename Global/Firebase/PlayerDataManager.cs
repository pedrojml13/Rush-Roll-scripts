using UnityEngine;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;
using System;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona toda la persistencia de datos del jugador en Firestore.
    /// Incluye perfil, monedas, skins, progreso de niveles y rankings globales.
    /// Implementa el patrón Singleton y persiste entre escenas.
    /// </summary>
    public class PlayerDataManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del PlayerDataManager.
        /// </summary>
        public static PlayerDataManager Instance { get; private set; }

        private FirebaseFirestore db;

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
        /// Si tiene acceso a internet, se conecta a la base de datos
        /// </summary>
        private void Start()
        {
            if((Application.internetReachability != NetworkReachability.NotReachable))
                db = FirebaseFirestore.DefaultInstance;
        }

        /// <summary>
        /// Devuelve la referencia al documento del usuario autenticado.
        /// </summary>
        /// <returns>Referencia al documento de usuario.</returns>
        private DocumentReference GetUserDoc()
        {
            string uid = AuthManager.Instance.GetUserId();
            if (string.IsNullOrEmpty(uid))
            {
                Debug.LogWarning("No hay usuario autenticado.");
                return null;
            }

            return db.Collection("users").Document(uid);
        }

        #region Player Data

        /// <summary>
        /// Carga el perfil completo del jugador en una sola lectura.
        /// Inicializa valores por defecto si el documento no existe.
        /// </summary>
        /// <param name="callback">Devuelve la información cargada.</param>
        public void LoadFullPlayerData(Action<PlayerProfileData> callback)
        {
            Debug.Log("[Firestore] Lectura iniciada: PlayerData");

            DocumentReference docRef = GetUserDoc();
            if (docRef == null)
            {
                callback?.Invoke(null);
                return;
            }

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                // Perfil por defecto
                var profile = new PlayerProfileData
                {
                    username = "",
                    coins = 0,
                    totalCollectedCoins = 0,
                    isSupporter = false,
                    currentSkinId = 0,
                    unlockedSkinIds = new List<int>(),
                    levels = new List<LevelData>(),
                    gameEnded = false,
                    totalPlayedTime = 0f
                };

                int totalLevels = 45;
                for (int i = 0; i < totalLevels; i++)
                {
                    profile.levels.Add(new LevelData(i)
                    {
                        unlocked = (i == 0), // Primer nivel desbloqueado
                        stars = 0,
                        bestTime = 0f,
                        tries = 0
                    });
                }

                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    var snapshot = task.Result;

                    if (snapshot.ContainsField("username"))
                        profile.username = snapshot.GetValue<string>("username");

                    if (snapshot.ContainsField("coins"))
                        profile.coins = (int)(long)snapshot.GetValue<object>("coins");

                    if (snapshot.ContainsField("totalCollectedCoins"))
                        profile.totalCollectedCoins = (int)(long)snapshot.GetValue<object>("totalCollectedCoins");
                    if (snapshot.ContainsField("isSupporter"))
                        profile.isSupporter = snapshot.GetValue<bool>("isSupporter");
                    if (snapshot.ContainsField("currentSkin"))
                        profile.currentSkinId = (int)(long)snapshot.GetValue<object>("currentSkin");

                    if (snapshot.ContainsField("unlockedBalls"))
                        profile.unlockedSkinIds =
                            snapshot.GetValue<List<object>>("unlockedBalls")
                                    .ConvertAll(o => (int)(long)o);

                    if (snapshot.ContainsField("levels"))
                    {
                        var rawLevels = snapshot.GetValue<Dictionary<string, object>>("levels");
                        foreach (var kvp in rawLevels)
                        {
                            int index = int.Parse(kvp.Key);
                            if (kvp.Value is Dictionary<string, object> val)
                            {
                                var level = profile.levels[index];
                                if (val.ContainsKey("unlocked")) level.unlocked = (bool)val["unlocked"];
                                if (val.ContainsKey("stars")) level.stars = (int)(long)val["stars"];
                                if (val.ContainsKey("bestTime")) level.bestTime = (float)(double)val["bestTime"];
                                if (val.ContainsKey("tries")) level.tries = (int)(long)val["tries"];
                            }
                        }
                    }
                    if (snapshot.ContainsField("gameEnded"))
                        profile.gameEnded = snapshot.GetValue<bool>("gameEnded");
                    if (snapshot.ContainsField("totalPlayedTime"))
                        profile.coins = (int)(long)snapshot.GetValue<object>("totalPlayedTime");
                }

                callback?.Invoke(profile);
            });
        }


        #endregion

        #region Username

        /// <summary>
        /// Comprueba si un nombre de usuario está disponible en la colección global de usernames.
        /// </summary>
        /// <param name="username">Nombre a comprobar.</param>
        /// <param name="callback">Devuelve si está disponible.</param>
        public void CheckUsernameAvailability(string username, Action<bool> callback)
        {
            Debug.Log("[Firestore] Lectura iniciada: CheckUsernameAvailability");

            if (string.IsNullOrEmpty(username))
            {
                callback(false);
                return;
            }

            db.Collection("usernames").Document(username).GetSnapshotAsync()
                .ContinueWithOnMainThread(task =>
                {
                    callback(task.IsCompletedSuccessfully && !task.Result.Exists);
                });
        }

        /// <summary>
        /// Guarda en Firestone el nombre de usuario del jugador y lo reserva para evitar duplicados entre usuarios.
        /// </summary>
        /// <param name="username">Nombre a guardar.</param>
        public void SaveUsername(string username)
        {
            Debug.Log("[Firestore] Escritura iniciada: SaveUsername");

            var uid = AuthManager.Instance.GetUserId();
            var userDoc = GetUserDoc();
            if (userDoc == null) return;

            userDoc.SetAsync(
                new Dictionary<string, object> { { "username", username } },
                SetOptions.MergeAll
            );

            db.Collection("usernames").Document(username)
                .SetAsync(new Dictionary<string, object> { { "uid", uid } });
        }
        #endregion

        /// <summary>
        /// Guarda en Firestone que es Supporter.
        /// </summary>
        public void SaveIsSupporter()
        {
            var userDoc = GetUserDoc();
            if (userDoc == null) return;

            userDoc.SetAsync(
                new Dictionary<string, object>
                {
                    { "isSupporter", true }
                },
                SetOptions.MergeAll
            );
        }

        /// <summary>
        /// Guarda en Firestone que ha finalizado el juego.
        /// </summary>
        public void SaveGameEnded()
        {
            var userDoc = GetUserDoc();
            if (userDoc == null) return;

            userDoc.SetAsync(
                new Dictionary<string, object>
                {
                    { "gameEnded", true }
                },
                SetOptions.MergeAll
            );
        }

        /// <summary>
        /// Guarda en Firestone el tiempo total de juego
        /// </summary>
        public void SaveTotalPlayedTime(float newPlayedTime)
        {
            DocumentReference docRef = GetUserDoc();
            if (docRef == null) return;

            docRef.SetAsync(
                new Dictionary<string, object> { { "totalPlayedTime", newPlayedTime } },
                SetOptions.MergeAll
            );
        }


        /// <summary>
        /// Guarda en Firestone el número de monedas del jugador.
        /// </summary>
        public void SaveCoins(int newCoinAmount)
        {
            DocumentReference docRef = GetUserDoc();
            if (docRef == null) return;

            docRef.SetAsync(
                new Dictionary<string, object> { { "coins", newCoinAmount } },
                SetOptions.MergeAll
            );
        }

        #region Balls / Skins

        /// <summary>
        /// Guarda las monedas, skin seleccionada y skins desbloqueadas en Firestone.
        /// </summary>
        /// <param name="skinId">Id de la skin.</param>
        /// <param name="newCoinAmount">Número de monedas.</param>
        public void BuySkin(int skinId, int newCoinAmount)
        {
            DocumentReference docRef = GetUserDoc();
            if (docRef == null) return;

            docRef.SetAsync(new Dictionary<string, object>
            {
                { "coins", newCoinAmount },
                { "currentSkin", skinId },
                { "unlockedBalls", FieldValue.ArrayUnion(skinId) }
            }, SetOptions.MergeAll);
        }

        /// <summary>
        /// Guarda la skin seleccionada en Firestone.
        /// </summary>
        /// <param name="skinId">Id de la skin.</param>
        public void SelectSkin(int skinId)
        {
            DocumentReference docRef = GetUserDoc();
            if (docRef == null) return;

            docRef.SetAsync(
                new Dictionary<string, object> { { "currentSkin", skinId } },
                SetOptions.MergeAll
            );
        }
        #endregion

        #region Levels

        /// <summary>
        /// Guarda los datos completos de un nivel en Firestone.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel.</param>
        /// <param name="stars">Número de estrellas del nivel.</param>
        /// <param name="bestTime">Mejor tiempo.</param>
        /// <param name="tries">Intentos del nivel.</param>
        /// <param name="totalLevels">Número de niveles totales.</param>
        /// <param name="coins">Número de monedas conseguidas.</param>
        /// <param name="trophyCollected">Si se ha recogido el trofeo.</param>
        public void SaveLevelData(int levelIndex, int stars, float bestTime, int tries, int totalLevels, int coins, int totalCollectedCoins, bool trophyCollected)
        {
            DocumentReference docRef = GetUserDoc();
            if (docRef == null) return;

            var levelsData = new Dictionary<string, object>
            {
                {
                    levelIndex.ToString(),
                    new Dictionary<string, object>
                    {
                        { "stars", stars },
                        { "bestTime", bestTime },
                        { "tries", tries },
                        { "trophyCollected", trophyCollected }
                    }
                }
            };

            if (levelIndex + 1 < totalLevels)
            {
                levelsData[(levelIndex + 1).ToString()] =
                    new Dictionary<string, object> { { "unlocked", true } };
            }

            docRef.SetAsync(new Dictionary<string, object>
            {
                { "levels", levelsData },
                { "coins", coins },
                { "totalCollectedCoins", totalCollectedCoins }
            }, SetOptions.MergeAll);
        }

        /// <summary>
        /// Guarda el número de intentos de un nivel en Firestone.
        /// </summary>
        /// <param name="index">Indice del nivel.</param>
        /// <param name="tries">Intentos del nivel.</param>
        public void SaveLevelTries(int index, int tries)
        {
            DocumentReference docRef = GetUserDoc();
            if (docRef == null) return;

            docRef.SetAsync(new Dictionary<string, object>
            {
                {
                    "levels",
                    new Dictionary<string, object>
                    {
                        { index.ToString(), new Dictionary<string, object> { { "tries", tries } } }
                    }
                }
            }, SetOptions.MergeAll);
        }
        #endregion

        #region Rankings

        /// <summary>
        /// Guarda el mejor tiempo global de un nivel si es récord, utilizando una transacción para garantizar atomicidad.
        /// </summary>
        /// <param name="levelIndex">Indice del nivel</param>
        /// <param name="newTime">Mejor tiempo.</param>
        /// <param name="playerName">Nombre del jugador.</param>
        /// <param name="onComplete">Devuelve si se ha guardado con éxito.</param>
        public void SaveGlobalBestTime(int levelIndex, float newTime, string playerName, Action<bool> onComplete = null)
        {
            DocumentReference docRef = db.Collection("rankings").Document("global");

            db.RunTransactionAsync(transaction =>
            {
                return transaction.GetSnapshotAsync(docRef).ContinueWith(task =>
                {
                    var snapshot = task.Result;
                    var data = snapshot.Exists
                        ? snapshot.ToDictionary()
                        : new Dictionary<string, object>();

                    string key = levelIndex.ToString();
                    float currentBest = float.MaxValue;

                    if (data.ContainsKey(key) &&
                        data[key] is Dictionary<string, object> levelData &&
                        levelData.ContainsKey("bestTime"))
                    {
                        float.TryParse(levelData["bestTime"].ToString(), out currentBest);
                    }

                    if (newTime < currentBest)
                    {
                        transaction.Set(docRef, new Dictionary<string, object>
                        {
                            {
                                key, new Dictionary<string, object>
                                {
                                    { "playerName", playerName },
                                    { "bestTime", newTime.ToString("F2") }
                                }
                            }
                        }, SetOptions.MergeAll);

                        return true;
                    }

                    return false;
                });
            }).ContinueWithOnMainThread(task =>
            {
                onComplete?.Invoke(task.IsCompletedSuccessfully && task.Result);
            });
        }

        /// <summary>
        /// Carga los rankings globales de todos los niveles.
        /// </summary>
        /// <param name="callback">Devuelve los rankings globales.</param>
        public void LoadGlobalRankings(Action<Dictionary<int, (string playerName, float bestTime)>> callback)
        {
            DocumentReference docRef = db.Collection("rankings").Document("global");

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                var rankings = new Dictionary<int, (string, float)>();

                if (task.IsCompletedSuccessfully && task.Result.Exists)
                {
                    foreach (var kvp in task.Result.ToDictionary())
                    {
                        if (int.TryParse(kvp.Key, out int index) &&
                            kvp.Value is Dictionary<string, object> val)
                        {
                            rankings[index] = (
                                val["playerName"].ToString(),
                                float.Parse(val["bestTime"].ToString())
                            );
                        }
                    }
                }

                callback(rankings);
            });
        }
        #endregion
    }
}
