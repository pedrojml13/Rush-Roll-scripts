using UnityEngine;
using System.Collections;
public class ObjectButton : MonoBehaviour
{

    public bool pressed = false;
    [SerializeField] float moveY;

    [SerializeField] AudioClip buttonPressed;
    [SerializeField] AudioClip buttonReleased;


    private void OnTriggerEnter(Collider other)
    {
        pressed = true;
        
        AudioManager.Instance.PlaySFX(buttonPressed);

        Vector3 newPos = transform.localPosition;
        newPos.y -= moveY;
        transform.localPosition = newPos;

    }

    private void OnTriggerExit(Collider other)
    {
        pressed = false;

        AudioManager.Instance.PlaySFX(buttonReleased);

        Vector3 newPos = transform.localPosition;
        newPos.y += moveY;
        transform.localPosition = newPos;

    }



}
