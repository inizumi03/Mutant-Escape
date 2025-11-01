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

    [Header("Extras")]
    public Transform jugador;

    NavMeshAgent agent;
    float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = velocidad;

        timer = 1 * Random.Range(0.0f, 1.0f);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Vector3 target;
            if (TryGetRandomPoint(transform.position, distanciaDeBusqueda, out target))
            {
                agent.SetDestination(target);
            }
        }
    }

    bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 result)
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

}
