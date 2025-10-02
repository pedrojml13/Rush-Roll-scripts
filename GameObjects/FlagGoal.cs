using System;
using UnityEngine;

namespace PJML.RushAndRoll
{
    public class FlagGoal : MonoBehaviour
    {
        [SerializeField] private AudioClip flagGoalClip;
        private MeshFilter meshFilter;
        public bool isBroken = false;

        void Start()
        {
            // Guarda referencia al MeshFilter del objeto
            meshFilter = GetComponent<MeshFilter>();
        }

        public void Break()
        {
            if (isBroken) return;
            isBroken = true;

            // Clona la malla actual para evitar modificar el asset original
            Mesh mesh = Instantiate(meshFilter.mesh);
            meshFilter.mesh = mesh;

            // Elimina los submeshes 1 y 2 (si existen) para simular rotura visual
            int[] empty = Array.Empty<int>();
            mesh.SetTriangles(empty, 1);
            mesh.SetTriangles(empty, 2);
        }

        void OnTriggerEnter(Collider other)
        {
            // Si el jugador toca la bandera y no está rota, se activa la victoria
            if (!isBroken && other.CompareTag("Player"))
            {
                AudioManager.Instance.PlaySFX(flagGoalClip);         // Sonido de meta
                AudioManager.Instance.StopLoop("Rolling");           // Detiene sonido de rodamiento
                LevelManager.Instance.WinLevel();                    // Lanza evento de victoria
            }
        }
    }
}