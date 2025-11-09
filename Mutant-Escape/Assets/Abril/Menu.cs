using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject instruccionesPanel;
    public GameObject MenuPanel;
    public string sceneName;
    public void Jugar()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void VerInstrucciones()
    {
        MenuPanel.SetActive(false);
        instruccionesPanel.SetActive(true);
    }

    public void CerrarInstrucciones()
    {
        MenuPanel.SetActive(true);
        instruccionesPanel.SetActive(false);
    }

    public void Salir()
    {
        Application.Quit();
    }

}