using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestor de autenticación y datos de Google Play Games Services (GPGS).
    /// Singleton accesible desde cualquier parte del proyecto.
    /// </summary>
    public class GPGSManager : MonoBehaviour
    {

        /// <summary>
        /// Instancia única del AudioManager.
        /// </summary>
        public static GPGSManager Instance { get; private set; }

        /// <summary>
        /// Indica si el jugador está autenticado en GPGS.
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        
        /// <summary>
        /// Singleton, asegura que solo exista una instancia
        /// </summary>
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

        /// <summary>
        /// Activa la plataforma de Google Play Games
        /// </summary>
        void Start()
        {
            PlayGamesPlatform.Activate();
        }

        /// <summary>
        /// Obtiene el nombre de usuario de GPGS si está autenticado.
        /// </summary>
        /// <returns>Nombre de usuario o null si no está autenticado.</returns>
        public string GetUsername()
        {
            if (IsAuthenticated)
                return PlayGamesPlatform.Instance.localUser.userName;

            return null;
        }

        /// <summary>
        /// Obtiene la imagen del usuario de GPGS si está autenticado.
        /// </summary>
        /// <returns>Imagen del usuario o null si no está autenticado.</returns>
        public Texture2D GetUserImage()
        {
            if (IsAuthenticated)
                return PlayGamesPlatform.Instance.localUser.image;

            return null;
        }

        /// <summary>
        /// Inicia sesión en Google Play Games Services.
        /// </summary>
        /// <param name="callback">Devuelve si se ha autenticado con éxito.</param>
        public void SignIn(Action<bool, string> callback)
        {
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                if (status == SignInStatus.Success)
                {
                    IsAuthenticated = true;
                    callback(true, null);
                }
                else
                {
                    callback(false, status.ToString());
                }
            });
        }

        /// <summary>
        /// Solicita un código de autenticación del servidor para iniciar sesión en Firebase.
        /// </summary>
        /// <param name="callback">Devuelve el código de autenticación.</param>
        public void GetServerAuthCode(Action<string> callback)
        {
            PlayGamesPlatform.Instance.RequestServerSideAccess(true, authCode =>
            {
                callback(authCode);
            });
        }
    }
}
