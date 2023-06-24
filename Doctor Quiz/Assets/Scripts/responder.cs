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
    public Text txtProgress;
    public Slider progressLevel;
    
    private static float valueProgressLevel = 0.0f;
    private List<Question> questions;
    private Question currentQuestion;
    public static int correctQuestions;

    private string path;
    private string imageDirectoryPath;
    private static int questionsAnswered = 0;
    private List<Question> questionsShown = new List<Question>();

    void Start()
    {
        imageDirectoryPath = Path.Combine(Application.dataPath);
        path = Path.Combine(Application.dataPath, "Scripts", "DoctorQuiz", "novas_perguntas.json");
        LoadQuestions();
        SetQuestion();
        Debug.Log("questões respondidas Start: "+ questionsAnswered);
    }

    
    void LoadQuestions()
    {
        string json = File.ReadAllText(path);
        QuestionList questionList = JsonUtility.FromJson<QuestionList>(json);
        questions = questionList.questions.ToList();
        foreach (Question question in questions)
    {
        //Debug.Log("Question: " + question.questionText);
        //Debug.Log("Image: " + question.questionImage);
        //Debug.Log("Options: " + string.Join(",", question.options));
        //Debug.Log("Correct Option Index: " + question.correctOptionIndex);
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
        {
            Debug.LogError("Arquivo da imagem não encontrado: " + imagePath);
        }
    }


    void SetQuestion()
    {
        txtProgress.text = (questionsAnswered + 1).ToString() + " / " + questions.Count.ToString();
        progressLevel.value = valueProgressLevel;
        if (questionsShown.Count < questions.Count)
        {
            // Seleciona uma questão aleatória que ainda não foi mostrada
            List<Question> remainingQuestions = questions.Except(questionsShown).ToList();
            int randomIndex = Random.Range(0, remainingQuestions.Count);
            currentQuestion = remainingQuestions[randomIndex];
            questionsShown.Add(currentQuestion);

            // Exibe a pergunta e a imagem, se houver
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

            // Embaralha as opções e define o texto dos botões de opção
            List<string> options = currentQuestion.options.ToList();
            options = options.OrderBy(option => Random.value).ToList();
            int selectedOptionIndex = -1;

            for (int i = 0; i < optionButtons.Count; i++)
            {
                int optionIndex = i;
                optionButtons[optionIndex].GetComponentInChildren<Text>().text = options[optionIndex];

                // Remove todos os listeners do botão de opção
                optionButtons[optionIndex].onClick.RemoveAllListeners();

                // Adiciona um listener que armazena o índice da opção selecionada na variável selectedOptionIndex
                optionButtons[optionIndex].onClick.AddListener(() =>
                {
                    selectedOptionIndex = optionIndex;
                    confirmButton.interactable = true;
                });
            }


            // Remove todos os listeners do botão de confirmação
            confirmButton.onClick.RemoveAllListeners();

            // Adiciona um único listener que chama a função ConfirmAnswer() com o índice da opção selecionada
            confirmButton.onClick.AddListener(() =>
            {
                ConfirmAnswer(selectedOptionIndex, options[selectedOptionIndex]);
            });

            // Desabilita o botão de confirmação até que uma opção seja selecionada
            confirmButton.interactable = false;
        }
    }



    void ConfirmAnswer(int index, string textoSelected)
    {
        if (index != -1){
            if (textoSelected == currentQuestion.correctOptionIndex)
            {
                Debug.Log("Resposta correta!");
                correctQuestions++;
            }
            else
            {
                Debug.Log("Resposta incorreta!");
            }
            questionsAnswered++;
            valueProgressLevel += 1.0f / questions.Count;
            Debug.Log("questões respondidas ConfirmAnswer: "+ questionsAnswered);
            if (questionsAnswered == questions.Count)
            {
                SceneManager.LoadScene("Results");
                Debug.Log("Respostas corretas: " + correctQuestions);
            }
            else
            {
                SetQuestion();
            }
        }
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string questionImage;
        public string[] options;
        public string correctOptionIndex;
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] questions;
    }
}
