using UnityEngine;
using TMPro;

namespace PJML.RushAndRoll
{
    /// <summary>
    /// Gestiona la animación del texto de la tienda de dinero real
    /// </summary>
    public class Jumpingtext : MonoBehaviour
    {
        public TextMeshProUGUI tmp;
        public float jumpHeight = 10f;
        public float jumpDuration = 0.5f;
        public float delayBetweenLetters = 0.05f;

        private TMP_TextInfo textInfo;
        private Vector3[][] originalVertices;

        void Start()
        {
            tmp.ForceMeshUpdate();
            textInfo = tmp.textInfo;

            originalVertices = new Vector3[textInfo.meshInfo.Length][];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                originalVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
            }

            AnimateLetters();
        }

        void AnimateLetters()
        {
            int charCount = textInfo.characterCount;

            for (int i = 0; i < charCount; i++)
            {
                int index = i;
                LeanTween.value(gameObject, 0f, 1f, jumpDuration)
                    .setDelay(i * delayBetweenLetters)
                    .setEaseInOutSine()
                    .setLoopPingPong()
                    .setOnUpdate((float val) =>
                    {
                        if (!textInfo.characterInfo[index].isVisible) return;

                        int meshIndex = textInfo.characterInfo[index].materialReferenceIndex;
                        int vertexIndex = textInfo.characterInfo[index].vertexIndex;

                        Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

                        for (int j = 0; j < 4; j++)
                        {
                            vertices[vertexIndex + j] = originalVertices[meshIndex][vertexIndex + j] + Vector3.up * jumpHeight * val;
                        }

                        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    });
            }
        }
    }
}