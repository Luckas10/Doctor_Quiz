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

public class readQuestions : MonoBehaviour
{

    public Button buttonIniciante;
    public Button buttonIntermediario;
    public Button buttonAvancado;
    public Button buttonExpert;
    public string DataBaseName;

    private List<Question> questions;

    private string pathToDB;

    private string verificarQuestions;
    private int id_usuario;


    void Start()
    {
        ConnectionDB(); // Chame a função ConnectionDB para configurar o caminho do banco de dados

        // Recupera o valor de "id_usuario" usando PlayerPrefs
        id_usuario = PlayerPrefs.GetInt("id_usuario", -1);

        Debug.Log(id_usuario);

        // Verifica se o valor foi salvo corretamente
        if (id_usuario != -1)
        {

            try {
                LoadQuestionsForUser(id_usuario, "iniciante");
                verificarQuestions = GetTipoQuestionsCount(id_usuario, "iniciante");
                Debug.Log(verificarQuestions);
                if (verificarQuestions == "respondido")
                {
                    buttonIniciante.interactable = false;
                }
                else {
                    buttonIniciante.interactable = true;
                }
            } catch {

            }

            try {
                LoadQuestionsForUser(id_usuario, "intermediário");
                verificarQuestions = GetTipoQuestionsCount(id_usuario, "intermediário");
                if (verificarQuestions == "respondido")
                {
                    buttonIntermediario.interactable = false;
                }
            } catch {
                buttonIntermediario.interactable = true;
            }

            try {
                LoadQuestionsForUser(id_usuario, "avançado");
                verificarQuestions = GetTipoQuestionsCount(id_usuario, "avançado");
                if (verificarQuestions == "respondido")
                {
                    buttonAvancado.interactable = false;
                }
            } catch {
                buttonAvancado.interactable = true;
            }

            try {
                LoadQuestionsForUser(id_usuario, "expert");
                verificarQuestions = GetTipoQuestionsCount(id_usuario, "expert");
                if (verificarQuestions == "respondido")
                {
                    buttonExpert.interactable = false;
                }
            } catch {
                buttonExpert.interactable = true;
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

    void LoadQuestionsForUser(int userId, string tipoAtual)
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

    string GetTipoQuestionsCount(int userId, string tipo)
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

        if (totalQuestionsOfType == answeredQuestionsOfType) {
            return "respondido";
        } else {
            return "não respondido";
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
