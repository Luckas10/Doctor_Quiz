using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;

public class Perfil : MonoBehaviour
{

    public Text textName;
    public string DataBaseName;

    void Start()
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
}
