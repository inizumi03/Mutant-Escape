using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampa : MonoBehaviour
{
    [Header("Valores")]
    public int daño;
    public int indice;
    public float intervalo;

    public delegate void EfectoDeTrampa(int daño, int indice);
    static public event EfectoDeTrampa Activar;

    float temp;

    private void Awake()
    {
        temp = intervalo;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Jugador") && indice != 2)
        {
            Activar.Invoke(daño, indice);
            if (indice != 1)
                gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        print("1");
        if (other.gameObject.CompareTag("Jugador") && indice == 2)
        {
            print("2");

            temp -= Time.deltaTime;

            if (temp < 0)
            {
                temp = intervalo;
                Activar.Invoke(daño, 0);
            }
        }
    }
}
