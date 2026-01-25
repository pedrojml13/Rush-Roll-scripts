using Google.Play.Review;
using System.Collections;
using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el flujo de reseñas nativas de Google Play (In-App Review).
    /// Permite que los usuarios califiquen el juego sin salir de la aplicación.
    /// </summary>
    public class InAppReviewManager : MonoBehaviour
    {
         public static InAppReviewManager Instance { get; private set; }

        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;

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
        /// Pre-carga el objeto de reseña al iniciar para que esté listo cuando se gane el nivel
        /// </summary>
        private void Start()
        {
            _reviewManager = new ReviewManager();
            StartCoroutine(RequestReviewObject());
        }

        /// <summary>
        /// Solicita el objeto ReviewInfo a los servidores de Google Play.
        /// </summary>
        private IEnumerator RequestReviewObject()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;

            if (requestFlowOperation.Error == ReviewErrorCode.NoError)
            {
                _playReviewInfo = requestFlowOperation.GetResult();
            }
            else
            {
                Debug.LogWarning("InAppReview: No se pudo precargar el flujo de reseña.");
            }
        }

        /// <summary>
        /// Método público para activar la reseña. 
        /// </summary>
        public void LaunchReview()
        {
            if (_playReviewInfo != null)
            {
                StartCoroutine(ShowReview());
            }
            else
            {
                StartCoroutine(RequestAndShow());
            }
        }

        /// <summary>
        /// Muestra la ventana de reseña sobre la pantalla de victoria.
        /// </summary>
        private IEnumerator ShowReview()
        {    
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            
            _playReviewInfo = null; 
        }

        /// <summary>
        /// Intenta obtener el objeto de reseña y mostrarlo si no se precargó correctamente al inicio.
        /// </summary>
        private IEnumerator RequestAndShow()
        {
            yield return StartCoroutine(RequestReviewObject());
            if (_playReviewInfo != null)
            {
                yield return StartCoroutine(ShowReview());
            }
        }
    }
}