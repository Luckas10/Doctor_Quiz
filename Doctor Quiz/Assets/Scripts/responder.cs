using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using System.Data;

public class responder : MonoBehaviour
{
    public Text questionText;
    public Image questionImage;
    public List<Button> optionButtons;
    public Button confirmButton;
    public Text txtProgress;
    public Slider progressLevel;
    public string DataBaseName;

    private static float valueProgressLevel = 0.0f;
    private List<Question> questions;
    private int currentQuestionIndex = 0;

    private string pathToDB;
    private string imageDirectoryPath;
    private int questionsAnswered = 0;
    public static int correctQuestions = 0; // Variável para contar as respostas corretas

    void Start()
    {
        imageDirectoryPath = Application.dataPath;
        pathToDB = Application.dataPath + "/" + DataBaseName;

        LoadQuestionsForUser(1); // Carregue as questões para o usuário com ID 1

        if (questions.Count > 0)
        {
            SetQuestion(currentQuestionIndex);
        }
        else
        {
            Debug.Log("Não há questões disponíveis para este usuário.");
        }
    }

    void LoadQuestionsForUser(int userId)
    {
        questions = new List<Question>();

        using (SqliteConnection dbConnection = new SqliteConnection("URI=file:" + pathToDB))
        {
            dbConnection.Open();

            using (SqliteCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT q.*, qu.id_questao as responded " +
                                    "FROM questoes q " +
                                    "LEFT JOIN questao_usuario qu ON q.id = qu.id_questao AND qu.id_usuario = @UserId";

                dbCmd.Parameters.Add(new SqliteParameter("@UserId", userId));

                using (SqliteDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Question question = new Question
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            questionText = reader["enunciado"].ToString(),
                            questionImage = reader["caminho_imagem"].ToString(), // Obtém o caminho da imagem da tabela questoes
                            alternativa_a = reader["alternativa_a"].ToString(),
                            alternativa_b = reader["alternativa_b"].ToString(),
                            alternativa_c = reader["alternativa_c"].ToString(),
                            alternativa_d = reader["alternativa_d"].ToString(),
                            opcao_correta = reader["opcao_correta"].ToString()
                        };
                        questions.Add(question);

                        // Verifique se a questão já foi respondida pelo usuário
                        int responded = reader["responded"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("responded")) : 0;
                        if (responded > 0)
                        {
                            questionsAnswered++;
                        }
                    }
                }
            }

            dbConnection.Close();
        }
    }

    void SetQuestion(int questionIndex)
    {
        if (questionIndex >= 0 && questionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionIndex];

            txtProgress.text = (questionsAnswered + 1).ToString() + " / " + questions.Count.ToString();
            progressLevel.value = valueProgressLevel;

            // Exiba a pergunta e imagem
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

            // Defina o texto dos botões de opção
            optionButtons[0].GetComponentInChildren<Text>().text = currentQuestion.alternativa_a;
            optionButtons[1].GetComponentInChildren<Text>().text = currentQuestion.alternativa_b;
            optionButtons[2].GetComponentInChildren<Text>().text = currentQuestion.alternativa_c;
            optionButtons[3].GetComponentInChildren<Text>().text = currentQuestion.alternativa_d;

            // Remova todos os ouvintes do botão de confirmação
            confirmButton.onClick.RemoveAllListeners();

            // Adicione um único ouvinte que chama a função ConfirmAnswer() com o índice da opção selecionada
            confirmButton.onClick.AddListener(() =>
            {
                ConfirmAnswer(currentQuestionIndex);
            });

            // Desabilite o botão de confirmação até que uma opção seja selecionada
            confirmButton.interactable = false;
        }
        else
        {
            Debug.Log("Todas as questões foram respondidas.");
            // Trate o caso em que todas as questões já foram respondidas.
        }
    }

    private void LoadImage(string imageName)
    {
        string imagePath = Path.Combine(imageDirectoryPath, imageName);

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

    void ConfirmAnswer(int questionIndex)
    {
        if (questionIndex >= 0 && questionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionIndex];

            if (optionButtons[0].GetComponentInChildren<Text>().text == currentQuestion.opcao_correta)
            {
                Debug.Log("Resposta correta!");
                correctQuestions++; // Incrementa a contagem de respostas corretas

                // Inserir na tabela questao_usuario apenas se a resposta estiver correta
                InsertIntoQuestaoUsuario(currentQuestion.id, 1); // Substitua 1 pelo ID do usuário

            }
            else
            {
                Debug.Log("Resposta incorreta!");
                // Lógica para tratar a resposta incorreta
            }

            questionsAnswered++;
            valueProgressLevel = (float)questionsAnswered / questions.Count;
            Debug.Log("questões respondidas ConfirmAnswer: " + questionsAnswered);

            currentQuestionIndex++;

            if (currentQuestionIndex < questions.Count)
            {
                SetQuestion(currentQuestionIndex);
            }
            else
            {
                Debug.Log("Todas as questões foram respondidas.");
                Debug.Log("Respostas corretas: " + correctQuestions); // Exibe o número de respostas corretas
                // Trate o caso em que todas as questões já foram respondidas.
            }
        }
    }

    void InsertIntoQuestaoUsuario(int questaoId, int userId)
    {
        using (SqliteConnection dbConnection = new SqliteConnection("URI=file:" + pathToDB))
        {
            dbConnection.Open();

            using (SqliteCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "INSERT INTO questao_usuario (id_questao, id_usuario) VALUES (@QuestaoId, @UserId)";
                dbCmd.Parameters.Add(new SqliteParameter("@QuestaoId", questaoId));
                dbCmd.Parameters.Add(new SqliteParameter("@UserId", userId));

                int rowsAffected = dbCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Debug.Log("Inserção na tabela questao_usuario bem-sucedida.");
                }
                else
                {
                    Debug.LogError("Falha ao inserir na tabela questao_usuario.");
                }
            }

            dbConnection.Close();
        }
    }

    [System.Serializable]
    public class Question
    {
        public int id;
        public string questionText;
        public string questionImage;
        public string alternativa_a;
        public string alternativa_b;
        public string alternativa_c;
        public string alternativa_d;
        public string opcao_correta;
    }
}
