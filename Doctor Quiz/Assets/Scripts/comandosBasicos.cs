using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class comandosBasicos : MonoBehaviour
{
    private string   questionType;
    public Button   buttonConfirmar;

    public void Start()
    {
        buttonConfirmar.interactable = false;
    }

    
    public void buttonPressionado(string textButton)
    {
        buttonConfirmar.interactable = true;
        questionType = textButton;
    }

    public void GetQuestionType(string cena)
    {
        buttonConfirmar.interactable = true;
        Debug.Log(questionType);
        SceneManager.LoadScene(cena);
    }
}
