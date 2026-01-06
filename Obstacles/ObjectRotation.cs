using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Proporciona una rotación continua a un objeto y aleatoriza su orientación inicial.
    /// </summary>
    public class ObjectRotation : MonoBehaviour
    {
        [Header("Configuración de Movimiento")]
        [Tooltip("Velocidad de rotación en grados por segundo para cada eje (X, Y, Z).")]
        public Vector3 rotationSpeed = new Vector3(0, 100, 0);

        /// <summary>
        /// Aplica una rotación aleatoria inicial para evitar que 
        /// múltiples objetos iguales se vean perfectamente sincronizados.
        /// </summary>
        void Start()
        {
            Vector3 randomRotation = Vector3.zero;

            if (rotationSpeed.y == 0)
            {
                randomRotation = new Vector3(0, 0, Random.Range(0f, 360f));
            }
            else if (rotationSpeed.z == 0)
            {
                randomRotation = new Vector3(0, Random.Range(0f, 360f), 0);
            }

            transform.Rotate(randomRotation);
        }

        /// <summary>
        /// Aplica la rotación frame a frame de forma independiente a la tasa de refresco.
        /// </summary>
        void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}