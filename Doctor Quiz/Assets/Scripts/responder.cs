using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class responder : MonoBehaviour
{
    public Text questionText;
    public List<Button> optionButtons;

    private List<Question> questions;
    private Question currentQuestion;

    private void Start()
    {
        LoadQuestionsFromJSON();
        ShuffleQuestions();
        DisplayQuestion();
    }

    private void LoadQuestionsFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "novas_perguntas.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            QuestionList questionList = JsonUtility.FromJson<QuestionList>(json);
            questions = questionList.questions;
        }
        else
        {
            Debug.LogError("Arquivo JSON de perguntas não encontrado: " + filePath);
        }
        
        ShuffleQuestions();
        DisplayQuestion();
    }


    private void ShuffleQuestions()
    {
        questions = questions.OrderBy(q => Random.Range(0, 1000)).ToList();

        foreach (Question question in questions)
        {
            question.options = question.options.OrderBy(o => Random.Range(0, 1000)).ToList();
        }
    }

    private void DisplayQuestion()
    {
        if (questions.Count > 0)
        {
            currentQuestion = questions[0];
            questionText.text = currentQuestion.questionText;

            for (int i = 0; i < optionButtons.Count; i++)
            {
                optionButtons[i].GetComponentInChildren<Text>().text = currentQuestion.options[i];
            }
        }
    }

    public void CheckAnswer(int optionIndex)
    {
        if (currentQuestion != null && optionIndex == currentQuestion.correctOptionIndex)
        {
            Debug.Log("Resposta correta!");
        }
        else
        {
            Debug.Log("Resposta incorreta!");
        }

        questions.RemoveAt(0); // Remover a pergunta atual da lista

        if (questions.Count > 0)
        {
            DisplayQuestion();
        }
        else
        {
            Debug.Log("Quiz concluído!");
        }
    }
}

[System.Serializable]
public class Question
{
    public string questionText;
    public List<string> options;
    public int correctOptionIndex;
}

[System.Serializable]
public class QuestionList
{
    public List<Question> questions;
}
