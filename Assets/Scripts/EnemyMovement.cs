using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movimiento")]
    private RaycastHit2D _raycastSueloI, _raycastSueloD, _raycastParedI, _raycastParedD;
    [SerializeField] private Transform _posRaycastI, _posRaycastD;
    [SerializeField] private LayerMask _capaSuelo;
    [SerializeField] private float _velocidadMovimiento;
    private int _direction = 1;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Movimiento();
    }

    private void Update()
    {
        GestionarRaycast();
    }

    private void GestionarRaycast()
    {
        Debug.DrawRay(_posRaycastI.position, Vector2.down * 0.5f, Color.green);
        Debug.DrawRay(_posRaycastD.position, Vector2.down * 0.5f, Color.green);
        Debug.DrawRay(_posRaycastI.position, Vector2.left * 0.25f, Color.green);
        Debug.DrawRay(_posRaycastD.position, Vector2.right * 0.25f, Color.green);
        _raycastSueloI = Physics2D.Raycast(_posRaycastI.position, Vector2.down, 0.5f, _capaSuelo);
        _raycastSueloD = Physics2D.Raycast(_posRaycastD.position, Vector2.down, 0.5f, _capaSuelo);
        _raycastParedI = Physics2D.Raycast(_posRaycastI.position, Vector2.left, 0.25f, _capaSuelo);
        _raycastParedD = Physics2D.Raycast(_posRaycastD.position, Vector2.right, 0.25f, _capaSuelo);

        if (!_raycastSueloI || _raycastParedI)
        {
            _direction = 1;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (!_raycastSueloD || _raycastParedD)
        {
            _direction = -1;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if ((!_raycastSueloI && !_raycastSueloD) || (_raycastParedD && _raycastParedI))
            _direction = 0;
    }

    private void Movimiento()
    {
        rb.velocity = new Vector2(_direction * _velocidadMovimiento * Time.fixedDeltaTime, rb.velocity.y);
    }
}