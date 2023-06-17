using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palanca : MonoBehaviour
{
    [SerializeField] private float posA;
    [SerializeField] private float posB;
    [SerializeField] private GameObject puerta;
    [SerializeField] private GameObject seccionFlotar;
    [SerializeField] private bool open;

    public void Interaccion()
    {
        if (!open)
        {
            Debug.Log("Abrir");
            open = true;
            puerta.transform.position = new Vector3(posB, puerta.transform.position.y);
            seccionFlotar.SetActive(true);
        }
        else
        {
            Debug.Log("Cerrar");
            open = false;
            puerta.transform.position = new Vector3(posA, puerta.transform.position.y);
            seccionFlotar.SetActive(false);
        }
    }
}
