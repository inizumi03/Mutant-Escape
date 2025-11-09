using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Parametros")]
    public float velocidad, tiempoDeVida;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = transform.forward * velocidad;

        Invoke("Destruir", tiempoDeVida);
    }

    void Destruir()
    {
        CancelInvoke("Destuir");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destruir();
    }
}
