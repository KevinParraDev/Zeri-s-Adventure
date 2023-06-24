using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField] private string habilidadDeAltar;

    public void Interaccion(PlayerMovement player)
    {
        player.DesbloquearHabilidad(habilidadDeAltar);
    }
}
