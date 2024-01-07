using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

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
    public static int correctQuestions = 0;
    public static string tipoAtual;

    private int totalQuestionsOfType;
    private int id_usuario;
    private string respostaUsuario;

    void Start()
    {
        imageDirectoryPath = Application.dataPath;
        ConnectionDB(); // Chame a função ConnectionDB para configurar o caminho do banco de dados

        // Recupera o valor de "id_usuario" usando PlayerPrefs
        id_usuario = PlayerPrefs.GetInt("id_usuario", -1);
        // Recupera a variável "questionType" usando PlayerPrefs
        tipoAtual = PlayerPrefs.GetString("QuestionType", "");

        Debug.Log(id_usuario);

        // Verifica se o valor foi salvo corretamente
        if (id_usuario != -1)
        {
            LoadQuestionsForUser(id_usuario);
            totalQuestionsOfType = GetTipoQuestionsCount(id_usuario, tipoAtual);
            if (totalQuestionsOfType > 0)
            {
                SetQuestion(currentQuestionIndex);
            }
            else
            {
                Debug.Log("Não há questões disponíveis para este usuário.");
            }
        }
        else
        {
            Debug.Log("Erro ao obter o valor de id_usuario de PlayerPrefs.");
        }
    }

    void ConnectionDB()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            pathToDB = Application.dataPath + "/StreamingAssets/" + DataBaseName;
        }
        else
        {
            pathToDB = Application.persistentDataPath + "/" + DataBaseName;

            if (!File.Exists(pathToDB))
            {
                WWW load = new WWW("jar:file://" + Application.dataPath + "!/assets/" + DataBaseName);
                while (!load.isDone) { }

                File.WriteAllBytes(pathToDB, load.bytes);
            }
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
                    "LEFT JOIN questao_usuario qu ON q.id = qu.id_questao AND qu.id_usuario = @UserId " +
                    "WHERE q.tipo = @TipoAtual AND qu.id_questao IS NULL";

                dbCmd.Parameters.Add(new SqliteParameter("@UserId", userId));
                dbCmd.Parameters.Add(new SqliteParameter("@TipoAtual", tipoAtual));

                using (SqliteDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Question question = new Question
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            questionText = reader["enunciado"].ToString(),
                            tipo = tipoAtual,
                            questionImage = reader["caminho_imagem"].ToString(),
                            alternativa_a = reader["alternativa_a"].ToString(),
                            alternativa_b = reader["alternativa_b"].ToString(),
                            alternativa_c = reader["alternativa_c"].ToString(),
                            alternativa_d = reader["alternativa_d"].ToString(),
                            opcao_correta = reader["opcao_correta"].ToString()
                        };
                        questions.Add(question);

                        question.responded = reader["responded"] != DBNull.Value ?
                                          reader.GetInt32(reader.GetOrdinal("responded")) : 1;
                    }
                }
            }

            dbConnection.Close();
        }
    }

    int GetTipoQuestionsCount(int userId, string tipo)
    {
        int totalQuestionsOfType = 0;
        int answeredQuestionsOfType = 0;

        using (SqliteConnection dbConnection = new SqliteConnection("URI=file:" + pathToDB))
        {
            dbConnection.Open();

            using (SqliteCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "SELECT COUNT(*) FROM questoes WHERE tipo = @Tipo";
                dbCmd.Parameters.Add(new SqliteParameter("@Tipo", tipo));

                totalQuestionsOfType = Convert.ToInt32(dbCmd.ExecuteScalar());

                dbCmd.CommandText = "SELECT COUNT(*) FROM questao_usuario qu " +
                                    "INNER JOIN questoes q ON qu.id_questao = q.id " +
                                    "WHERE qu.id_usuario = @UserId AND q.tipo = @Tipo";
                dbCmd.Parameters.Add(new SqliteParameter("@UserId", userId));

                answeredQuestionsOfType = Convert.ToInt32(dbCmd.ExecuteScalar());
            }

            dbConnection.Close();
        }

        txtProgress.text = answeredQuestionsOfType.ToString() + " / " + totalQuestionsOfType.ToString();

        float progress = (float)answeredQuestionsOfType / totalQuestionsOfType;
        progressLevel.value = progress;

        return totalQuestionsOfType;
    }

    void SetQuestion(int questionIndex)
    {
        if (questionIndex >= 0 && questionIndex < totalQuestionsOfType)
        {
            currentQuestionIndex = questionIndex;
            Question currentQuestion = questions[currentQuestionIndex];

            GetTipoQuestionsCount(id_usuario, tipoAtual);

            questionText.text = currentQuestion.questionText;

            if (!string.IsNullOrEmpty(currentQuestion.questionImage))
            {
                LoadImage(currentQuestion.questionImage);
            }
            else
            {
                questionImage.gameObject.SetActive(false);
            }

            optionButtons[0].GetComponentInChildren<Text>().text = currentQuestion.alternativa_a;
            optionButtons[1].GetComponentInChildren<Text>().text = currentQuestion.alternativa_b;
            optionButtons[2].GetComponentInChildren<Text>().text = currentQuestion.alternativa_c;
            optionButtons[3].GetComponentInChildren<Text>().text = currentQuestion.alternativa_d;

            confirmButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(() =>
            {
                ConfirmAnswer(currentQuestionIndex);
            });

            confirmButton.interactable = false;
        }
        else
        {
            Debug.Log("Todas as questões foram respondidas-SetQuestion.");
        }
    }

    public void ButtonInteractable(Text textResposta)
    {
        respostaUsuario = textResposta.text;
        confirmButton.interactable = true;
    }


    private void LoadImage(string imageName)
    {
        string imagePath = "";

        if (Application.platform != RuntimePlatform.Android)
        {
            imagePath = Application.dataPath + "/StreamingAssets" + imageName;
            if (File.Exists(imagePath))
            {
                byte[] imageData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                questionImage.gameObject.SetActive(true);

            }
            else
            {
                questionImage.gameObject.SetActive(false);
                Debug.LogError("Arquivo da imagem não encontrado: " + imagePath);
            }
        }
        else
        {
            imagePath = Application.streamingAssetsPath + imageName;

            // Se for Android, usa UnityWebRequest para carregar o arquivo
            UnityWebRequest www = UnityWebRequest.Get(imagePath);
            www.SendWebRequest();

            while (!www.isDone) { }

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Carrega a imagem
                byte[] imageData = www.downloadHandler.data;
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            else
            {
                Debug.LogError("Erro ao carregar imagem: " + www.error);
            }
        }
    }

    void ConfirmAnswer(int questionIndex)
    {
        id_usuario = PlayerPrefs.GetInt("id_usuario", -1);
        if (questionIndex >= 0 && questionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionIndex];

            if (respostaUsuario == currentQuestion.opcao_correta)
            {
                Debug.Log("Resposta correta!");
                correctQuestions++;

                InsertIntoQuestaoUsuario(currentQuestion.id, id_usuario, 1);
            }
            else
            {
                Debug.Log("Resposta incorreta!");
                InsertIntoQuestaoUsuario(currentQuestion.id, id_usuario, 1);
            }

            questionsAnswered++;
            valueProgressLevel = (float)questionsAnswered / totalQuestionsOfType;

            int nextUnansweredQuestionIndex = questionIndex + 1;

            Debug.Log(nextUnansweredQuestionIndex + "/" + questions.Count);

            if (nextUnansweredQuestionIndex < questions.Count)
            {
                SetQuestion(nextUnansweredQuestionIndex);
            }
            else
            {
                Debug.Log("Todas as questões foram respondidas: " + nextUnansweredQuestionIndex + "/" + questions.Count);
                Debug.Log("Questões corretas: " + correctQuestions + "/" + totalQuestionsOfType);
                SceneManager.LoadScene("Results");
            }

            while (nextUnansweredQuestionIndex < questions.Count &&
                questions[nextUnansweredQuestionIndex].responded > 0)
            {
                nextUnansweredQuestionIndex++;
            }
        }
    }

    void InsertIntoQuestaoUsuario(int questaoId, int userId, int responded)
    {
        using (SqliteConnection dbConnection = new SqliteConnection("URI=file:" + pathToDB))
        {
            dbConnection.Open();

            using (SqliteCommand dbCmd = dbConnection.CreateCommand())
            {
                dbCmd.CommandText = "INSERT INTO questao_usuario (id_questao, id_usuario, responded) " +
                    "VALUES (@QuestaoId, @UserId, @Responded)";
                dbCmd.Parameters.Add(new SqliteParameter("@QuestaoId", questaoId));
                dbCmd.Parameters.Add(new SqliteParameter("@UserId", userId));
                dbCmd.Parameters.Add(new SqliteParameter("@Responded", responded));

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
        public string tipo;
        public string questionImage;
        public string alternativa_a;
        public string alternativa_b;
        public string alternativa_c;
        public string alternativa_d;
        public string opcao_correta;
        public int responded;
    }
}
