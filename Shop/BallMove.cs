using UnityEngine;

public class BallMove : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 20f;

    private int currentBallIndex = 0;
    private Texture currentTexture;

    private void Start()
    {
        ShowBall(currentBallIndex);
    }

    private void Update()
    {
        // Rotación automática de la bola mostrada
            //transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void ShowNextBall()
    {
        var skins = GameManager.Instance.GetAllSkins();
        if (skins.Count == 0) return;

        currentBallIndex = (currentBallIndex + 1) % skins.Count;
        ShowBall(currentBallIndex);
    }

    public void ShowPreviousBall()
    {
        var skins = GameManager.Instance.GetAllSkins();
        if (skins.Count == 0) return;

        currentBallIndex = (currentBallIndex - 1 + skins.Count) % skins.Count;
        ShowBall(currentBallIndex);
    }

    private void ShowBall(int index)
    {
        var skins = GameManager.Instance.GetAllSkins();
        if (skins.Count == 0 || index < 0 || index >= skins.Count) return;

        BallSkin skin = skins[index];
        currentTexture = skin.texture;
        GetComponent<Renderer>().material.mainTexture = currentTexture;

        Debug.Log($"Mostrando bola: {skin.name}, desbloqueada: {skin.isUnlocked}");
    }

    public BallSkin GetCurrentBall()
    {
        return GameManager.Instance.GetAllSkins()[currentBallIndex];
    }

    public int GetCurrentBallIndex()
    {
        return currentBallIndex;
    }
}
