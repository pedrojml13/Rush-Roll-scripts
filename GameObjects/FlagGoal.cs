using System;
using UnityEngine;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Representa la meta del nivel.
    /// Al entrar en contacto con el jugador, activa la victoria.
    /// Puede romperse visualmente para indicar que ya fue usada.
    /// </summary>
    public class FlagGoal : MonoBehaviour
    {
        [Tooltip("Sonido que se reproduce al alcanzar la meta")]
        [SerializeField] private AudioClip flagGoalClip;

        private MeshFilter meshFilter;

        /// <summary>
        /// Indica si la bandera ya ha sido activada o rota.
        /// Evita que la victoria se dispare más de una vez.
        /// </summary>
        public bool isBroken = false;

        private void Start()
        {
            // Guarda referencia al MeshFilter del objeto
            meshFilter = GetComponent<MeshFilter>();
        }

        /// <summary>
        /// Rompe visualmente la bandera eliminando submeshes,
        /// sin modificar el asset original.
        /// </summary>
        public void Break()
        {
            if (isBroken) return;
            isBroken = true;

            // Clona la malla para no afectar al asset compartido
            Mesh mesh = Instantiate(meshFilter.mesh);
            meshFilter.mesh = mesh;

            // Elimina submeshes específicos para simular la rotura
            int[] empty = Array.Empty<int>();
            mesh.SetTriangles(empty, 1);
            mesh.SetTriangles(empty, 2);
        }

        /// <summary>
        /// Detecta cuando el jugador alcanza la meta del nivel.
        /// </summary>
        /// <param name="other">Objeto que colisiona.</param>
        private void OnTriggerEnter(Collider other)
        {
            if (isBroken || !other.CompareTag("Player")) return;

            VibrationManager.Instance.Vibrate();

            AudioManager.Instance.PlaySFX(flagGoalClip);
            AudioManager.Instance.StopLoop("Rolling");
            LevelManager.Instance.WinLevel();
        }
    }
}
