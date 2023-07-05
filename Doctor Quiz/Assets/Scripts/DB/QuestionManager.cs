using UnityEngine;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    private DatabaseManager _databaseManager;

    private void Start()
    {
        _databaseManager = GetComponent<DatabaseManager>();
    }

    public void AddQuestionToDatabase(DatabaseManager.Question question)
    {
        _databaseManager.AddQuestion(question);
    }

    public List<DatabaseManager.Question> GetQuestionsFromDatabase()
    {
        return _databaseManager.GetQuestions();
    }

    public void RemoveAllQuestionsFromDatabase()
    {
        _databaseManager.RemoveAllQuestions();
    }
}
