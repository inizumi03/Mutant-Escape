using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutageno : MonoBehaviour
{
    [Header("Parametros")]
    public float tiepoDeReaparicion;

    private void OnDisable()
    {
        Invoke("Activar", tiepoDeReaparicion);
    }

    void Activar()
    {
        gameObject.SetActive(true);
    }
}
