using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class comandosBasicos : MonoBehaviour
{
    private Button  buttonPressed;
    public string   questionType;
    public Button   buttonConfirmar;

    public void Start()
    {
        try {
            buttonConfirmar.interactable = false;
        }
        catch {
            Debug.Log("Botão não instanciado");
        }
    }

    
    public void buttonPressionado(Button button)
    {
        buttonConfirmar.interactable = true;
        buttonPressed = button;
    }


    public void LoadScenes(string cena)
    {
        SceneManager.LoadScene(cena);
    }

    public void GetQuestionType()
    {
        if (buttonPressed != null)
        {
            questionType = buttonPressed.GetComponentInChildren<Text>().text;
            Debug.Log(questionType);
        }
    }
}
