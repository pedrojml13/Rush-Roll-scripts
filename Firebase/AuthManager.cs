using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    public static FirebaseUser currentUser;

    void Start()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            currentUser = auth.CurrentUser;
            Debug.Log("Usuario ya logueado: " + currentUser.UserId);
            return;
        }

        // Si no hay usuario, iniciar sesión anónima
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al iniciar sesión anónima: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                currentUser = task.Result.User;
                Debug.Log("Usuario anónimo creado: " + currentUser.UserId);
            }
        });
    }
}
