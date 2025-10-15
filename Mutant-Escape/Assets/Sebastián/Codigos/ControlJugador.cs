using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;

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

    [Header("Modo Apuntar")]
    public Transform posicionCamara;

    private Rigidbody rb;
    float gatilloL, gatilloR;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rotacionYcam = transform.eulerAngles.y;
        rotacionXcam = transform.eulerAngles.x;

        Cursor.visible = false;
    }

    void Update()
    {
        Apuntar();

        Saltar();

        ResetearPosision();

        Gatillos();
    }

    private void MovimientoCamara()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotacionYcam += mouseX * sencibilidadCamara;
        rotacionXcam += mouseY * sencibilidadCamara;

        rotacionXcam = Mathf.Clamp(rotacionXcam, -89f, 89f);

        Quaternion rotacionCamara = Quaternion.Euler(rotacionXcam, rotacionYcam, 0f);
        Vector3 offset = rotacionCamara * new Vector3(0f, 0f, -distanciaCamara);

        transformCamara.position = transform.position + offset;
        transformCamara.LookAt(transform.position);
    }

    private void MovimientoJugador()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 direccionInput = new Vector3(h, 0f, v).normalized;

        Quaternion rotacionJugador = Quaternion.Euler(0f, rotacionYcam, 0f);
        Vector3 direccion = rotacionJugador * direccionInput;

        Vector3 velocidadDireccionada = direccion * velocidad;
        rb.velocity = new Vector3(velocidadDireccionada.x, rb.velocity.y, velocidadDireccionada.z);

        if (direccionInput.sqrMagnitude > 0.01f)
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
        if (Input.GetButton("Fire2") || gatilloL > 0)
        {
            transformCamara.position = posicionCamara.position;
            Disparar();
        }
        else
        {
            MovimientoJugador();
            MovimientoCamara();
        }
    }

    public void Disparar()
    {
        if (gatilloR > 0)
        {
            print("Fuego");
        }
    }

    public void Gatillos()
    {
        if (Gamepad.current != null)
        {
            var gamepad = Gamepad.current;
            gatilloL = gamepad.leftTrigger.ReadValue();   // 0..1
            gatilloR = gamepad.rightTrigger.ReadValue();
        }
        else
        {
            gatilloL = 0;
            gatilloR = 0;
        }
    }
}
