using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType { Idle, Alert, Chase, Attack, Stop, Search }

public class Bull : MonoBehaviour
{
    public StateType stateType;
    private Rigidbody2D _rb;
    private Animator _anim;

    public GameObject _player;

    [SerializeField] private float _alertRange;
    [SerializeField] private float _speed;
    [SerializeField] private int _direction = 1;

    [SerializeField] private Transform _colH, _colUp;
    [SerializeField] private Vector2 _hitBoxSize, _hitBoxSizeUp;
    [SerializeField] private LayerMask _capaSuelo;
    [SerializeField] private LayerMask _capaJugador;

    [SerializeField] private float _rebote;
    private bool _chocarPared;
    private bool _chocarJugador;
    private bool _pisado;
    private bool _puedeAtacar = false;
    public float forceAttackX, forceAttackY;

    private void ChangeState(StateType newState)
    {
        stateType = newState;

        switch (stateType)
        {
            case StateType.Idle:
                Idle();
                break;
            case StateType.Alert:
                Alert();
                break;
            case StateType.Chase:
                Chase();
                break;
            case StateType.Attack:
                Attack();
                break;
            case StateType.Search:
                Search();
                break;
        }
    }

    private void Start()
    {
        ChangeState(StateType.Idle);

        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void DetectarEntorno()
    {
        _chocarPared = Physics2D.OverlapBox(_colH.position, _hitBoxSize, 0f, _capaSuelo);
        _chocarJugador = Physics2D.OverlapBox(_colH.position, _hitBoxSize, 0f, _capaJugador);
        _pisado = Physics2D.OverlapBox(_colUp.position, _hitBoxSizeUp, 0f, _capaJugador);

        if (_chocarJugador)
            Chocar("Jugador");
        else if (_chocarPared)
        {
            Chocar("Pared");
        }

        if (_pisado)
        {
            Chocar("Pisado");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_colH.position, _hitBoxSize);
        Gizmos.DrawWireCube(_colUp.position, _hitBoxSizeUp);
    }

    private void Idle()
    {
        Debug.Log("Idle");
    }

    private void Alert()
    {
        Debug.Log("Bull alert");
        _anim.SetTrigger("Alert");
        StartCoroutine(stateDelay(1, StateType.Chase));
    }

    private void Chase()
    {
        Debug.Log("Bull Chase");
        _anim.SetTrigger("Chase");
        _rb.velocity = new Vector2(_direction * _speed, 0);
    }

    private void Chocar(string tipo)
    {
        switch (tipo)
        {
            case "Jugador":
                if (_puedeAtacar)
                {
                    Debug.Log("Chocó jugador");
                    _anim.SetTrigger("Attack");
                    ChangeState(StateType.Attack);
                }
                break;
            case "Pared":
                if (_puedeAtacar)
                {
                    Debug.Log("Chocó Pared");
                    _anim.SetTrigger("CrashWall");
                    ChoquePared();
                    //ChangeState(StateType.Attack);
                }
                break;
            case "Pisado":
                Debug.Log("Rebotar");
                _anim.SetTrigger("Death");
                Death();
                break;
        }
    }

    private void Death()
    {
        _rb.velocity = new Vector2(0, 0);
        _puedeAtacar = false;
        _player.GetComponent<PlayerMovement>().Atacada(0, _rebote);
    }

    public void DesactivarToro()
    {
        gameObject.SetActive(false);
    }

    private void Attack()
    {
        _rb.velocity = new Vector2(0, 0);
        _puedeAtacar = false;
        _player.GetComponent<PlayerMovement>().Atacada(forceAttackX * _direction, forceAttackY);
        _player.GetComponent<PlayerMovement>().Morir();
        _player.GetComponent<Health>().Attack(2);
        StartCoroutine(stateDelay(1, StateType.Search));
    }

    private void ChoquePared()
    {
        _rb.velocity = new Vector2(0, 0);
        _puedeAtacar = false;
        //StartCoroutine(stateDelay(1, StateType.Search));
    }

    public void TurnArround()
    {
        Debug.Log("Giraar");
        ChangeState(StateType.Search);
    }

    private void Search()
    {
        Debug.Log("Bull Search");

        if (transform.position.x > _player.transform.position.x)
        {
            _direction = -1;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            _direction = 1;
            GetComponent<SpriteRenderer>().flipX = true;
        }

        //StartCoroutine(stateDelay(1f, StateType.Chase));
        ChangeState(StateType.Chase);
    }

    IEnumerator stateDelay(float delay, StateType nextState)
    {
        yield return new WaitForSeconds(delay);
        ChangeState(nextState);
    }

    private void Update()
    {
        DetectarEntorno();

        if (stateType == StateType.Idle)
        {
            float dist = (transform.position - _player.transform.position).magnitude;
            if (dist <= _alertRange)
            {
                Debug.Log("En rango");
                ChangeState(StateType.Alert);
            }
        }

        if (stateType == StateType.Chase && !_chocarPared && !_chocarJugador)
            _puedeAtacar = true;
    }
}
