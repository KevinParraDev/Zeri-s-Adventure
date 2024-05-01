using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType { Frente, Arriba, Abajo }

public class MeleeAttack : MonoBehaviour
{
    AttackType attackType;
    private Rigidbody2D rb;
    [SerializeField] private float fuerzaRetrocesoY;
    [SerializeField] private float fuerzaRetrocesoX;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void DetectarInputs()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            attackType = AttackType.Arriba;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            attackType = AttackType.Abajo;
        else
            attackType = AttackType.Frente;
    }

    public void Atacar()
    {
        DetectarInputs();

        if (attackType == AttackType.Frente)
        {
            Debug.Log("Attaque normal");
            AplicarRetroceso(-fuerzaRetrocesoX, 0);
        }
        else if (attackType == AttackType.Arriba)
        {
            Debug.Log("Attaque arriba");
        }
        else if (attackType == AttackType.Abajo)
        {
            Debug.Log("Attaque abajo");
            AplicarRetroceso(0, fuerzaRetrocesoY);
        }
    }

    public void AplicarRetroceso(float x, float y)
    {
        rb.velocity = new Vector2(0, 0);
        rb.velocity = new Vector2(x, y);
    }

    private void Update()
    {
        //if (Input.GetButtonDown("Fire1"))
        //    Atacar();
    }
}
