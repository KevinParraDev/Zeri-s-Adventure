using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //----------------------- Var Extra -------------------------

    private Rigidbody2D rb;
    private Animator _anim;
    [SerializeField] private bool _viva = true;

    //-----------------------------------------------------------
    [Header("Inputs")]
    [SerializeField] private LayerMask _capaSuelo;
    private bool _enSuelo, _enPared, _enParedD, _nParedI;
    [SerializeField] private bool _puedeSaltar, _puedeHacerDash = true;
    private float _gravedadInicial;
    private bool _sePuedeMover = true;

    [SerializeField] private Transform _transformSuelo, _transformParedI, _transformParedD;
    [SerializeField] private Vector2 _tamañoCajaSuelo, _tamañoCajaPared;

    [Header("Movimiento")]
    [SerializeField] private float _velocidadMovimiento;
    [Range(0, 0.3f)] public float _suavisadoMovimiento;
    private Vector3 _velocidadZero = Vector3.zero;
    private bool _mirarDerecha = true;
    private bool _agachado = false;

    [Header("Salto")]
    [SerializeField] private float _fuerzaSalto;
    [SerializeField] private float _fuerzaSaltoParedX, _fuerzaSaltoParedY;
    [SerializeField] private float _tiempoSaltoPared;
    [SerializeField] private float _tiempoCoyoteTime, _tiempoGuardadoSalto;
    private bool _saltandoEnPared = false;
    private float _dirSaltoPared;
    private bool _saltoGuardado = false;
    private bool _cayendo = false;
    private bool _atacada = false;

    [Header("Pisoton")]
    public bool _pisoton;
    [SerializeField] private bool _pisotonDesbloqueado = false;
    [SerializeField] private float _fuerzaPisoton;

    [Header("Dash")]
    [SerializeField] private bool _dashDesbloqueado = false;
    [SerializeField] private float _velocidadDash;
    [SerializeField] private float _tiempoDash;

    [Header("Trepar")]
    private bool _agarrarPared, _primerToque = true, _deslizandose;
    [SerializeField] private float _velocidadDeTrepado;
    [SerializeField] private float _velocidadDeDeslizamiento;

    [Header("Volar")]
    [SerializeField] private bool _planearDesbloquado = false;
    [SerializeField] private bool _puedeVolar = false;
    [SerializeField] private bool _volando = false;
    [SerializeField] private float _fuerzaFlotar;


    [Header("Interaccion")]
    [SerializeField] private GameObject objetoInteractivo;
    [SerializeField] private GameObject altarActivo;

    [Header("Sonidos")]
    [SerializeField] private AudioSource arerrizarSound;

    private Vector3 _lastCheckpoint;

    private void Start()
    {
        _viva = true;
        rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _gravedadInicial = rb.gravityScale;
    }

    private void Update()
    {
        if (_viva)
        {
            DetectarEstado();
            GetInputs();
        }
    }

    public void Atacada(float x, float y)
    {
        _atacada = true;
        rb.velocity = new Vector2(0, 0);
        rb.velocity = new Vector2(x, y);
        StartCoroutine(ResetAtacada());
    }

    IEnumerator ResetAtacada()
    {
        yield return new WaitForSeconds(0.1f);
        _atacada = false;
    }

    public void DesbloquearHabilidad(string habilidad)
    {
        switch (habilidad)
        {
            case "Dash": _dashDesbloqueado = true; break;
            case "Pisoton": _pisotonDesbloqueado = true; break;
            case "Planear": _planearDesbloquado = true; break;
        }
    }

    private void FixedUpdate()
    {
        if (_viva)
        {
            DetectarEntorno();
            // Moverse
            if (_sePuedeMover)
                Moverse(Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime);

            // Trepar
            if (_agarrarPared)
                Trepar(Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime);
        }
    }

    private void GetInputs()
    {
        //Interactuar
        if (Input.GetKeyDown(KeyCode.E) && objetoInteractivo != null)
        {
            if (objetoInteractivo.TryGetComponent<Palanca>(out Palanca objeto))
                objeto.Interaccion();

            if (objetoInteractivo.TryGetComponent<Cartel>(out Cartel objeto3))
                objeto3.Interaccion();
        }

        if (Input.GetKeyDown(KeyCode.E) && altarActivo != null)
        {
            if (altarActivo.TryGetComponent<Altar>(out Altar objeto2))
                objeto2.Interaccion(this);
        }

        // flotar
        if (Input.GetButton("Fire2") && _puedeVolar && _planearDesbloquado && !_agarrarPared)
        {
            Flotar(true);
        }
        else if (Input.GetKeyUp(KeyCode.H) || _agarrarPared && _volando)
        {
            Flotar(false);
        }

        // Saltar
        if ((Input.GetButtonDown("Jump")) && _puedeSaltar && !_atacada)
        {
            Saltar();
        }
        else if ((Input.GetButtonDown("Jump")) && !_puedeSaltar)
        {
            StartCoroutine(GuardarSalto());
        }

        // Agacharse
        if (Input.GetAxisRaw("Vertical") < 0 && _agachado == false && !_agarrarPared && _enSuelo)
            Agacharse(true);
        else if (Input.GetAxisRaw("Vertical") >= 0 && _agachado == true)
            Agacharse(false);

        // Atacar
        if (Input.GetButtonDown("Fire1"))
            Atacar();

        // Dash
        if (Input.GetButtonDown("Fire3") && _puedeHacerDash && _dashDesbloqueado)
            StartCoroutine(Dash());

        // Agarrar Pared
        if (Input.GetAxisRaw("Horizontal") != 0 && rb.velocity.y < 0 && _enPared && !_agarrarPared && !_saltandoEnPared)
            Deslizarse(true);
        else if (_deslizandose && !_agarrarPared)
            Deslizarse(false);

        if (_enPared && Input.GetButton("Fire2") && !_saltandoEnPared && _primerToque)
            AgarrarPared(true);
        else if (Input.GetButtonUp("Fire2"))
            AgarrarPared(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Palanca>() || other.GetComponent<Cartel>())
        {
            objetoInteractivo = other.gameObject;
        }

        if (other.GetComponent<Altar>())
        {
            altarActivo = other.gameObject;
        }

        if (other.GetComponent<Cartel>())
            other.GetComponent<Cartel>().Interaccion(true);

        if (other.gameObject.name == "Flotar")
        {
            _puedeVolar = true;
        }

        if (other.tag == "checkpoint")
        {
            _lastCheckpoint = other.gameObject.transform.position;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Palanca>() || other.GetComponent<Cartel>())
            objetoInteractivo = null;

        if (other.GetComponent<Altar>())
            altarActivo = null;

        if (other.GetComponent<Cartel>())
            other.GetComponent<Cartel>().Interaccion(false);

        if (other.gameObject.name == "Flotar")
        {
            _puedeVolar = false;
            Flotar(false);
        }
    }

    private void DetectarEntorno()
    {
        _enSuelo = Physics2D.OverlapBox(_transformSuelo.position, _tamañoCajaSuelo, 0f, _capaSuelo);
        _nParedI = Physics2D.OverlapBox(_transformParedI.position, _tamañoCajaPared, 0f, _capaSuelo);
        _enParedD = Physics2D.OverlapBox(_transformParedD.position, _tamañoCajaPared, 0f, _capaSuelo);
    }

    private void DetectarEstado()
    {
        if (_nParedI || _enParedD)
            _enPared = true;
        else
        {
            _enPared = false;
            if (_agarrarPared)
                AgarrarPared(false);
        }

        if (_enSuelo)
        {
            if (_cayendo)
            {
                _cayendo = false;
                arerrizarSound.Play();
                _anim.SetTrigger("Aterrizar");
            }
        }

        if (_enSuelo || _enPared)
        {
            _puedeHacerDash = true;
            _puedeSaltar = true;
            if (_saltoGuardado)
            {
                _saltoGuardado = false;
                Saltar();
            }

            if (_pisoton)
                _pisoton = false;
        }
        else
        {
            if (Input.GetAxisRaw("Vertical") < 0 && _pisoton == false && _pisotonDesbloqueado)
            {
                Debug.Log("pisoton");
                _pisoton = true;
                rb.velocity = new Vector2(0, _fuerzaPisoton);
            }
            Debug.Log("No suelo ni pared");
            if (rb.velocity.y < 0 && _cayendo == false)
            {
                _cayendo = true;
                Debug.Log("Anim caer");
                _anim.SetTrigger("Caer");
            }
            StartCoroutine(CoyoteTime());   //Guardar salto aqui
        }
    }

    private void Moverse(float inputX)
    {
        float movimiento = inputX * _velocidadMovimiento;

        if (movimiento != 0)
            _anim.SetBool("Caminar", true);
        else
            _anim.SetBool("Caminar", false);

        if (!_saltandoEnPared)
        {
            Vector3 velocidadFinal = new Vector2(movimiento, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadFinal, ref _velocidadZero, _suavisadoMovimiento);
        }

        //Cambiar la direccion a la que saltará cuando este en una pared (Antes de girar)
        DireccionDeSalto();

        //Girar
        if ((movimiento > 0 && !_mirarDerecha) || (movimiento < 0 && _mirarDerecha))
            Girar();

    }

    private void Girar()
    {
        _mirarDerecha = !_mirarDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void DireccionDeSalto()
    {
        if (_nParedI)
            _dirSaltoPared = transform.localScale.x;
        else if (_enParedD)
            _dirSaltoPared = -1 * transform.localScale.x;
    }

    private string TipoDeSalto()
    {
        if ((_enSuelo && _enPared) || _enSuelo)
            return "Basico";
        else if (_enPared)
            return "Pared";
        else
            return "Basico";
    }

    private void Saltar()
    {
        AgarrarPared(false);
        _anim.SetTrigger("Saltar");

        switch (TipoDeSalto())
        {
            case "Basico":
                rb.AddForce(new Vector2(0, _fuerzaSalto));
                break;
            case "Pared":
                StartCoroutine(SaltandoPared());
                Girar();
                rb.velocity = new Vector2(_fuerzaSaltoParedX * _dirSaltoPared, _fuerzaSaltoParedY);
                break;
        }
    }

    IEnumerator GuardarSalto()
    {
        _saltoGuardado = true;
        yield return new WaitForSeconds(_tiempoGuardadoSalto);
        _saltoGuardado = false;
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(_tiempoCoyoteTime);
        _puedeSaltar = false;
    }

    IEnumerator SaltandoPared()
    {
        _saltandoEnPared = true;
        yield return new WaitForSeconds(_tiempoSaltoPared);
        _saltandoEnPared = false;
    }

    private IEnumerator Dash()
    {
        Debug.Log("Dash");

        _anim.SetBool("Dash", true);
        _puedeHacerDash = false;
        _sePuedeMover = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(_velocidadDash * transform.localScale.x, 0);

        yield return new WaitForSeconds(_tiempoDash);
        _anim.SetBool("Dash", false);
        _sePuedeMover = true;
        Debug.Log("Gravedad normal dash");
        rb.gravityScale = _gravedadInicial;
    }

    private void Agacharse(bool e)
    {
        Debug.Log("Agacharse:" + e);
        _agachado = e;
        string t = e ? "Agacharse" : "Pararse";
        _anim.SetTrigger(t);
    }

    private void Atacar()
    {
        Debug.Log("Atacar");
    }

    private void Pisoton()
    {
        Debug.Log("Pisoton");
    }

    private void Deslizarse(bool deslizandose)
    {
        _deslizandose = deslizandose;
        if (_deslizandose)
        {
            rb.gravityScale = _velocidadDeDeslizamiento;
            deslizandose = true;
            _anim.SetTrigger("EnPared");
        }
        else
        {
            rb.gravityScale = _gravedadInicial;
            deslizandose = false;
        }
    }

    private void AgarrarPared(bool agarrarPared)
    {
        _agarrarPared = agarrarPared;
        _primerToque = true;

        if (_agarrarPared)
        {
            print("Trepando");
            _anim.SetTrigger("AgarrarPared");
            _primerToque = false;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);
            _sePuedeMover = false;
        }
        else
        {
            print("Dejó de trepar");
            rb.gravityScale = _gravedadInicial;
            _sePuedeMover = true;
        }
    }

    private void Trepar(float inputY)
    {
        if (inputY == 0)
            _anim.SetTrigger("AgarrarPared");
        else if (inputY > 0)
            _anim.SetBool("Trepar", true);
        else if (inputY < 0)
            _anim.SetBool("Trepar", true);

        float movimiento = inputY * _velocidadDeTrepado;
        rb.velocity = new Vector2(0, movimiento);
    }

    private void Flotar(bool flotar)
    {
        if (flotar)
        {
            _cayendo = false;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, _fuerzaFlotar);
            _volando = true;
            Debug.Log("Volar");
            _anim.SetTrigger("Saltar");
        }
        else if (!flotar)
        {
            _volando = false;
            rb.gravityScale = _gravedadInicial;
        }
    }

    public void Morir()
    {
        Debug.Log("Desaparecer");
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        _viva = false;
        Debug.Log("Holis");
        _anim.SetTrigger("Morir");
    }

    public IEnumerator delayReaparecer()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSecondsRealtime(0.5f);
        Reaparecer();
    }

    public void Reaparecer()
    {
        _viva = true;
        gameObject.transform.position = _lastCheckpoint;
        GetComponent<SpriteRenderer>().enabled = true;
        _anim.SetTrigger("Reaparecer");
    }

    public void VolverGravedad()
    {
        rb.gravityScale = _gravedadInicial;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_transformSuelo.position, _tamañoCajaSuelo);
        Gizmos.DrawWireCube(_transformParedD.position, _tamañoCajaPared);
        Gizmos.DrawWireCube(_transformParedI.position, _tamañoCajaPared);
    }
}
