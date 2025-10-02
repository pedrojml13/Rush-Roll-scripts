using UnityEngine;
using Firebase.Auth;
using System;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace PJML.RushAndRoll
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }
        private FirebaseAuth auth;

        // Acceso estático al usuario actual
        public static FirebaseUser currentUser => FirebaseAuth.DefaultInstance.CurrentUser;

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
            auth = FirebaseAuth.DefaultInstance;
        }

        // Login con email y contraseña
        public void Login(string email, string password, Action<bool, string> callback)
        {

            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    callback(false, task.Exception?.Message);
                }
                else
                {
                    FirebaseUser user = task.Result.User;
                    callback(true, null);
                }
            });
        }

        //Registro con email y contraseña
        public void Register(string email, string password, Action<bool, string, string> callback)
        {
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    callback(false, null, task.Exception?.Message);
                }
                else
                {
                    FirebaseUser user = task.Result.User;
                    string name = user.DisplayName ?? user.Email;
                    callback(true, name, null);
                }
            });
        }

        // Login con Google Play Juegos
        /*public void LoginWithPlayGames(Action<bool, string> callback)
        {
            PlayGamesPlatform.Activate();

            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    PlayGamesPlatform.Instance.GetServerAuthCode(authCode =>
                    {
                        if (!string.IsNullOrEmpty(authCode))
                        {
                            Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
                            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCanceled || task.IsFaulted)
                                {
                                    callback(false, task.Exception?.Message);
                                }
                                else
                                {
                                    FirebaseUser user = task.Result;
                                    callback(true, null);
                                }
                            });
                        }
                        else
                        {
                            callback(false, "No se pudo obtener el AuthCode");
                        }
                    });
                }
                else
                {
                    callback(false, "Login con Play Juegos fallido");
                }
            });
        }

        // Cerrar sesión
        public void Logout()
        {
            auth.SignOut();
            PlayGamesPlatform.Instance.SignOut();
        }*/


    }
}