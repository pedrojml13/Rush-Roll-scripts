using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] float spawnInterval = 3f;
    [SerializeField] float objectLifetime = 5f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnObject), 0f, spawnInterval);
    }

    void SpawnObject()
    {
        GameObject instance = Instantiate(objectToSpawn, transform.position, transform.rotation);

        Destroy(instance, objectLifetime);
    }
}
