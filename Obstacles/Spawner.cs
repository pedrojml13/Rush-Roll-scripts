using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la generación automática de objetos en una posición específica.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        [Header("Configuración del Prefab")]
        [Tooltip("El objeto que se va a clonar.")]
        [SerializeField] private GameObject objectToSpawn;

        [Header("Tiempos")]
        [Tooltip("Segundos entre cada generación de objeto.")]
        [SerializeField] private float spawnInterval = 3f;

        [Tooltip("Tiempo de vida del objeto antes de ser destruido.")]
        [SerializeField] private float objectLifetime = 5f;

        /// <summary>
        /// Inicia un ciclo repetitivo que llama a SpawnObject
        /// </summary>
        private void Start()
        {
            InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
        }

        /// <summary>
        /// Crea una instancia del objeto y programa su destrucción futura.
        /// </summary>
        private void SpawnObject()
        {
            if (objectToSpawn == null) return;

            GameObject instance = Instantiate(objectToSpawn, transform.position, transform.rotation);
            Destroy(instance, objectLifetime);
        }
    }
}