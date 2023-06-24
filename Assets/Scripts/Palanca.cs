using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palanca : MonoBehaviour
{
    [SerializeField] private float posA;
    [SerializeField] private float posB;
    [SerializeField] private Animator puertaAnim;
    [SerializeField] private GameObject seccionFlotar;
    [SerializeField] private bool open;
    [SerializeField] private Sprite[] spritesPalanca;

    public void Interaccion()
    {
        if (!open)
        {
            Debug.Log("Abrir");
            open = true;
            puertaAnim.SetTrigger("Abrir");
            GetComponent<SpriteRenderer>().sprite = spritesPalanca[1];
            seccionFlotar.SetActive(true);
        }
        else
        {
            Debug.Log("Cerrar");
            open = false;
            puertaAnim.SetTrigger("Cerrar");
            GetComponent<SpriteRenderer>().sprite = spritesPalanca[0];
            seccionFlotar.SetActive(false);
        }
    }
}
