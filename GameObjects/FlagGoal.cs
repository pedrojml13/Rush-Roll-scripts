using System;
using UnityEngine;

public class FlagGoal : MonoBehaviour
{
    [SerializeField] private AudioClip flagGoalClip;
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

        mesh.SetTriangles(empty, 1);
        mesh.SetTriangles(empty, 2);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isBroken)
        {
            AudioManager.Instance.PlaySFX(flagGoalClip);
            AudioManager.Instance.StopLoop("Rolling");
            LevelManager.Instance.WinLevel();
        }
    }
}
