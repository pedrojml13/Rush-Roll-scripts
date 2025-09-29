using UnityEngine;
using System.Collections;

public class ObjectBridge : MonoBehaviour
{
    
    [SerializeField] private ObjectButton button1;
    [SerializeField] private ObjectButton button2;
    [SerializeField] private float delayBetweenTiles = 0.1f;
    [SerializeField] private float tileRiseHeight = 2f;
    [SerializeField] private float tileRiseDuration = 0.3f;

    //Sound
    [SerializeField] private AudioClip tileRevealSound;
    [SerializeField] private AudioClip tileHideSound;

    private float baseTilePositionY;
    private bool hasRevealed = false;
    private bool hasHide = true;
    private bool revealing = false;
    private bool hiding = false;

    void Start()
    {
        baseTilePositionY = transform.GetChild(0).localPosition.y;
    }

    void Update()
    {
        if (!revealing && !hiding && !hasRevealed)
        {
            if (button2 == null && button1.pressed)
            {
                StartCoroutine(RevealTiles());
                hasRevealed = true;
                hasHide = false;
            }
            else if(button2 != null && button1.pressed && button2.pressed)
            {
                StartCoroutine(RevealTiles());
                hasRevealed = true;
                hasHide = false;
            }
        }

        else if (!revealing && !hiding && !hasHide)
        {
            if (button2 == null && !button1.pressed)
            {
                StartCoroutine(HideTiles());
                hasRevealed = false;
                hasHide = true;
            }
            else if (button2 != null && (!button1.pressed || !button2.pressed))
            {
                StartCoroutine(HideTiles());
                hasRevealed = false;
                hasHide = true;
            }
        }
    }

    private IEnumerator RevealTiles()
    {
        revealing = true;

        foreach (Transform tile in gameObject.transform)
        {
            Vector3 targetPos = tile.localPosition;
            targetPos.y = baseTilePositionY;

            Vector3 startPos = targetPos - new Vector3(0f, tileRiseHeight, 0f);
            tile.localPosition = startPos;

            tile.gameObject.SetActive(true);

            AudioManager.Instance.PlaySFX(tileRevealSound);
            
            float elapsed = 0f;
            while (elapsed < tileRiseDuration)
            {
                tile.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / tileRiseDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }


            tile.localPosition = targetPos;

            yield return new WaitForSeconds(delayBetweenTiles);
        }

        revealing = false;
    }

    private IEnumerator HideTiles()
    {
        hiding = true;
        foreach (Transform tile in gameObject.transform)
        {
            Vector3 startPos = tile.localPosition;
            Vector3 targetPos = startPos - new Vector3(0f, tileRiseHeight, 0f);
            targetPos.y = baseTilePositionY;

            float elapsed = 0f;
            while (elapsed < tileRiseDuration)
            {
                tile.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / tileRiseDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            tile.localPosition = targetPos;

            tile.gameObject.SetActive(false);

            AudioManager.Instance.PlaySFX(tileHideSound);

            yield return new WaitForSeconds(delayBetweenTiles);
        }

        hiding = false;
    }

}
