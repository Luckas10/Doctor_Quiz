using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;

public class funcoesUsuario : MonoBehaviour
{

    public InputField NameInput;
    public InputField SobrenomeInput;
    public InputField EmailInput;
    public InputField PasswordInput;
    public string DataBaseName;

    public void InsertInto()
    {

        var _NameInput = NameInput.text.Trim();
        var _SobrenomeInput = SobrenomeInput.text.Trim();
        var _EmailInput = EmailInput.text.Trim();
        var _PasswordInput = PasswordInput.text.Trim();

        string conn = SetDataBaseClass.SetDataBase(DataBaseName + ".db");
        IDbConnection dbcon;
        IDbCommand dbcmd;
        IDataReader reader;

        dbcon = new SqliteConnection(conn);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();
        string SQlQuery = "Insert Into Usuarios(nome, sobrenome, email, senha)" +
                          "Values('" + _NameInput + "', '" + _SobrenomeInput + "', '" + _EmailInput + "', '" + _PasswordInput + "')";
        dbcmd.CommandText = SQlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbcon.Close();
        dbcon = null;
        
    }

}
