using UnityEngine;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    private SQLiteConnection _connection;

    private void Awake()
    {
        string databasePath = Application.persistentDataPath + "/questions.db";
        _connection = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // Create the questions table if it doesn't exist yet
        _connection.CreateTable<Question>();
    }

    private void OnDestroy()
    {
        _connection.Close();
    }

    // Method to add a new question to the database
    public void AddQuestion(Question question)
    {
        _connection.Insert(question);
    }

    // Method to get all questions from the database
    public List<Question> GetQuestions()
    {
        return _connection.Table<Question>().ToList();
    }

    // Method to remove all questions from the database
    public void RemoveAllQuestions()
    {
        _connection.DeleteAll<Question>();
    }

    [System.Serializable]
    public class Question
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string questionText { get; set; }
        public string questionImage { get; set; }
        public string options { get; set; }
        public string correctOptionIndex { get; set; }
    }
}
