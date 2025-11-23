using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ComportamientoEnemigo : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad;
    public float sampleMaxDistance = 1;
    public float intervaloDeBusqueda = 5;
    public float distanciaDeBusqueda = 10;

    [Header("Disparar")]
    public GameObject bala;
    public Transform mira;
    public int limiteMaximoBalas;
    public float cadencia;
    public float interbaloDisparo;
    public float desvio;
    public float correcionDeDesbio;
    public GameObject arma;
    int balasDisparadas;
    float tiempoEspera;

    [Header("Extras")]
    public int vida;
    public Transform jugador;
    public GameObject forma;
    public SphereCollider trigger;
    public float distanciaDetencion;

    [Header("Animacion")]
    public Animator animator;
    public string aniJugadorDetectado;

    NavMeshAgent agent;
    float timer;
    static bool jugadorDetectado;
    int vidaActual;

    public delegate void CargarceEnespera(GameObject enemigo);
    static public event CargarceEnespera enviar;

    private void OnEnable()
    {
        vidaActual = vida;
    }

    private void OnDisable()
    {
        enviar.Invoke(gameObject);
    }

    private void Awake()
    {
        jugadorDetectado = false;

        agent = GetComponent<NavMeshAgent>();

        agent.speed = velocidad;

        timer = 1 * Random.Range(0.0f, 1.0f);

        vidaActual = vida;

        arma.SetActive(false);
    }

    private void Update()
    {
        arma.SetActive(jugadorDetectado);
        timer -= Time.deltaTime;

        CorrejirDesvio();

        if (jugadorDetectado)
        {
            PerseguirJugador();

            if (agent.stoppingDistance > 0)
            {
                tiempoEspera -= Time.deltaTime;

                if (tiempoEspera < 0)
                    Disparar();
            }
            else
            {
                tiempoEspera = Random.Range(interbaloDisparo - .5f, interbaloDisparo + .5f);
                balasDisparadas = 0;
            }
        }
        else
        {
            if (timer <= 0f)
            {
                Vector3 target;
                if (MovimientoAleatorio(transform.position, distanciaDeBusqueda, out target))
                {
                    agent.SetDestination(target);
                }
            }
        }

        Desactivar();
        ControlDeAnimacion();

        if (jugadorDetectado)
        {
            trigger.enabled = false;
        }
        else
        {
            trigger.enabled = true;
        }
    }

    bool MovimientoAleatorio(Vector3 center, float radius, out Vector3 result)
    {
        Vector3 randomDir = Random.insideUnitSphere * radius;
        Vector3 candidate = center + randomDir;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidate, out hit, sampleMaxDistance, NavMesh.AllAreas))
        {
            timer = intervaloDeBusqueda;
            result = hit.position;
            return true;
        }

        result = center;
        return false;
    }

    void PerseguirJugador()
    {
        int mask = LayerMask.GetMask("Default", "Jugador");
        Vector3 origen = transform.position + Vector3.up;
        Vector3 direccion = (jugador.position - origen).normalized;
        RaycastHit hit;
        bool golpeo = Physics.SphereCast(origen + transform.forward, .25f, direccion, out hit, distanciaDetencion - 1, mask, QueryTriggerInteraction.Ignore);

        if (golpeo)
        {
            if (hit.collider.CompareTag("Jugador"))
                agent.stoppingDistance = distanciaDetencion;
            else
                agent.stoppingDistance = 0;
        }
        else
            agent.stoppingDistance = 0;

        agent.SetDestination(jugador.position);
        forma.transform.LookAt(new Vector3(jugador.position.x, transform.position.y, jugador.position.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Jugador"))
        {
            jugadorDetectado = true;
        }
        if (other.gameObject.CompareTag("Bala"))
            vidaActual--;
    }

    void Disparar()
    {
        mira.LookAt(PuntoOvjetivo());

        Instantiate(bala, mira.position, mira.rotation);
        balasDisparadas += 1 + Random.Range(0, 4);

        if (balasDisparadas >= limiteMaximoBalas)
        {
            tiempoEspera = Random.Range(.5f, 1.5f);
            balasDisparadas = 0;
        }
        else
            tiempoEspera = cadencia;
    }

    void Desactivar()
    {
        if (vidaActual <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    Vector3 PuntoOvjetivo()
    {
        Vector3 pocicion = jugador.position;

        pocicion += (Vector3.right * Random.Range(-desvio, desvio)) + (Vector3.forward * Random.Range(-desvio, desvio));

        return pocicion;
    }

    void ControlDeAnimacion()
    {
        animator.SetBool(aniJugadorDetectado, jugadorDetectado);
    }

    void CorrejirDesvio()
    {
        if (desvio > 0)
            desvio -= Time.deltaTime * correcionDeDesbio;
        else
            desvio = 0;
    }
}
