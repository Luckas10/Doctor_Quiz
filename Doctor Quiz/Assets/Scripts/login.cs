using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using System;

public class login : MonoBehaviour
{

    public InputField EmailInput;
    public InputField PasswordInput;
    public string DataBaseName;
    public Text LoginStatus;

    public void CheckInputs()
    {
        string textoDigitado = EmailInput.text;
        if (textoDigitado.Contains("@gmail.com") && textoDigitado.Length > 10)
        {
            LoginStatus.text = "Email válido!";
        }
        else
        {
            LoginStatus.text = "Digite um email válido";
        }
    }


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
        string SQlQuery = "Select count(*) from usuarios where email='" + _EmailInput + "' and senha='" + _PasswordInput + "'";
        dbcmd.CommandText = SQlQuery;
        int result = Convert.ToInt32(dbcmd.ExecuteScalar());

        if (result > 0)
        {
            LoginStatus.text = "Login realizado com sucesso";
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