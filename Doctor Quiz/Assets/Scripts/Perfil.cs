using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using UnityEngine.Networking;

public class Perfil : MonoBehaviour
{

    public Text textName;
    public string DataBaseName;

    private string pathToDB;

    void Start()
    {

        ConnectionDB();

        string conn = SetDataBaseClass.SetDataBase(DataBaseName);
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

        // Recupera o valor de "id_usuario" usando PlayerPrefs
        int id_usuario = PlayerPrefs.GetInt("id_usuario", -1);
        // Recupera a variável "DataBaseName" usando PlayerPrefs

        string SQlQuery = "SELECT nome FROM usuarios WHERE id_usuario = @id_usuario";
        dbcmd.Parameters.Add(new SqliteParameter("@id_usuario", id_usuario));

        dbcmd.CommandText = SQlQuery;
        reader = dbcmd.ExecuteReader();
        if (reader.Read())
        {
            string nome = reader.GetString(0);
            textName.text = nome;
        }
        reader.Close();

        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;

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

    public void ZerarQuiz()
    {
        int id_usuario = PlayerPrefs.GetInt("id_usuario", -1);

        if (id_usuario != -1)
        {
            using (SqliteConnection dbConnection = new SqliteConnection("URI=file:" + pathToDB))
            {
                dbConnection.Open();

                using (SqliteCommand dbCmd = dbConnection.CreateCommand())
                {
                    dbCmd.CommandText = "DELETE FROM questao_usuario WHERE id_usuario = @UserId";
                    dbCmd.Parameters.Add(new SqliteParameter("@UserId", id_usuario));

                    int rowsAffected = dbCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.Log("Exclusão na tabela questao_usuario bem-sucedida. Todas as colunas para o usuário foram removidas.");
                    }
                    else
                    {
                        Debug.LogWarning("Nenhuma coluna foi encontrada para o usuário na tabela questao_usuario.");
                    }
                }

                dbConnection.Close();
            }
        }
        else
        {
            Debug.LogError("Erro ao obter o valor de id_usuario de PlayerPrefs.");
        }
    }


}
