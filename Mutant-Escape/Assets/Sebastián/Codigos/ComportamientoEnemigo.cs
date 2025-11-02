using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ComportamientoEnemigo : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad;
    public float sampleMaxDistance = 1;
    public float intervaloDeBusqueda = 5;
    public float distanciaDeBusqueda = 10;
    public float distanciaDetencion;

    [Header("Extras")]
    public Transform jugador;
    public GameObject forma;
    public SphereCollider trigger;

    NavMeshAgent agent;
    float timer;
    static bool jugadorDetectado;

    private void Awake()
    {
        jugadorDetectado = false;

        agent = GetComponent<NavMeshAgent>();

        agent.speed = velocidad;

        timer = 1 * Random.Range(0.0f, 1.0f);
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (jugadorDetectado)
        {
            PerseguirJugador();
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
        bool golpeo = Physics.Raycast(origen + transform.forward, direccion, out hit, distanciaDetencion - 1, mask, QueryTriggerInteraction.Ignore);

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
        forma.transform.LookAt(jugador);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Jugador"))
        {
            trigger.enabled = false;
            jugadorDetectado = true;
        }
    }
}
