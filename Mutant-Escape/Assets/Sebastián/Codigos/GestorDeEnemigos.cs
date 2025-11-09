using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorDeEnemigos : MonoBehaviour
{
    [Header("Componentes")]
    public List<GameObject> enemigos;
    public float intervalo;

    float temp;

    private void Awake()
    {
        ComportamientoEnemigo.enviar += CargarEnemigo;
        temp = intervalo;
    }

    private void Update()
    {
        LiverarEnemigo();
    }

    private void OnDestroy()
    {
        ComportamientoEnemigo.enviar -= CargarEnemigo;
    }

    void CargarEnemigo(GameObject enemigo)
    {
        enemigos.Add(enemigo);
    }

    void LiverarEnemigo()
    {
        if (enemigos.Count > 0)
        {
            temp -= Time.deltaTime;
            
            if (temp < 0)
            {
                enemigos[0].transform.position = gameObject.transform.position;

                enemigos[0].gameObject.SetActive(true);

                enemigos.RemoveAt(0);

                temp = intervalo;
            }
        }
    }
}
