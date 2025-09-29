using System;
using UnityEngine;

public class FlagStart : MonoBehaviour
{
    [SerializeField] private AudioClip flagStartClip;
    private MeshFilter mf;

    public bool isBroken = false;

    void Start()
    {
        mf = GetComponent<MeshFilter>();
        
    }
    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        Mesh mesh = Instantiate(mf.mesh);
        mf.mesh = mesh;

        int[] empty = Array.Empty<int>();

        mesh.SetTriangles(empty, 0);
        mesh.SetTriangles(empty, 2);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBroken)
        {
            AudioManager.Instance.PlaySFX(flagStartClip);
            Break();
            LevelManager.Instance.StartFlag();
        }
    }


}
