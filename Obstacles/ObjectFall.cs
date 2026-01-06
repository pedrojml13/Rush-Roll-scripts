using UnityEngine;
using System.Collections;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona plataformas que tiemblan y caen al vacío tras ser pisadas por el jugador.
    /// </summary>
    public class ObjectFall : MonoBehaviour
    {
        [Header("Tiempos")]
        [Tooltip("Cuánto tiempo vibra la plataforma antes de caer.")]
        public float tiempoTemblor = 0.5f;
        
        [Tooltip("Tiempo antes de que el objeto sea eliminado de la escena tras caer.")]
        public float tiempoDestruccion = 5f;

        [Header("Ajustes de Vibración")]
        [Tooltip("Magnitud del movimiento aleatorio durante el temblor.")]
        public float intensidadTemblor = 0.05f;

        private bool activada = false;

        /// <summary>
        /// Detecta si el jugador ha aterrizado sobre la plataforma.
        /// </summary>
        /// <param name="collision">Objeto que colisiona.</param>
        void OnCollisionEnter(Collision collision)
        {
            if (!activada && collision.gameObject.CompareTag("Player"))
            {
                activada = true;
                StartCoroutine(TemblarYCaer());
            }
        }

        /// <summary>
        /// Vibración visual seguida de activación de gravedad.
        /// </summary>
        IEnumerator TemblarYCaer()
        {
            Vector3 posicionOriginal = transform.position;

            float tiempo = 0f;
            while (tiempo < tiempoTemblor)
            {
                float offsetX = Random.Range(-intensidadTemblor, intensidadTemblor);
                float offsetZ = Random.Range(-intensidadTemblor, intensidadTemblor);
                
                transform.position = posicionOriginal + new Vector3(offsetX, 0, offsetZ);

                tiempo += Time.deltaTime;
                yield return null;
            }

            transform.position = posicionOriginal;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            Destroy(gameObject, tiempoDestruccion);
        }
    }
}