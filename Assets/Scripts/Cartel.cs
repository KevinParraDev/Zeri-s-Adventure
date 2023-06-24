using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cartel : MonoBehaviour
{
    [SerializeField] Animator botonAnim;
    [SerializeField] private int indexDialogo;
    [SerializeField] private DialogSystem dialogSystem;
    [SerializeField] private bool activo = false;

    public void Interaccion()
    {
        activo = !activo;
        dialogSystem.MostrarCartel(indexDialogo);
    }

    public void Interaccion(bool e)
    {
        if (e == true)
        {
            botonAnim.SetTrigger("Aparecer");
        }
        else
        {
            botonAnim.SetTrigger("Desaparecer");
        }

        if (activo)
        {
            activo = !activo;
            dialogSystem.MostrarCartel(indexDialogo);
        }
    }
}
