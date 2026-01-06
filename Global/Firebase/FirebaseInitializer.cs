using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Inicializa Firebase al arrancar el juego y verifica que
    /// todas las dependencias estén disponibles antes de su uso.
    /// Implementa el patrón Singleton y persiste entre escenas.
    /// </summary>
    public class FirebaseInitializer : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del FirebaseInitializer.
        /// </summary>
        public static FirebaseInitializer Instance { get; private set; }
        public bool IsFirebaseReady { get; private set; } = false;

        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// y evita su destrucción al cambiar de escena.
        /// </summary>
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

        /// <summary>
        /// Comprueba y corrige automáticamente las dependencias de Firebase.
        /// Marca true cuando todas están disponibles.
        /// </summary>
        private void Start()
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
                FirebaseApp.CheckAndFixDependenciesAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        var dependencyStatus = task.Result;

                        if (dependencyStatus == DependencyStatus.Available)
                        {
                            IsFirebaseReady = true;
                        }
                        else
                        {
                            Debug.LogError(
                                "Firebase no está disponible: " + dependencyStatus
                            );
                        }
                    });
        }
    }
}
