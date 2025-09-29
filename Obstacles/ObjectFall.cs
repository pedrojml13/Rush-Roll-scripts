using UnityEngine;

public class ObjectFall : MonoBehaviour
{
    public float tiempoTemblor = 0.5f;
    public float tiempoCaida = 2f;
    public float intensidadTemblor = 0.05f;
    public float tiempoDestruccion = 5f;

    private bool activada = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!activada && collision.gameObject.CompareTag("Player"))
        {
            activada = true;
            StartCoroutine(TemblarYCaer());
        }
    }

    System.Collections.IEnumerator TemblarYCaer()
    {
        Vector3 posicionOriginal = transform.position;

        float tiempo = 0f;
        while (tiempo < tiempoTemblor)
        {
            float offsetX = Random.Range(-intensidadTemblor, intensidadTemblor);
            float offsetZ = Random.Range(-intensidadTemblor, intensidadTemblor);
            transform.position = posicionOriginal + new Vector3(offsetX, 0, offsetZ);

            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.position = posicionOriginal;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Destroy(gameObject, tiempoDestruccion);
    }


}
