using UnityEngine;
using Firebase.Auth;
using System;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
//using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using System.Collections;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la autenticación del usuario mediante Firebase.
    /// Soporta login por email/contraseña, Google y Google Play Games Services.
    /// Implementa el patrón Singleton y persiste entre escenas.
    /// </summary>
    public class AuthManager : MonoBehaviour
    {
        /// <summary>
        /// Instancia única del AuthManager.
        /// </summary>
        public static AuthManager Instance { get; private set; }

        private FirebaseAuth auth;

        /// <summary>
        /// Acceso estático al usuario actual autenticado en Firebase.
        /// Devuelve null si no hay sesión activa.
        /// </summary>
        public static FirebaseUser currentUser;
            

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
        /// Si tiene internet, se autentica.
        /// </summary>
        private IEnumerator Start()
        {
            while (!FirebaseInitializer.Instance.IsFirebaseReady)
                    yield return null;
            auth = FirebaseAuth.DefaultInstance;

            currentUser = auth.CurrentUser;
        }

        /// <summary>
        /// Devuelve el usuario autenticado actualmente en Firebase.
        /// </summary>
        /// <returns>Usuario actual.</returns>
        public FirebaseUser GetCurrentUser()
        {
            return auth.CurrentUser;
        }

        /// <summary>
        /// Devuelve el ID único del usuario autenticado.
        /// </summary>
        /// <returns>Id del usuario.</returns>
        public string GetUserId()
        {
            return auth.CurrentUser?.UserId;
        }

        /// <summary>
        /// Comprueba si existe una sesión activa en Firebase.
        /// </summary>
        /// <param name="callback">Devuelve si hay sesión activa.</param>
        public void CheckSession(Action<bool, string> callback)
        {
            FirebaseUser user = auth.CurrentUser;

            if (user != null)
            {
                callback(true, "");
            }
            else
            {
                callback(false, null);
            }
        }

        /// <summary>
        /// Inicia sesión con email y contraseña mediante Firebase.
        /// </summary>
        /// <param name="email">Email del usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        /// <param name="callback">Devuelve si se ha logeado con éxito.</param>
        public void Login(string email, string password, Action<bool, string> callback)
        {
            auth.SignInWithEmailAndPasswordAsync(email, password)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        callback(false, task.Exception?.Message);
                    }
                    else
                    {
                        FirebaseUser user = task.Result.User;
                        user.ReloadAsync().ContinueWithOnMainThread(reloadTask =>
                        {
                            if (reloadTask.IsCompleted && !reloadTask.IsFaulted)
                            {
                                callback(true, null);
                            }
                            else
                            {
                                callback(false, "Error al recargar sesión");
                            }
                        });
                    }
                });
        }

        /// <summary>
        /// Registra un nuevo usuario con email y contraseña en Firebase.
        /// </summary>
        /// <param name="email">Email a registrar.</param>
        /// <param name="password">Contraseña a registrar.</param>
        /// <param name="callback">Devuelve si se ha registrado con éxito.</param>
        public void Register(string email, string password, Action<bool, string> callback)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        callback(false, task.Exception?.Message);
                    }
                    else
                    {
                        callback(true, null);
                    }
                });
        }

        /// <summary>
        /// Cierra la sesión actual del usuario en Firebase.
        /// </summary>
        public void LogOut()
        {
            if (auth.CurrentUser != null)
            {
                auth.SignOut();
                Debug.Log("Sesión cerrada correctamente.");
            }
            else
            {
                Debug.LogWarning("No hay usuario autenticado para cerrar sesión.");
            }
        }

        /// <summary>
        /// Inicia sesión en Firebase utilizando un ID Token de Google.
        /// </summary>
        /// <param name="idToken">Id del token de Google.</param>
        /// <param name="callback">Devuelve si se ha logeado con éxito.</param>
        public void LoginWithGoogle(string idToken, Action<bool, string> callback)
        {
            Credential credential =
                GoogleAuthProvider.GetCredential(idToken, null);

            auth.SignInWithCredentialAsync(credential)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        callback(false, task.Exception?.Message);
                    }
                    else
                    {
                        callback(true, null);
                    }
                });
        }

        /// <summary>
        /// Inicia sesión mediante Google Play Games Services
        /// y enlaza la cuenta con Firebase.
        /// </summary>
        /// <param name="callback">Devuelve si se ha logeado con éxito.</param>
        public void LoginWithGPGS(Action<bool, string> callback)
        {
            GPGSManager.Instance.SignIn((success, error) =>
            {
                if (!success)
                {
                    callback(false, error);
                    return;
                }

                GPGSManager.Instance.GetServerAuthCode(authCode =>
                {
                    Credential credential =
                        PlayGamesAuthProvider.GetCredential(authCode);

                    auth.SignInWithCredentialAsync(credential)
                        .ContinueWithOnMainThread(task =>
                        {
                            if (task.IsFaulted || task.IsCanceled)
                            {
                                callback(false, task.Exception?.Message);
                            }
                            else
                            {
                                callback(true, null);
                            }
                        });
                });
            });
        }
    }
}
