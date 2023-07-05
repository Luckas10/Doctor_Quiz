using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using SQLite4Unity3d;

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

    private SQLiteConnection dbConnection;

    private static int questionsAnswered = 0;
    private List<Question> questionsShown = new List<Question>();

    private DatabaseManager databaseManager;

    private void Start()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, "MyDatabase.sqlite");
        dbConnection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        dbConnection.CreateTable<Question>();

        LoadQuestionsFromDatabase();
        SetQuestion();
        Debug.Log("Questões respondidas Start: " + questionsAnswered);
    }

    private void LoadQuestionsFromDatabase()
    {
        // Verifica se o banco de dados já contém questões
        List<Question> existingQuestions = dbConnection.Table<Question>().ToList();
        if (existingQuestions.Count > 0)
        {
            // Se já existirem questões no banco de dados, não é necessário inserir novamente
            questions = existingQuestions;
            return;
        }

        // Inserção das questões no banco de dados
        List<Question> questionsToAdd = new List<Question>
        {
            // new Question
            // {
            //     questionText = "O que significa o segmento ST em um ECG?",
            //     questionImage = "Images/img_Enunciado.png",
            //     options = "Segmento de Taquicardia,Segmento de Tendência,Segmento de Tristeza,Segmento de ST",
            //     correctOptionText = "Segmento de ST"
            // },
            // new Question
            // {
                
            //     questionText = "Qual é a principal alteração do ECG associada à isquemia cardíaca?",
            //     questionImage = "Images/img_enunciado2.png",
            //     options = "Elevação do segmento ST,Aumento da amplitude da onda P,Diminuição do intervalo PR,Aumento da duração do complexo QRS",
            //     correctOptionText = "Elevação do segmento ST"
            // },
            // new Question
            // {
            //     questionText = "O que indica um intervalo PR prolongado no ECG?",
            //     questionImage = "Images/img_enunciado3.png",
            //     options = "Bloqueio do ramo direito,Bloqueio do ramo esquerdo,Bloqueio atrioventricular de primeiro grau,Bloqueio atrioventricular de segundo grau",
            //     correctOptionText = "Bloqueio atrioventricular de primeiro grau"
            // },
            // new Question
            // {
            //     questionText = "O que o gráfico desse ECG indica?",
            //     questionImage = "Images/img_enunciado4.png",
            //     options = "Taquicardia supraventricular,Ritmo sinusal normal,Taquicardia Ventricular,Fibrilação atrial",
            //     correctOptionText = "Fibrilação atrial"
            // }
        };

        foreach (Question question in questionsToAdd)
        {
            dbConnection.Insert(question);
        }

        questions = dbConnection.Table<Question>().ToList();
    }

    private void LoadImage(string imageName)
    {
        string imagePath = Path.Combine(Application.dataPath, imageName);
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            Debug.LogError("Arquivo da imagem não encontrado: " + imagePath);
        }
    }


    private void SetQuestion()
    {
        txtProgress.text = (questionsAnswered + 1).ToString() + " / " + questions.Count.ToString();
        progressLevel.value = valueProgressLevel;
        if (questionsShown.Count < questions.Count)
        {
            currentQuestion = questions[questionsAnswered];
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

            List<string> options = currentQuestion.options.Split(',').ToList();
            options = options.OrderBy(option => Random.value).ToList();
            int selectedOptionIndex = -1;

            for (int i = 0; i < optionButtons.Count; i++)
            {
                int optionIndex = i;
                optionButtons[optionIndex].GetComponentInChildren<Text>().text = options[optionIndex];

                optionButtons[optionIndex].onClick.RemoveAllListeners();

                optionButtons[optionIndex].onClick.AddListener(() =>
                {
                    selectedOptionIndex = optionIndex;
                    confirmButton.interactable = true;
                });
            }

            confirmButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(() =>
            {
                ConfirmAnswer(selectedOptionIndex, options[selectedOptionIndex]);
            });

            confirmButton.interactable = false;
        }
    }


    private void ConfirmAnswer(int index, string selectedOption)
    {
        if (index != -1)
        {
            if (selectedOption == currentQuestion.correctOptionText)
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
            Debug.Log("Questões respondidas ConfirmAnswer: " + questionsAnswered);

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
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionImage { get; set; }
        public string options { get; set; }
        public string correctOptionText { get; set; }
    }
}
