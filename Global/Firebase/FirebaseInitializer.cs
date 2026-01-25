using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.AppCheck;

namespace PJML.RushAndRoll
{
    public class FirebaseInitializer : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del SessionManager.
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
        /// Inicializa Firebase y App Check.
        /// </summary>
        private void Start()
        {
            InitializeFirebaseAndAppCheck();
        }

        /// <summary>
        /// Inicializa Firebase y App Check.
        /// </summary>
        void InitializeFirebaseAndAppCheck()
        {

            FirebaseAppCheck.SetAppCheckProviderFactory(PlayIntegrityProviderFactory.Instance);             

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result != DependencyStatus.Available)
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
                    return;
                }     
                
                IsFirebaseReady = true;
            });
        }

    }
}
