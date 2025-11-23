using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class ControlJugador : MonoBehaviour
{
    [Header("Movimiento Jugador")]
    public Transform jugador;
    public float velocidad = 5f;
    public float velocidadRotacion = 10f;
    public float fuerzaDeSalto = 5f;

    [Header("Movimiento Camara")]
    public Transform transformCamara;
    public float distanciaCamara = 5f;
    public float sencibilidadCamara = 5f;
    private float rotacionYcam, rotacionXcam;
    public float limiteInferiro, limiteSuperior;
    public float distanciaCamaraMinimo;
    public float velocidadDecorreccion = 10f;

    [Header("Funcionamiento Del Arma")]
    public GameObject bala;
    public Transform mira;
    public int recarga;
    public float cadencia;
    private float espera;
    public GameObject imagenMira;
    public TextMeshProUGUI contadorBalas;
    public GameObject textoRecargar;
    int municion;

    [Header("Efectos Mutageno 1")]
    public float alteracionVelocidadMutUno;
    public float tiempoDeActivacionMutUno;

    [Header("Efectos Mutageno 2")]
    public float alteracionVelocidadMutDos;
    public float tiempoDeActivacionMutDos;

    [Header("Extras")]
    public Transform arma;
    public int vida;
    public Slider barraVida;
    public GameObject CanvasDerrota;
    public bool[] mutacion;
    public float[] tempMut;
    public GameObject[] particulasMutageno;

    [Header("Animacion")]
    public Animator animator;
    public string aniVelocidadXZ;
    public string aniVelocidadY;
    public string aniDisparando;

    private Rigidbody rb;
    float gatilloL, gatilloR, alterecionVelocidadP, alteracionVelocidadN, distanciaCamaraReal;
    int vidaActual;

    void Awake()
    {
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();

        rotacionYcam = transform.eulerAngles.y;
        rotacionXcam = transform.eulerAngles.x;

        Cursor.visible = false;

        alteracionVelocidadN = 1;
        alterecionVelocidadP = 1;
        vidaActual = vida;
        barraVida.maxValue = vida;
        barraVida.value = vidaActual;
        distanciaCamaraReal = distanciaCamara;
        ActivacionDeParticulasMutageno();
        municion = recarga;

        Trampa.Activar += Trampas;
    }

    private void OnDestroy()
    {
        Trampa.Activar -= Trampas;
    }

    void Update()
    {
        MovimientoJugador();
        MovimientoCamara();

        Gatillos();
        Disparar();
        Apuntar();

        Saltar();

        ResetearPosision();
        CondicionDeDerrota();
        ControlDeAnimacion();

        TemporizadorDeMutacion();
        ActivacionDeParticulasMutageno();
        ControlDeMunicion();

        barraVida.value = vidaActual;
    }

    private void MovimientoCamara()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotacionYcam += mouseX * sencibilidadCamara;
        rotacionXcam += mouseY * sencibilidadCamara;

        rotacionXcam = Mathf.Clamp(rotacionXcam, -limiteSuperior, limiteInferiro);

        Quaternion rotacionCamara = Quaternion.Euler(rotacionXcam, rotacionYcam, 0f);
        Vector3 direccionCamara = rotacionCamara * Vector3.back;
        float distanciaAjustada = AjustarDistanciaCamara(transform.position, direccionCamara, distanciaCamaraReal, distanciaCamaraMinimo);
        Vector3 offset = direccionCamara * distanciaAjustada;

        transformCamara.position = transform.position + offset;
        transformCamara.LookAt(transform.position);

        Vector3 anguloParaArma = new Vector3(transformCamara.rotation.eulerAngles.x, transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z);
        Quaternion anguloArma = Quaternion.Euler(anguloParaArma);

        arma.rotation = Quaternion.Slerp(arma.rotation, anguloArma, 10 * Time.deltaTime);
    }

    private void MovimientoJugador()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direccionInput = new Vector3(h, 0f, v).normalized;

        Quaternion rotacionJugador = Quaternion.Euler(0f, rotacionYcam, 0f);
        Vector3 direccion = rotacionJugador * direccionInput;

        Vector3 velocidadDireccionada = direccion * ((velocidad * alteracionVelocidadN) * alterecionVelocidadP);
        rb.velocity = new Vector3(velocidadDireccionada.x, rb.velocity.y, velocidadDireccionada.z);

        if (direccionInput.sqrMagnitude > 0.01f && !(gatilloR > 0 || Input.GetButton("Fire1")))
        {
            Quaternion anguloObjetivo = Quaternion.LookRotation(direccion, Vector3.up);
            Quaternion rotacion = Quaternion.Slerp(rb.rotation, anguloObjetivo, velocidadRotacion * Time.deltaTime);
            rb.MoveRotation(rotacion);
        }
    }

    void ResetearPosision()
    {
        if (transform.position.y < -10)
        {
            transform.position = Vector3.up * 2;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void Saltar()
    {
        if (Input.GetButtonDown("Saltar") && ComprobacioSuelo())
            rb.velocity += Vector3.up * fuerzaDeSalto;
    }

    bool ComprobacioSuelo()
    {
        return Physics.Raycast(jugador.position, Vector3.down, 1.55f);
    }

    public void Apuntar()
    {
        if (imagenMira != null)
        {
            if (Input.GetButton("Fire2") || gatilloL > 0)
            {
                imagenMira.SetActive(true);
            }
            else
            {
                imagenMira.SetActive(false);
            }
        }
    }

    public void Disparar()
    {
        if ((gatilloR > 0 || Input.GetButton("Fire1")) && (municion > 0 && espera <= 0))
        {
            Vector3 camEuler = transformCamara.rotation.eulerAngles;
            Quaternion targetRot = Quaternion.Euler(0f, camEuler.y, 0f);

            StartCoroutine(ShootAfterRotate(targetRot));

            espera = cadencia;
            municion--;
        }
        else if (espera > 0)
        {
            espera -= Time.deltaTime;
        }
    }

    public void Gatillos()
    {
        if (Gamepad.current != null)
        {
            var gamepad = Gamepad.current;
            gatilloL = gamepad.leftTrigger.ReadValue();
            gatilloR = gamepad.rightTrigger.ReadValue();
        }
        else
        {
            gatilloL = 0;
            gatilloR = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mutageno 1") && mutacion[0] == false)
        {
            Alteraciones(alteracionVelocidadMutUno, .5f, false);
            mutacion[0] = true;
            tempMut[0] = tiempoDeActivacionMutUno;
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Mutageno 2") && mutacion[1] == false)
        {
            Alteraciones(-alteracionVelocidadMutDos, 2, true);
            mutacion[1] = true;
            tempMut[1] = tiempoDeActivacionMutDos;
            other.gameObject.SetActive(false);
        }
        else if (other.gameObject.CompareTag("Bala Enemigo"))
        {
            vidaActual -= 1;
        }
        else if (other.gameObject.CompareTag("Municion"))
        {
            other.gameObject.SetActive(false);
            municion = recarga;
        }
    }

    void Alteraciones(float valor, float escala = 0, bool negativo = false)
    {
        if (negativo)
            alteracionVelocidadN += valor;
        else
            alterecionVelocidadP += valor;

        if (escala > 0)
        {
            rb.velocity += Vector3.up * 2 * escala;
            transform.localScale *= escala;
        }
    }

    void TemporizadorDeMutacion()
    {
        if (mutacion[0] == true)
        {
            tempMut[0] -= Time.deltaTime;

            if (tempMut[0] < 0)
            {
                mutacion[0] = false;
                Alteraciones(-alteracionVelocidadMutUno, 2f);
            }
        }

        if (mutacion[1] == true)
        {
            tempMut[1] -= Time.deltaTime;

            if (tempMut[1] < 0)
            {
                mutacion[1] = false;
                Alteraciones(alteracionVelocidadMutDos, .5f, true);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        rb.angularVelocity = Vector3.zero;
    }

    private System.Collections.IEnumerator ShootAfterRotate(Quaternion targetRot)
    {
        rb.MoveRotation(targetRot);            
        yield return new WaitForFixedUpdate();    
        Instantiate(bala, mira.position, mira.rotation);
    }

    void CondicionDeDerrota()
    {
        if (vidaActual <= 0)
        {
            Time.timeScale = 0;
            CanvasDerrota.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void ControlDeAnimacion()
    {
        animator.SetFloat(aniVelocidadXZ, math.abs(rb.velocity.x) + math.abs(rb.velocity.z));
        animator.SetFloat(aniVelocidadY, math.abs(rb.velocity.y));
        animator.SetBool(aniDisparando, gatilloR > 0 || Input.GetButton("Fire1"));
    }

    void Trampas(int daño, int indice)
    {
        if (indice == 0)
            vidaActual -= daño;
        else if (indice == 1 && !mutacion[1])
            vidaActual -= daño;
    }

    private float AjustarDistanciaCamara(Vector3 origen, Vector3 direccion, float distanciaMaxima, float distanciaMinima)
    {
        RaycastHit hit;

        if (Physics.Linecast(origen, origen + direccion * distanciaMaxima, out hit))
        {
            float distanciaImpacto = Vector3.Distance(origen, hit.point);
            return Mathf.Clamp(distanciaImpacto, distanciaMinima, distanciaMaxima);
        }

        return distanciaMaxima;
    }

    void ActivacionDeParticulasMutageno()
    {
        particulasMutageno[0].SetActive(mutacion[0]);
        particulasMutageno[1].SetActive(mutacion[1]);
    }

    void ControlDeMunicion()
    {
        contadorBalas.text = municion.ToString();

        if (municion == 0 && textoRecargar != null)
            textoRecargar.SetActive(true);
        else if (textoRecargar != null)
            textoRecargar.SetActive(false);
    }
}
