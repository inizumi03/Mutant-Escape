using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alcantarilla : MonoBehaviour
{
    [Header("Extras")]
    public GameObject mutageno;
    public GameObject GanarCanvas;

    private void OnTriggerEnter(Collider other)
    {
        if (!mutageno.activeSelf && other.gameObject.CompareTag("Jugador"))
        {
            Time.timeScale = 0f;
            GanarCanvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
