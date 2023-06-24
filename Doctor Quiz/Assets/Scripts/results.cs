using UnityEngine;
using UnityEngine.UI;

public class results : MonoBehaviour
{
    public Text questionsCorrectText;
    public Text samples;

    void Start()
    {
        int questionsCorrect = responder.correctQuestions;
        questionsCorrectText.text = questionsCorrect.ToString();
        samples.text = (questionsCorrect * 40).ToString();
    }
}
