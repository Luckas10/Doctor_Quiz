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
    public static string tipoAtual = "iniciante"; // Armazena o tipo de questão atual

    private int totalQuestionsOfType;

    void Start()
    {
        imageDirectoryPath = Application.dataPath;
        pathToDB = Application.dataPath + "/StreamingAssets/" + DataBaseName;

        LoadQuestionsForUser(4); // Carregue as questões para o usuário com ID 2. Substitua o primeiro parametro pelo id do usuario atual
        totalQuestionsOfType = GetTipoQuestionsCount(4, tipoAtual); // USER ID SETADO COMO 1, mudei para 2. Substitua o primeiro parametro pelo id do usuario atual
        if (totalQuestionsOfType > 0)
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
                    "LEFT JOIN questao_usuario qu ON q.id = qu.id_questao AND qu.id_usuario = @UserId " +
                    "WHERE q.tipo = @TipoAtual AND qu.id_questao IS NULL";

                dbCmd.Parameters.Add(new SqliteParameter("@UserId", userId)); // Adicionar o parâmetro UserId
                dbCmd.Parameters.Add(new SqliteParameter("@TipoAtual", tipoAtual)); // Adicionar o parâmetro TipoAtual

                using (SqliteDataReader reader = dbCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Question question = new Question
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            questionText = reader["enunciado"].ToString(),
                            tipo = tipoAtual,
                            questionImage = reader["caminho_imagem"].ToString(), // Obtém o caminho da imagem da tabela questoes
                            alternativa_a = reader["alternativa_a"].ToString(),
                            alternativa_b = reader["alternativa_b"].ToString(),
                            alternativa_c = reader["alternativa_c"].ToString(),
                            alternativa_d = reader["alternativa_d"].ToString(),
                            opcao_correta = reader["opcao_correta"].ToString()
                        };
                        questions.Add(question);

                        // Verifique se a questão já foi respondida pelo usuário
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
                // Consulta para contar todas as questões do tipo especificado
                dbCmd.CommandText = "SELECT COUNT(*) FROM questoes WHERE tipo = @Tipo";
                dbCmd.Parameters.Add(new SqliteParameter("@Tipo", tipo));

                totalQuestionsOfType = Convert.ToInt32(dbCmd.ExecuteScalar());

                // Consulta para contar as questões desse tipo já respondidas pelo usuário
                dbCmd.CommandText = "SELECT COUNT(*) FROM questao_usuario qu " +
                                    "INNER JOIN questoes q ON qu.id_questao = q.id " +
                                    "WHERE qu.id_usuario = @UserId AND q.tipo = @Tipo";
                dbCmd.Parameters.Add(new SqliteParameter("@UserId", userId));

                answeredQuestionsOfType = Convert.ToInt32(dbCmd.ExecuteScalar());
            }

            dbConnection.Close();
        }

        // Calcular e retornar o progresso com base no tipo atual
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

            txtProgress.text = (questionsAnswered + 1).ToString() + " / " + totalQuestionsOfType.ToString();

            // Atualize o progressLevel com base no tipo atual
            GetTipoQuestionsCount(4, tipoAtual); // USER ID SETADO COMO 1, mudei para 2. Substitua o primeiro parametro pelo id do usuario atual

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
            Debug.Log("Todas as questões foram respondidas-SetQuestion.");
            // Trate o caso em que todas as questões já foram respondidas.
        }
    }

    private void LoadImage(string imageName)
    {
        string imagePath = imageDirectoryPath + imageName;

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

    void ConfirmAnswer(int questionIndex) {
        if (questionIndex >= 0 && questionIndex < questions.Count) {
            Question currentQuestion = questions[questionIndex];
            
            if (optionButtons[0].GetComponentInChildren<Text>().text == currentQuestion.opcao_correta) {
                Debug.Log("Resposta correta!");
                correctQuestions++;  
                
                InsertIntoQuestaoUsuario(currentQuestion.id, 4, 1); // Substitua o segundo parâmetro pelo id do usuário atual
                
            } else {
                Debug.Log("Resposta incorreta!");
                InsertIntoQuestaoUsuario(currentQuestion.id, 4, 1); // Substitua o segundo parâmetro pelo id do usuário atual
            }
            
            questionsAnswered++;
            valueProgressLevel = (float)questionsAnswered / totalQuestionsOfType;  
            
            int nextUnansweredQuestionIndex = questionIndex + 1;
            
            Debug.Log(nextUnansweredQuestionIndex + "/" + questions.Count);//Questão atual id / total de questões

            if (nextUnansweredQuestionIndex < questions.Count) {
                SetQuestion(nextUnansweredQuestionIndex);   
            } else {
                Debug.Log("Todas as questões foram respondidas: " + nextUnansweredQuestionIndex + "/" + questions.Count);
                Debug.Log("Questões corretas: " + correctQuestions + "/" + totalQuestionsOfType);
            }

            while (nextUnansweredQuestionIndex < questions.Count &&  
                questions[nextUnansweredQuestionIndex].responded > 0) {
                nextUnansweredQuestionIndex++;    
            }
        }
    }

    void InsertIntoQuestaoUsuario(int questaoId, int userId, int responded) {
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
