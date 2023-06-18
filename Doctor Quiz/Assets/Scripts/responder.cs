using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class responder : MonoBehaviour
{
    public Text questionText;
    public Image questionImage;
    public List<Button> optionButtons;
    public Button confirmButton;

    private List<Question> questions;
    private Question currentQuestion;

    private string path;
    private string imageDirectoryPath;
    private int questionsAnswered = 0;
    private List<Question> questionsShown = new List<Question>();

    void Start()
    {
        imageDirectoryPath = Path.Combine(Application.dataPath);
        path = Path.Combine(Application.dataPath, "Scripts", "DoctorQuiz", "novas_perguntas.json");
        LoadQuestions();
        SetQuestion();
    }

    
    void LoadQuestions()
    {
        string json = File.ReadAllText(path);
        QuestionList questionList = JsonUtility.FromJson<QuestionList>(json);
        questions = questionList.questions.ToList();
        foreach (Question question in questions)
    {
        Debug.Log("Question: " + question.questionText);
        Debug.Log("Image: " + question.questionImage);
        Debug.Log("Options: " + string.Join(",", question.options));
        Debug.Log("Correct Option Index: " + question.correctOptionIndex);
    }

    }

    private void LoadImage(string imageName)
    {           string imagePath = Path.Combine(imageDirectoryPath, imageName);
        if (File.Exists(imagePath))
        {               byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {               Debug.LogError("Arquivo da imagem não encontrado: " + imagePath);
        }
    }


    void SetQuestion()
    {
        if (questionsShown.Count < questions.Count)
        {
            List<Question> remainingQuestions = questions.Except(questionsShown).ToList();
            int randomIndex = Random.Range(0, remainingQuestions.Count);
            currentQuestion = remainingQuestions[randomIndex];
            questionsShown.Add(currentQuestion);
            questionText.text = currentQuestion.questionText;
            if (!string.IsNullOrEmpty(currentQuestion.questionImage))
            {
                LoadImage(currentQuestion.questionImage);
                questionImage.gameObject.SetActive(true);
            }
            else
            {
                questionImage.gameObject.SetActive(false);
            }
            List<string> options = currentQuestion.options.ToList();
            options = options.OrderBy(option => Random.value).ToList();
            for (int i = 0; i < optionButtons.Count; i++)
            {
                optionButtons[i].GetComponentInChildren<Text>().text = options[i];
                int index = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => ConfirmAnswer(index));
            }
            confirmButton.gameObject.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("Results");
        }
    }


    void ConfirmAnswer(int index)
    {
        if (index == currentQuestion.correctOptionIndex)
        {
            Debug.Log("Resposta correta!");
        }
        else
        {
            Debug.Log("Resposta incorreta!");
        }
        questionsAnswered++;
        if (questionsAnswered == questions.Count)
        {
            SceneManager.LoadScene("Results");
        }
        else
        {
            SetQuestion();
        }
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string questionImage;
        public string[] options;
        public int correctOptionIndex;
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] questions;
    }
}
