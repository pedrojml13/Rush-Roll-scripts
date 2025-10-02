using UnityEngine;

namespace PJML.RushAndRoll
{
    public class Trunk : MonoBehaviour
    {
        private Vector3 initialPos;

        void Start()
        {
            // Guarda la posición inicial del tronco al comenzar
            initialPos = transform.position;
        }

        void Update()
        {
            // Si el tronco cae por debajo del umbral, lo reinicia a su posición original
            if (transform.position.y < -10f)
            {
                transform.position = initialPos;
            }
        }
    }
}