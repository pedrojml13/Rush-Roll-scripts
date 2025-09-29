using UnityEngine;
using TMPro;
using System.Collections;

public class ShopTextAnimator : MonoBehaviour
{
    private TMP_Text text;
    public float letterDelay = 0.1f;
    public float cycleDelay = 1f;
    public float blinkDuration = 0.2f;

    private string originalText;


    IEnumerator Start()
    {
        yield return null; // Espera un frame
        text = GetComponent<TMP_Text>();
        originalText = text.text;
        StartCoroutine(AnimateTextLoop());
    }

    IEnumerator AnimateTextLoop()
    {
        while (true)
        {
            text.text = "";

            for (int i = 0; i < originalText.Length; i++)
            {
                text.text += originalText[i];
                text.ForceMeshUpdate();
                TMP_TextInfo textInfo = text.textInfo;

                if (!textInfo.characterInfo[i].isVisible) continue;


                text.UpdateVertexData();
                yield return new WaitForSeconds(letterDelay);
            }
            
            for (int b = 0; b < 2; b++)
            {
                text.enabled = false;
                yield return new WaitForSeconds(blinkDuration);
                text.enabled = true;
                yield return new WaitForSeconds(blinkDuration);
            }
            yield return new WaitForSeconds(cycleDelay);
        }
    }
}