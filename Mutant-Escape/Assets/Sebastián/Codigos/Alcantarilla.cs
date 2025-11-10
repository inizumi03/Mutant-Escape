using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alcantarilla : MonoBehaviour
{
    [Header("Extras")]
    public GameObject mutageno;

    private void OnTriggerEnter(Collider other)
    {
        if (!mutageno.activeSelf && other.gameObject.CompareTag("Jugador"))
        {
            //ganar
        }
    }
}
