using UnityEngine;

namespace PJML.RushAndRoll
{
    public class Wind : MonoBehaviour
    {
        [SerializeField] private AudioClip windSound;
        [SerializeField] Vector3 windDirection = new Vector3(1f, 0f, 0f);
        [SerializeField] float windStrength = 5f;


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                AudioManager.Instance.PlayLoop("wind", windSound);
            }
        }
        private void OnTriggerStay(Collider other)
        {

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(windDirection.normalized * windStrength, ForceMode.Force);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                AudioManager.Instance.StopLoop("wind");
            }
        }
    }
}