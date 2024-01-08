using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ConvertDataBaseToMultiPlatform : MonoBehaviour
{
    public string DataBaseName;

    public void Awake()
    {
        GenerateConnectionString(DataBaseName + ".db");
    }

    public void GenerateConnectionString(string DatabaseName)
    {
#if UNITY_EDITOR
        string dbPath = Application.dataPath + "/StreamingAssets/" + DatabaseName;
#else
        // Verifique se o arquivo existe em Application.persistentDataPath
        string filepath = Application.persistentDataPath + "/" + DatabaseName;

        if (!File.Exists(filepath) || new System.IO.FileInfo(filepath).Length == 0)
        {
            // Se não existir, carregue de StreamingAssets e salve em persistentDataPath
            string loadPath = "";

#if UNITY_ANDROID
            loadPath = "jar:file://" + Application.dataPath + "!/assets/" + DatabaseName;
#elif UNITY_IOS
            loadPath = Application.dataPath + "/Raw/" + DatabaseName;
#elif UNITY_WP8 || UNITY_WINRT
            loadPath = Application.dataPath + "/StreamingAssets/" + DatabaseName;
#endif

            UnityWebRequest www = UnityWebRequest.Get(loadPath);
            www.SendWebRequest();

            while (!www.isDone) { }

            if (www.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(filepath, www.downloadHandler.data);
            }
            else
            {
                Debug.LogError("Falha ao carregar o banco de dados: " + www.error);
            }
        }

        var dbPath = filepath;
#endif
    }
}
