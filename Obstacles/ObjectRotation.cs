using UnityEngine;

namespace PJML.RushAndRoll
{
    public class ObjectRotation : MonoBehaviour
    {
        public Vector3 rotationSpeed = new Vector3(0, 100, 0); // grados por segundo

        void Update()
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }
}