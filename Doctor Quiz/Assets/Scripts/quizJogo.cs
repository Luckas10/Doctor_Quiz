using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class quizJogo : MonoBehaviour
{

    public Button   btnContinuar;

    private int     idTema;


    // Start is called before the first frame update
    public void Start()
    {
        btnContinuar.interactable = false;
    }

    
    public void selecioneTema()
    {
        btnContinuar.interactable = true;
    }
}
