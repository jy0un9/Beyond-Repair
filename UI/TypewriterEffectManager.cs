using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TypewriterEffectManager : MonoBehaviour
{
    public List<TextMeshProUGUI> textComponents;
    public float typeSpeed = 0.05f;
    public float delayBetweenTexts = 1f;
    public float waitAfterTyping = 5f;

    void Start()
    {
        foreach (TextMeshProUGUI textComponent in textComponents)
        {
            Color textColor = textComponent.color;
            textColor.a = 0;
            textComponent.color = textColor;
        }

        StartCoroutine(TypeAllTexts());
    }

    IEnumerator TypeAllTexts()
    {
        foreach (TextMeshProUGUI textComponent in textComponents)
        {
            string originalText = textComponent.text;
            textComponent.text = "";
            Color textColor = textComponent.color;
            textColor.a = 1;
            textComponent.color = textColor;

            foreach (char letter in originalText.ToCharArray())
            {
                textComponent.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }

            yield return new WaitForSeconds(delayBetweenTexts);
        }

        yield return new WaitForSeconds(waitAfterTyping);
        SceneManager.LoadScene("Tittle");
    }
}