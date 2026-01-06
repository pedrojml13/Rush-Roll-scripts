using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Representa un tronco que puede caer en el nivel. 
    /// Si cae por debajo de un umbral, se reinicia a su posición inicial.
    /// </summary>
    public class Trunk : MonoBehaviour
    {
        private Vector3 initialPos;

        /// <summary>
        /// Guarda la posición inicial del tronco al iniciar la escena.
        /// </summary>
        void Start()
        {
            initialPos = transform.position;
        }

        /// <summary>
        /// Reinicia el tronco a su posición inicial si cae por debajo del umbral de Y.
        /// </summary>
        void Update()
        {
            if (transform.position.y < -10f)
            {
                transform.position = initialPos;
            }
        }
    }
}
