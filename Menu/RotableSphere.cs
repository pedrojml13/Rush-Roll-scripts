using UnityEngine;
using UnityEngine.InputSystem;

namespace PJML.RushAndRoll
{
    public class RotatableSphereInputSystem : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 0.2f;
        [SerializeField] private float inertiaDamping = 5f;
        [SerializeField] private float autoRotationSpeed = 10f; // rotación lenta cuando está parado

        private bool dragging = false;
        private Vector2 previousTouchPos;
        private Vector2 currentVelocity;

        void Update()
        {
            if (Pointer.current != null)
            {
                if (Pointer.current.press.isPressed)
                {
                    Vector2 currentPos = Pointer.current.position.ReadValue();

                    if (!dragging)
                    {
                        dragging = true;
                        previousTouchPos = currentPos;
                        currentVelocity = Vector2.zero;
                    }
                    else
                    {
                        Vector2 delta = currentPos - previousTouchPos;

                        // rotación
                        transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);
                        transform.Rotate(Vector3.right, delta.y * rotationSpeed, Space.World);

                        // guardar la velocidad
                        currentVelocity = delta / Time.deltaTime;

                        previousTouchPos = currentPos;
                    }
                }
                else
                {
                    dragging = false;
                }
            }

            if (!dragging)
            {
                if (currentVelocity.sqrMagnitude > 0.01f)
                {
                    // aplicar inercia
                    Vector2 frameDelta = currentVelocity * Time.deltaTime;

                    transform.Rotate(Vector3.up, -frameDelta.x * rotationSpeed, Space.World);
                    transform.Rotate(Vector3.right, frameDelta.y * rotationSpeed, Space.World);

                    // frenado progresivo
                    currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, inertiaDamping * Time.deltaTime);
                }
                else
                {
                    // rotación lenta automática
                    transform.Rotate(Vector3.up, autoRotationSpeed * Time.deltaTime, Space.World);
                }
            }
        }
    }
}