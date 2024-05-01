using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatoMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Transform player;
    [SerializeField] private float maximaSeparacion;
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float constanteVelocidad;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Mover()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        //transform.position += dir * velocidadMovimiento;
        rb.velocity = dir * velocidadMovimiento;
    }

    private void Update()
    {
        float distancia = Vector2.Distance(player.position, transform.position);
        velocidadMovimiento = distancia * constanteVelocidad;

        if (distancia >= maximaSeparacion)
        {
            Mover();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
