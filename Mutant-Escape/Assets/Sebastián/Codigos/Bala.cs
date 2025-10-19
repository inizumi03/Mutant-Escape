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
    }

    private void OnTriggerEnter(Collider other)
    {
        Destruir();
    }

    private void OnTriggerStay(Collider other)
    {
        Destruir();
    }

    void Destruir()
    {
        print("Hola");
        CancelInvoke("Destuir");
        Destroy(gameObject);
    }
}
