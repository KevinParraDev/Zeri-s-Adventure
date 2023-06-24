using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    [SerializeField] private float typingTime = 0.025f;

    [SerializeField] private Animator cuadroDialogoAnim;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;
    private bool cuadroActivo = false;

    private int lineIndex;


    public void MostrarCartel(int index)
    {
        if (!cuadroActivo)
        {
            lineIndex = index;
            cuadroActivo = true;
            cuadroDialogoAnim.SetTrigger("Aparecer");
            StartCoroutine(ShowLine());
        }
        else
        {
            cuadroActivo = false;
            dialogueText.text = "";
            cuadroDialogoAnim.SetTrigger("Desaparecer");
        }

    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        int charIndex = 0;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;

            charIndex++;
            yield return new WaitForSeconds(typingTime);
        }
    }
}
