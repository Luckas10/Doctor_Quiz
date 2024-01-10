using UnityEngine;
using UnityEngine.UI;

public class results : MonoBehaviour
{
    public Text questionsCorrectText;
    public Text samples;
    static private int questionsCorrect;

    void Start()
    {
        questionsCorrect = responder.correctQuestions;
        questionsCorrectText.text = questionsCorrect.ToString();
        samples.text = (questionsCorrect * 40).ToString();

        pontuacao.DataBaseAddAmostras("ecg_app", questionsCorrect * 40);
    }
}
