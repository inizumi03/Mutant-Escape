using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Parametros")]
    public float velocidad, tiempoDeVida;

    private void Start()
    {
        Invoke("Destruir", tiempoDeVida);
    }

    private void Update()
    {
        transform.position += transform.forward * velocidad * Time.deltaTime;
        Colicion();
    }

    void Destruir()
    {
        CancelInvoke("Destuir");
        Destroy(gameObject);
    }

    void Colicion()
    {
        if (Physics.Raycast(transform.position, transform.forward, transform.localScale.z / 2))
            Destruir();
    }
}
