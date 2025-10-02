using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

namespace PJML.RushAndRoll
{
    public class FirebaseInitializer : MonoBehaviour
    {
        public static FirebaseInitializer Instance { get; private set; }
        public bool IsFirebaseReady { get; private set; } = false;

        void Awake()
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
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    Debug.Log("Firebase está listo.");
                    IsFirebaseReady = true;
                }
                else
                {
                    Debug.LogError(" Firebase no está disponible: " + dependencyStatus);
                }
            });
        }
    }
}