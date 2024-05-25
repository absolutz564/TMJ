using NekraliusDevelopmentStudio;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class APIRequester : MonoBehaviour
{
    public PhotoTaker photoTaker;
    public GameObject WaintingObject;
    private string bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im9wYTZAdXNlci5jb20iLCJleHBlcmllbmNlX2lkIjoiYzQxMmE2NzUtZGQ2OS00YzRlLThkZDUtZGMwNjQzYmQ4N2I2IiwiaWF0IjoxNzE2MzYyNDYxLCJleHAiOjE3MTY0NDg4NjEsInN1YiI6Ijk0NDBhODUzLTM1N2UtNGVkOS1iMDliLWE1ZTczN2Y3YTRjNCJ9.YpBm-GwV-Z-lbKix0a4n_Wc3kokhba9l3sgmtb7WPDM";

    // URL da API
    private string apiUrl = "http://localhost:3000/getCommand";

    private void Start()
    {
        StartCoroutine(GetCommandFromAPI());
    }

    private IEnumerator GetCommandFromAPI()
    {
        // Configura a requisição
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("Authorization", "Bearer " + bearerToken);

        // Envia a requisição e espera a resposta
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            // Analisa a resposta JSON
            string jsonResponse = request.downloadHandler.text;
            CommandResponse commandResponse = JsonUtility.FromJson<CommandResponse>(jsonResponse);

            // Chama a função com base no comando recebido
            if (commandResponse.command == "ExecuteFunction")
            {
                ExecuteFunction();
            }
        }
    }

    private void ExecuteFunction()
    {
        Debug.Log("Function Executed!");
        // Aqui você coloca o código da função que deseja executar
        WaintingObject.SetActive(false);
        photoTaker.StartVideoAction();
    }

    [System.Serializable]
    private class CommandResponse
    {
        public string command;
    }
}
