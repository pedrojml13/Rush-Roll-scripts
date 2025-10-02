using UnityEngine;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;

namespace PJML.RushAndRoll
{
    public class PlayerDataManager : MonoBehaviour
    {
        public static PlayerDataManager Instance;

        FirebaseFirestore db;

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

        void Start()
        {
            db = FirebaseFirestore.DefaultInstance;
        }

        #region Language
        public void saveLanguage(int lang)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "language", lang }
        };

            docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                    Debug.Log("Lenguaje guardado: " + lang);
                else
                    Debug.LogError("Error guardando lenguaje: " + task.Exception);
            });
        }

        public void LoadLanguage(System.Action<int> callback)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    if (task.Result.ContainsField("language"))
                    {
                        int lang = task.Result.GetValue<int>("language");
                        callback.Invoke(lang);
                    }
                    else
                    {
                        callback.Invoke(0); // por defecto inglés
                    }
                }
                else
                {
                    callback.Invoke(0); // fallback
                }
            });
        }
        #endregion

        #region Username
        public void SaveUsername(string username)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "username", username }
        };

            docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                    Debug.Log("Nombre de usuario guardado" + username);
                else
                    Debug.LogError("Error guardando el nombre de usuario: " + task.Exception);
            });
        }

        public void LoadUsername(System.Action<string> callback)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    string username = task.Result.GetValue<string>("username");
                    callback.Invoke(username);
                }
                else
                {
                    callback.Invoke("");
                }
            });
        }
        #endregion

        #region Coins
        public void SaveCoins(int coins)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "coins", coins }
        };

            docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                    Debug.Log("Monedas guardadas: " + coins);
                else
                    Debug.LogError("Error guardando monedas: " + task.Exception);
            });
        }

        public void LoadCoins(System.Action<int> callback)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    int coins = task.Result.GetValue<int>("coins");
                    callback.Invoke(coins);
                }
                else
                {
                    callback.Invoke(0);
                }
            });
        }
        #endregion

        #region Skins
        public void SaveSkin(int skinId)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "currentSkin", skinId }
        };

            docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                    Debug.Log("Skin actual guardada: " + skinId);
                else
                    Debug.LogError("Error guardando currentSkin: " + task.Exception);
            });
        }

        public void LoadCurrentSkin(System.Action<int> callback)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    if (task.Result.ContainsField("currentSkin"))
                    {
                        int skinId = task.Result.GetValue<int>("currentSkin");
                        callback.Invoke(skinId);
                    }
                    else
                    {
                        callback.Invoke(0); // por defecto la skin 0
                    }
                }
                else
                {
                    callback.Invoke(0); // fallback
                }
            });
        }

        #endregion

        #region Skins
        public void SaveUnlockedBall(int ballId)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            // Intenta actualizar con ArrayUnion (creará el array si no existe)
            docRef.UpdateAsync("unlockedBalls", FieldValue.ArrayUnion(ballId))
                .ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log($"Bola desbloqueada guardada: {ballId}");
                }
                else
                {
                    // Si falla (por ejemplo, documento no existe), crea el documento/field con Set (merge)
                    var data = new Dictionary<string, object>
                    {
                    { "unlockedBalls", new List<int> { ballId } }
                    };
                    docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(t2 =>
                    {
                        if (!t2.IsFaulted && !t2.IsCanceled)
                            Debug.Log($"Bola desbloqueada guardada (creada): {ballId}");
                        else
                            Debug.LogError($"Error guardando bola: {t2.Exception}");
                    });
                }
            });
        }
        public void LoadUnlockedBalls(System.Action<List<int>> callback)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result.Exists)
                {
                    if (task.Result.ContainsField("unlockedBalls"))
                    {
                        List<int> unlockedBalls = task.Result.GetValue<List<int>>("unlockedBalls");
                        callback.Invoke(unlockedBalls);
                    }
                    else
                    {
                        callback.Invoke(new List<int>()); // lista vacía si aún no existe
                    }
                }
                else
                {
                    callback.Invoke(new List<int>());
                }
            });
        }
        #endregion

        #region UnlockedLevels

        public void SaveLevelUnlockedOnly(int levelIndex)
        {
            if (AuthManager.currentUser == null)
            {
                Debug.LogError("No hay usuario autenticado.");
                return;
            }

            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            var levelData = new Dictionary<string, object>
        {
            { levelIndex.ToString(), new Dictionary<string, object> { { "unlocked", true } } }
        };

            var data = new Dictionary<string, object>
        {
            { "levels", levelData }
        };

            docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                    Debug.Log($"Nivel {levelIndex} desbloqueado correctamente.");
                else
                    Debug.LogError($"Error desbloqueando nivel {levelIndex}: {task.Exception?.Message}");
            });
        }
        public void SaveLevel(LevelData level)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            // Creamos un diccionario simple para el nivel
            var levelData = new Dictionary<string, object>
        {
            { "unlocked", level.unlocked },
            { "stars", level.stars },
            { "bestTime", level.bestTime }
        };


            var levelsData = new Dictionary<string, object>
        {
            { level.levelIndex.ToString(), levelData }
        };

            var data = new Dictionary<string, object>
        {
            { "levels", levelsData }
        };

            docRef.SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                    Debug.Log($"Nivel {level.levelIndex} guardado correctamente.");
                else
                    Debug.LogError($"Error guardando nivel {level.levelIndex}: {task.Exception}");
            });
        }

        public void LoadLevels(System.Action<List<LevelData>> callback, int totalLevels = 27)
        {
            string uid = AuthManager.currentUser.UserId;
            DocumentReference docRef = db.Collection("users").Document(uid);

            docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                List<LevelData> levels = new List<LevelData>();

                // Inicializa todos los niveles localmente
                for (int i = 0; i < totalLevels; i++)
                {
                    LevelData level = new LevelData(i);
                    level.unlocked = (i == 0); // primer nivel siempre desbloqueado
                    level.stars = 0;
                    level.bestTime = 0f;
                    levels.Add(level);
                }

                if (task.IsCompleted && task.Result.Exists && task.Result.ContainsField("levels"))
                {
                    var rawLevels = task.Result.GetValue<Dictionary<string, object>>("levels");
                    foreach (var kvp in rawLevels)
                    {
                        int index = int.Parse(kvp.Key);
                        if (kvp.Value is Dictionary<string, object> val)
                        {
                            levels[index].unlocked = val.ContainsKey("unlocked") && (bool)val["unlocked"];
                            levels[index].stars = val.ContainsKey("stars") ? System.Convert.ToInt32(val["stars"]) : 0;
                            levels[index].bestTime = val.ContainsKey("bestTime") ? System.Convert.ToSingle(val["bestTime"]) : 0f;
                        }
                    }
                }

                callback?.Invoke(levels);
            });
        }
        #endregion
    }
}