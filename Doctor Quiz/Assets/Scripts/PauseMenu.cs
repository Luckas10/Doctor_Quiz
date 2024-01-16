using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public GameObject PausePanel;
    public GameObject CorrecaoMenu;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AtivarCorrecaoMenu() {
        CorrecaoMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Pause()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

}
