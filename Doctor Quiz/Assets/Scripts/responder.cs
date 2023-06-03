using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Responder : MonoBehaviour
{
    public Text questionText;
    public Image questionImage;
    public List<Button> optionButtons;

    private List<Question> questions;
    private Question currentQuestion;

    private string imageDirectoryPath;

    private void Start()
    {
        imageDirectoryPath = Path.Combine(Application.dataPath, "DoctorQuiz", "Assets", "Images");
        LoadQuestionsFromJSON();
        ShuffleQuestions();
        DisplayQuestion();
    }

    // Carrega as perguntas do arquivo JSON
    private void LoadQuestionsFromJSON()
    {
        // Caminho do arquivo JSON contendo as perguntas
        string filePath = Path.Combine(Application.dataPath, "Scripts", "DoctorQuiz", "novas_perguntas.json");

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
    }

    private void ShuffleQuestions()
    {
        // Embaralhar a lista de perguntas
        questions = questions.OrderBy(q => Random.Range(0, 1000)).ToList();

        // Embaralhar as opções de cada pergunta e manter a alternativa correta na posição correta
        foreach (Question question in questions)
        {
            // Obtenha a alternativa correta antes do embaralhamento
            string correctOption = question.options[question.correctOptionIndex];

            // Embaralhar as opções de resposta, excluindo a alternativa correta
            List<string> shuffledOptions = question.options.Where(o => o != correctOption).ToList();
            shuffledOptions = shuffledOptions.OrderBy(o => Random.Range(0, 1000)).ToList();

            // Insira a alternativa correta em uma posição aleatória
            int correctOptionPosition = Random.Range(0, shuffledOptions.Count + 1);
            shuffledOptions.Insert(correctOptionPosition, correctOption);

            // Atualize a lista de opções embaralhadas na pergunta
            question.options = shuffledOptions;
        }
    }

    // Exibe a pergunta atual na interface do usuário
    private void DisplayQuestion()
    {
        // Verifica se ainda existem perguntas na lista
        if (questions.Count > 0)
        {
            // Obtém a primeira pergunta da lista
            currentQuestion = questions[0];
            questionText.text = currentQuestion.questionText;

            // Verifica se a pergunta possui uma imagem
            if (!string.IsNullOrEmpty(currentQuestion.questionImage))
            {
                // Ativa a exibição da imagem
                questionImage.gameObject.SetActive(true);

                // Carrega a imagem usando o caminho da imagem (pode ser um URL, caminho local, etc.)
                LoadImage(currentQuestion.questionImage);
            }
            else
            {
                // Desativa a exibição da imagem
                questionImage.gameObject.SetActive(false);
            }

            // Atualiza o texto das opções nos botões correspondentes
            for (int i = 0; i < optionButtons.Count; i++)
            {
                optionButtons[i].GetComponentInChildren<Text>().text = currentQuestion.options[i];
            }
        }
    }

    // Carrega uma imagem
    private void LoadImage(string imageName)
    {
        // Constrói o caminho completo para a imagem usando o diretório base e o nome do arquivo
        string imagePath = Path.Combine(imageDirectoryPath, imageName);

        // Verifica se o arquivo da imagem existe
        if (File.Exists(imagePath))
        {
            // Lê os bytes do arquivo da imagem
            byte[] imageData = File.ReadAllBytes(imagePath);

            // Cria uma textura e carrega os bytes da imagem
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            // Cria um sprite com base na textura e atribui ao componente de imagem
            questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            // Exibe uma mensagem de erro caso o arquivo da imagem não seja encontrado
            Debug.LogError("Arquivo da imagem não encontrado: " + imagePath);
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
        public string questionImage;               // Nome do arquivo da imagem da pergunta
        public List<string> options;               // Lista de opções de resposta
        public int correctOptionIndex;             // Índice da opção correta na lista de opções
    }

    [System.Serializable]
    public class QuestionList
    {
        public List<Question> questions;           // Lista de perguntas
    }
}
