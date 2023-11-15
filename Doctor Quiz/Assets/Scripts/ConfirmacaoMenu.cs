using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmacaoMenu : MonoBehaviour
{

    public GameObject PausePanel;
    public GameObject ConfirmacaPanel;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Exit()
    {
        PausePanel.SetActive(false);
        ConfirmacaPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Sim(string cena)
    {
        SceneManager.LoadScene(cena);
    }

    public void Nao()
    {
        ConfirmacaPanel.SetActive(false);
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

}
