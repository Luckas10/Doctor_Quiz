using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class pontuacao : MonoBehaviour
{

    public string DataBaseName;
    public bool Verificador;
    public Text Amostras;

    void Start()
    {
        if (Verificador == true) {
            ExibirAmostras();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void AddAmostras(string NameDB)
    {
        var id_usuario = PlayerPrefs.GetInt("id_usuario", -1);

        string conn = SetDataBaseClass.SetDataBase(NameDB + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

        // Verifica se o ID já existe na tabela pontuacao
        string checkQuery = "SELECT COUNT(*) FROM pontuacao WHERE id_usuario = @idUsuario";
        dbcmd.CommandText = checkQuery;
        dbcmd.Parameters.Add(new SqliteParameter("@idUsuario", id_usuario));

        int idExistence = Convert.ToInt32(dbcmd.ExecuteScalar());

        if (idExistence > 0)
        {
            Debug.Log("ID já cadastrado");
            // Trate conforme necessário, como exibir uma mensagem para o usuário
        }
        else
        {
            // ID não existe, pode realizar a inserção
            string insertQuery = "INSERT INTO pontuacao (id_usuario, amostras) VALUES (@idUsuario, 0)";
            dbcmd.CommandText = insertQuery;

            try
            {
                dbcmd.ExecuteNonQuery();
                Debug.Log("Inserção bem-sucedida.");
                SceneManager.LoadScene("Login");
            }
            catch (System.Exception ex)
            {
                Debug.Log("Erro durante a inserção: " + ex.Message);
            }
        }

        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }

    
    public void ExibirAmostras()
    {
        string conn = SetDataBaseClass.SetDataBase(DataBaseName + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

        // Recupera o valor de "id_usuario" usando PlayerPrefs
        int id_usuario = PlayerPrefs.GetInt("id_usuario", -1);

        string SQlQuery = "SELECT amostras FROM pontuacao WHERE id_usuario = @id_usuario";
        dbcmd.Parameters.Add(new SqliteParameter("@id_usuario", id_usuario));

        dbcmd.CommandText = SQlQuery;
        reader = dbcmd.ExecuteReader();

        if (reader.Read())
        {
            // Use GetInt32 para recuperar um valor inteiro da coluna "amostras"
            int valorAmostras = reader.GetInt32(0);
            Debug.Log(valorAmostras);

            // Converta o valor inteiro para uma string antes de atribuir ao Text
            Amostras.text = valorAmostras.ToString();
        }

        reader.Close();

        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }


    static public void DataBaseAddAmostras(string NameDB, int valorAmostras)
    {
        var id_usuario = PlayerPrefs.GetInt("id_usuario", -1);

        string conn = SetDataBaseClass.SetDataBase(NameDB + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

        // Verifica se o ID já existe na tabela pontuacao
        string checkQuery = "SELECT COUNT(*) FROM pontuacao WHERE id_usuario = @idUsuario";
        dbcmd.CommandText = checkQuery;
        dbcmd.Parameters.Add(new SqliteParameter("@idUsuario", id_usuario));

        int idExistence = Convert.ToInt32(dbcmd.ExecuteScalar());

        if (idExistence > 0)
        {
            // ID já existe, realiza a atualização
            string updateQuery = "UPDATE pontuacao SET amostras = amostras + @valorAmostras WHERE id_usuario = @idUsuario";
            dbcmd.CommandText = updateQuery;
            dbcmd.Parameters.Add(new SqliteParameter("@idUsuario", id_usuario));
            dbcmd.Parameters.Add(new SqliteParameter("@valorAmostras", valorAmostras));

            try
            {
                dbcmd.ExecuteNonQuery();
                Debug.Log("Atualização bem-sucedida.");
            }
            catch (System.Exception ex)
            {
                Debug.Log("Erro durante a atualização: " + ex.Message);
            }
        }
        else
        {
            // ID não existe, pode realizar a inserção
            string insertQuery = "INSERT INTO pontuacao (id_usuario, amostras) VALUES (@idUsuario, @valorAmostras)";
            dbcmd.CommandText = insertQuery;
            dbcmd.Parameters.Add(new SqliteParameter("@idUsuario", id_usuario));
            dbcmd.Parameters.Add(new SqliteParameter("@valorAmostras", valorAmostras));

            try
            {
                dbcmd.ExecuteNonQuery();
                Debug.Log("Inserção bem-sucedida.");
            }
            catch (System.Exception ex)
            {
                Debug.Log("Erro durante a inserção: " + ex.Message);
            }
        }

        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }


}
