using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampa : MonoBehaviour
{
    [Header("Valores")]
    public int daño;
    public int indice;

    public delegate void EfectoDeTrampa(int daño, int indice);
    static public event EfectoDeTrampa Activar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Jugador"))
        {
            Activar.Invoke(daño, indice);
            gameObject.SetActive(false);
        }
    }
}
