using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Municion : MonoBehaviour
{
    [Header("Variables")]
    public float velocidadDeRotacion;
    public float tiempoDeReaparicion;

    private void Update()
    {
        transform.Rotate(Vector3.up * velocidadDeRotacion);
    }

    private void OnDisable()
    {
        Invoke("Reactivar", tiempoDeReaparicion);
    }

    void Reactivar()
    {
        gameObject.SetActive(true);
    }
}
