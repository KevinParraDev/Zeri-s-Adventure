using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeriMovement : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [Header("Movimiento")]
    private float inputX;
    private float movimiento = 0f;
    public float velocidadMovimiento;
    [Range(0, 0.3f)] public float suavisadoMovimiento = 0f;
    private Vector3 velocidadZero = Vector3.zero;
    [SerializeField] private bool mirarDerecha = true;

    [Header("Salto")]
    public float fuerzaSalto;
    public LayerMask capaSuelo;
    public Transform transformSuelo;
    public Vector3 dimensionCaja;
    public bool enSuelo;
    private bool salto = false;
    public bool saltoGuardado = false;
    private float tiempoDeGuardado;
    private bool enAire = false;

    [Header("SaltoPared")]
    [SerializeField] private Transform controladorParedD, controladorParedI;
    [SerializeField] private Vector3 dimensionesCajaPared;
    [SerializeField] private bool enPared, enParedD, enParedI;
    [SerializeField] private bool puedeSaltarPared, saltandoEnPared;
    [SerializeField] private float fuerzaSaltoParedX, fuerzaSaltoParedY;
    [SerializeField] private float tiempoSaltoPared;

    [Header("Dash")]
    [SerializeField] private float velocidadDash;
    [SerializeField] private float tiempoDash;
    private float gravedadInicial;
    private bool puedeHacerDash = true;
    private bool sePuedeMover = true;

    void Start()
    {
        // Encontrar componentes
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravedadInicial = rb.gravityScale;
    }

    void Update()
    {
        //------------- Movimiento horizontal -------------
        inputX = Input.GetAxisRaw("Horizontal");
        movimiento = inputX * velocidadMovimiento;

        if (inputX != 0 && enSuelo)
            anim.SetBool("Caminando", true);
        else
            anim.SetBool("Caminando", false);


        //------------------- Salto -----------------------
        if (Input.GetButtonDown("Jump") && (enSuelo || enPared))
        {
            salto = true;
        }
        else if (Input.GetButtonDown("Jump") && !(enSuelo || enPared))
        {
            saltoGuardado = true;
            StartCoroutine(TiempoSaltoGuardado());
        }

        //------------------- Dash -----------------------

        if (Input.GetKeyDown(KeyCode.LeftShift) && puedeHacerDash)
            StartCoroutine(Dash());

        //------------------- Agacharse -----------------------
        if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("Agacharse", true);
        }
        else
            anim.SetBool("Agacharse", false);

        if (rb.velocity.y < 0)
        {
            anim.SetBool("Cayendo", true);
            enAire = true;
        }
        if (enAire && enSuelo)
            anim.SetBool("Cayendo", false);

        if (enPared && inputX != 0 && !enSuelo && !saltandoEnPared)
        {
            anim.SetTrigger("Deslizarse");
            enAire = true;
        }
        else if (enPared && !enSuelo && !saltandoEnPared)
            anim.SetBool("Cayendo", false);
    }


    private void FixedUpdate()
    {
        // Verificar si est� en el suelo
        if ((enSuelo || enPared) && (salto | saltoGuardado))
            Saltar();

        // Dibujar caja
        enSuelo = Physics2D.OverlapBox(transformSuelo.position, dimensionCaja, 0f, capaSuelo);
        enParedD = Physics2D.OverlapBox(controladorParedD.position, dimensionesCajaPared, 0f, capaSuelo);
        enParedI = Physics2D.OverlapBox(controladorParedI.position, dimensionesCajaPared, 0f, capaSuelo);

        if (enParedD || enParedI)
            enPared = true;
        else
            enPared = false;

        // Movimiento Horizontal
        if (sePuedeMover)
            Mover(movimiento * Time.fixedDeltaTime);
    }

    private void Mover(float mover)
    {
        if (!saltandoEnPared)
        {
            Vector3 velocidadFinal = new Vector2(mover, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadFinal, ref velocidadZero, suavisadoMovimiento);
        }


        //Girar el sprite
        if ((mover > 0 && !mirarDerecha) || (mover < 0 && mirarDerecha))
            Girar();
    }

    private IEnumerator Dash()
    {
        puedeHacerDash = false;
        sePuedeMover = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(velocidadDash * transform.localScale.x, 0);
        anim.SetBool("Dash", true);

        yield return new WaitForSeconds(tiempoDash);
        puedeHacerDash = true;
        sePuedeMover = true;
        rb.gravityScale = gravedadInicial;
        anim.SetBool("Dash", false);
    }

    private void Girar()
    {
        mirarDerecha = !mirarDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }


    IEnumerator CambiosSaltoPared()
    {
        saltandoEnPared = true;
        yield return new WaitForSeconds(tiempoSaltoPared);
        saltandoEnPared = false;
    }

    private void Saltar()
    {
        if (enSuelo)
        {
            rb.AddForce(new Vector2(0, fuerzaSalto));
            enSuelo = false;
            anim.SetTrigger("Saltar");
        }
        else if (enPared)
        {
            int dirSalto = 0;

            if (enParedD)
                dirSalto = -1;
            else if (enParedI)
                dirSalto = 1;

            if (!mirarDerecha)
                dirSalto = 1;

            rb.velocity = new Vector2(fuerzaSaltoParedX * dirSalto, fuerzaSaltoParedY);
            enPared = false;
            enParedD = false;
            enParedI = false;
            anim.SetTrigger("Saltar");
            StartCoroutine(CambiosSaltoPared());
        }
        saltoGuardado = false;
        salto = false;
    }

    IEnumerator TiempoSaltoGuardado()
    {
        yield return new WaitForSeconds(0.25f);
        saltoGuardado = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transformSuelo.position, dimensionCaja);
        Gizmos.DrawWireCube(controladorParedD.position, dimensionesCajaPared);
        Gizmos.DrawWireCube(controladorParedI.position, dimensionesCajaPared);
    }
}
