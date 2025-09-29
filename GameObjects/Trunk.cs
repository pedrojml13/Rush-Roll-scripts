using UnityEngine;

public class Trunk : MonoBehaviour
{
    private Vector3 initialPos;

    void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        if(transform.position.y < -10f)
        {
            transform.position = initialPos;
        }
    }
}
