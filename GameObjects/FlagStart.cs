using System;
using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Bandera de inicio del nivel.
    /// Se activa cuando el jugador la toca por primera vez,
    /// marcando el comienzo oficial del gameplay.
    /// </summary>
    public class FlagStart : MonoBehaviour
    {
        [Tooltip("Sonido que se reproduce al iniciar el nivel")]
        [SerializeField] private AudioClip flagStartClip;

        private MeshFilter mf;

        /// <summary>
        /// Indica si la bandera ya fue activada.
        /// Evita que el inicio del nivel se dispare más de una vez.
        /// </summary>
        public bool isBroken = false;

        private void Start()
        {
            // Guarda la referencia al MeshFilter del objeto
            mf = GetComponent<MeshFilter>();
        }

        /// <summary>
        /// Rompe visualmente la bandera de inicio eliminando submeshes,
        /// sin modificar el asset original.
        /// </summary>
        public void Break()
        {
            if (isBroken) return;
            isBroken = true;

            // Clona la malla para no afectar al asset compartido
            Mesh mesh = Instantiate(mf.mesh);
            mf.mesh = mesh;

            int[] empty = Array.Empty<int>();

            // Elimina submeshes específicos para simular la rotura visual
            mesh.SetTriangles(empty, 0);
            mesh.SetTriangles(empty, 2);
        }

        /// <summary>
        /// Detecta cuando el jugador activa la bandera de inicio del nivel.
        /// </summary>
        /// <param name="other">Objeto que colisiona.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || isBroken) return;

            AudioManager.Instance.PlaySFX(flagStartClip);
            Break();
            LevelManager.Instance.StartFlag();
        }
    }
}
