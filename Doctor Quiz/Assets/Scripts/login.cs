using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class login : MonoBehaviour
{
    public InputField EmailInput;
    public InputField PasswordInput;
    public string DataBaseName;
    public Text LoginStatus;

    public void InsertLogin()
    {
        string _EmailInput = EmailInput.text.Trim();
        string _PasswordInput = PasswordInput.text.Trim();
        string conn = SetDataBaseClass.SetDataBase(DataBaseName + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

        // checa se existe o usuario
        string SQlQuery = "Select count(*) from usuarios where email='" + _EmailInput + "' and senha='" + _PasswordInput + "'";
        dbcmd.CommandText = SQlQuery;
        int result = Convert.ToInt32(dbcmd.ExecuteScalar());

        if (result > 0)
        {
            // se existir, pega o id do usuario
            SQlQuery = "Select id_usuario from usuarios where email='" + _EmailInput + "' and senha='" + _PasswordInput + "'";
            dbcmd.CommandText = SQlQuery;
            reader = dbcmd.ExecuteReader();
            if (reader.Read())
            {
                int id_usuario = reader.GetInt32(0);
                PlayerPrefs.SetInt("id_usuario", id_usuario);
            }
            reader.Close();

            LoginStatus.text = "Login realizado com sucesso";

            pontuacao.AddAmostras(DataBaseName);

            SceneManager.LoadScene("Dashboard");
        }
        else
        {
            LoginStatus.text = "Email ou senha inválido";
        }

        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }
}
