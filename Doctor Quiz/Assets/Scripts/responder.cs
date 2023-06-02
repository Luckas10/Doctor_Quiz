using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class responder : MonoBehaviour
{
    public Text questionText;
    public List<Button> optionButtons;

    private List<Question> questions;
    private Question currentQuestion;

    private void Start()
    {
        LoadQuestionsFromJSON();
        ShuffleQuestions();
        DisplayQuestion();
    }

    // Carrega as perguntas do arquivo JSON
    private void LoadQuestionsFromJSON()
    {
        // Caminho do arquivo JSON contendo as perguntas
        string filePath = Path.Combine(Application.streamingAssetsPath, "novas_perguntas.json");

        // Verifica se o arquivo existe
        if (File.Exists(filePath))
        {
            // Lê o conteúdo do arquivo JSON
            string json = File.ReadAllText(filePath);

            // Converte o JSON em uma instância de QuestionList usando JsonUtility
            QuestionList questionList = JsonUtility.FromJson<QuestionList>(json);

            // Obtém a lista de perguntas do QuestionList
            questions = questionList.questions;
        }
        else
        {
            // Exibe uma mensagem de erro caso o arquivo não seja encontrado
            Debug.LogError("Arquivo JSON de perguntas não encontrado: " + filePath);
        }
        
        // Embaralha as perguntas e exibe a primeira pergunta
        ShuffleQuestions();
        DisplayQuestion();
    }

    private void ShuffleQuestions()
    {
        // Embaralhar a lista de perguntas
        questions = questions.OrderBy(q => Random.Range(0, 1000)).ToList();

        // Embaralhar as opções de cada pergunta
        foreach (Question question in questions)
        {
            question.options = question.options.OrderBy(o => Random.Range(0, 1000)).ToList();
        }
    }


    // Exibe a pergunta atual na interface do usuário
    private void DisplayQuestion()
    {
        // Verifica se ainda existem perguntas na lista
        if (questions.Count > 0)
        {   
            // Check if the question has an image
            string imageCaminho = "Assets/Images" + currentQuestion.imagePath;
            questionImage.sprite = Resources.Load<Sprite>(imagePath);
            if (!string.IsNullOrEmpty(currentQuestion.imagePath))
            {
                // Enable image display
                questionImage.gameObject.SetActive(true);
                // Load the image using the imagePath (can be a URL, local path, etc.)
                // questionImage.sprite = LoadImage(currentQuestion.imagePath);
            }
            else
            {
                // Disable image display
                questionImage.gameObject.SetActive(false);
            }

            // Obtém a primeira pergunta da lista
            currentQuestion = questions[0];
            questionText.text = currentQuestion.questionText;

            // Atualiza o texto da pergunta na interface do usuário
            questionText.text = currentQuestion.questionText;

            // Atualiza o texto das opções nos botões correspondentes
            for (int i = 0; i < optionButtons.Count; i++)
            {
                optionButtons[i].GetComponentInChildren<Text>().text = currentQuestion.options[i];
            }
        }
    }


    // Verifica a resposta selecionada pelo jogador
    public void CheckAnswer(int optionIndex)
    {
        // Verifica se há uma pergunta atual e se a opção selecionada é a correta
        if (currentQuestion != null && optionIndex == currentQuestion.correctOptionIndex)
        {
            Debug.Log("Resposta correta!");
        }
        else
        {
            Debug.Log("Resposta incorreta!");
        }

        // Remove a pergunta atual da lista, avançando para a próxima pergunta
        questions.RemoveAt(0);

        // Verifica se ainda existem perguntas na lista
        if (questions.Count > 0)
        {
            // Exibe a próxima pergunta na interface do usuário
            DisplayQuestion();
        }
        else
        {
            Debug.Log("Quiz concluído!");
        }
    }

    [System.Serializable]
    public class Question
    {
        public string questionText;                // Texto da pergunta
        public List<string> options;               // Lista de opções de resposta
        public int correctOptionIndex;             // Índice da opção correta na lista de opções
    }

    [System.Serializable]
    public class QuestionList
    {
        public List<Question> questions;           // Lista de perguntas
    }
}

// Lógica do código:
// - O método Start() é chamado quando o objeto é inicializado, ele carrega as perguntas do arquivo JSON, embaralha as perguntas e exibe a primeira pergunta.
// - LoadQuestionsFromJSON(): Carrega as perguntas do arquivo JSON e as atribui à lista de perguntas "questions".
// - ShuffleQuestions(): Embaralha as perguntas e as opções de resposta usando valores aleatórios.
// - DisplayQuestion(): Exibe a pergunta atual na interface do usuário, atualizando o texto da pergunta e das opções de resposta nos botões.
// - CheckAnswer(int optionIndex): Verifica se a resposta selecionada pelo jogador está correta, exibindo uma mensagem correspondente. Remove a pergunta atual da lista e exibe a próxima pergunta, se houver, ou exibe uma mensagem de conclusão do quiz.