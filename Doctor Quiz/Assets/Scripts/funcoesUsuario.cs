using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class funcoesUsuario : MonoBehaviour
{
    public InputField NameInput;
    public InputField SobrenomeInput;
    public InputField EmailInput;
    public InputField PasswordInput;
    public string DataBaseName;
    public Text CadastroStatus;

    public void InsertInto()
    {
        var _NameInput = NameInput.text.Trim();
        var _SobrenomeInput = SobrenomeInput.text.Trim();
        var _EmailInput = EmailInput.text.Trim();
        var _PasswordInput = PasswordInput.text.Trim();

        if (string.IsNullOrEmpty(_NameInput) || string.IsNullOrEmpty(_SobrenomeInput) ||
            string.IsNullOrEmpty(_EmailInput) || string.IsNullOrEmpty(_PasswordInput))
        {
            CadastroStatus.text = "Todos os campos devem ser preenchidos.";
            return;
        }

        if (_EmailInput.Length < 7)
        {
            CadastroStatus.text = "E-mail inválido. O email deve ter pelo menos 7 caracteres.";
            return;
        }

        if (_PasswordInput.Length < 8)
        {
            CadastroStatus.text = "A senha deve ter pelo menos 8 caracteres.";
            return;
        }

        string conn = SetDataBaseClass.SetDataBase(DataBaseName + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();
        string SQlQuery = "Insert Into Usuarios(nome, sobrenome, email, senha)" +
                          "Values('" + _NameInput + "', '" + _SobrenomeInput + "', '" + _EmailInput + "', '" + _PasswordInput + "')";
        dbcmd.CommandText = SQlQuery;

        try
        {
            dbcmd.ExecuteNonQuery();
            Debug.Log("Inserção bem-sucedida.");
            SceneManager.LoadScene("Login");
        }
        catch (System.Exception)
        {
            CadastroStatus.text = "E-mail já cadastrado";
        }

        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
    }

}