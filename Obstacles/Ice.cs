using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona el comportamiento de las plataformas de hielo.
    /// </summary>
    public class Ice : MonoBehaviour
    {
        [Header("Efectos de Sonido")]
        [SerializeField] private AudioClip iceCrackSound;
        [SerializeField] private AudioClip iceBreakSound;

        [Header("Efectos Visuales")]
        [SerializeField] private ParticleSystem breakIceParticles;
        [SerializeField] private Material breakIceMaterial;

        [Header("Ajustes de Mecánica")]
        [Tooltip("Tiempo máximo en segundos que el jugador puede estar sobre el hielo antes de que se rompa.")]
        [SerializeField] private float maxTimeOnIce = 10f;

        private float timeOnIce = 0f;
        private bool isBreak = false;


        
        /// <summary>
        /// Al entrar en contacto, elimina el rozamiento del jugador para simular una superficie resbaladiza.
        /// </summary>
        /// <param name="other">Objeto que colisiona.</param>   
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Rigidbody playerRigidbody = other.gameObject.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    Vector3 slideDirection = playerRigidbody.linearVelocity.normalized;
                    float slideForceMagnitude = 1f;
                    Vector3 slideForce = slideDirection * slideForceMagnitude;

                    playerRigidbody.AddForce(slideForce, ForceMode.VelocityChange);

                    playerRigidbody.linearDamping = 0f;
                    playerRigidbody.angularDamping = 0f;
                }
            }
        }

        /// <summary>
        /// Mientras el jugador permanezca sobre el hielo, se acumula tiempo para la destrucción.
        /// </summary>
        /// <param name="other">Objeto que permanece en el área.</param> 
        public void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                timeOnIce += Time.deltaTime;

                if (timeOnIce > maxTimeOnIce / 2 && !isBreak)
                {
                    ChangeToBrokenState();
                }
                else if (timeOnIce > maxTimeOnIce)
                {
                    BreakCompletely();
                }
            }
        }

        /// <summary>
        /// Cambia el aspecto visual y reproduce sonidos/partículas de advertencia.
        /// </summary>
        private void ChangeToBrokenState()
        {
            GetComponent<Renderer>().material = breakIceMaterial;
            if (breakIceParticles != null) breakIceParticles.Play();
            
            AudioManager.Instance.PlaySFX(iceCrackSound);
            isBreak = true;
        }

        /// <summary>
        /// Destruye la plataforma de hielo.
        /// </summary>
        private void BreakCompletely()
        {
            AudioManager.Instance.PlaySFX(iceBreakSound);
            Destroy(gameObject);
        }

        /// <summary>
        /// Al salir del hielo, restaura los valores de fricción originales del jugador.
        /// </summary>
        /// <param name="player">Objeto que sale del área.</param> 
        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Rigidbody playerRigidbody = other.gameObject.GetComponent<Rigidbody>();
                if (playerRigidbody != null)
                {
                    playerRigidbody.linearDamping = 0.2f;
                    playerRigidbody.angularDamping = 0.2f;
                }
            }
        }
    }
}