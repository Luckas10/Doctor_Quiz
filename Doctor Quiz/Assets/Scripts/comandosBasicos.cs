using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class comandosBasicos : MonoBehaviour
{
    private string questionType;
    public Button buttonConfirmar;
    public Toggle CheckBox;

    public void Start()
    {
        try
        {
            buttonConfirmar.interactable = false;
        }
        catch
        {
            
        }
    }


    public void buttonPressionado(string textButton)
    {
        buttonConfirmar.interactable = true;
        questionType = textButton;
    }

    public void GetQuestionType(string cena)
    {
        buttonConfirmar.interactable = true;
        PlayerPrefs.SetString("QuestionType", questionType); // Salva a variável questionType
        Debug.Log(questionType);
        SceneManager.LoadScene(cena);
    }

    public void LoadScene(string cena)
    {
        SceneManager.LoadScene(cena);
    }

    public void ButtonTerms(Button buttonCadastrar)
    {
        if (CheckBox.isOn)
        {
            buttonCadastrar.interactable = true;
        }
        else
        {
            buttonCadastrar.interactable = false;
        }
    }
}
