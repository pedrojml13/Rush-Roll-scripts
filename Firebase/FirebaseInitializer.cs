using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class FirebaseInitializer : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase está listo.");
                // Ahora puedes usar FirebaseAuth y Firestore
                FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

                // Aquí puedes iniciar tu AuthManager, PlayerDataManager, etc.
            }
            else
            {
                Debug.LogError("Firebase no está disponible: " + dependencyStatus);
            }
        });
    }
}