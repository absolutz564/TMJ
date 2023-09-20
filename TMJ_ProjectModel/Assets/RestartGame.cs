using UnityEngine;

public class RestartGame : MonoBehaviour
{
    private const string ExecutionCountKey = "ExecutionCount";
    private const int MaxExecutionsBeforeRestart = 3;

    private void Start()
    {
        int executionCount = PlayerPrefs.GetInt(ExecutionCountKey, 0);
        executionCount++;
        Debug.Log("execução " + executionCount);
        PlayerPrefs.SetInt(ExecutionCountKey, executionCount);
        PlayerPrefs.Save();

        if (executionCount >= MaxExecutionsBeforeRestart)
        {
            PlayerPrefs.SetInt(ExecutionCountKey, 0);
            Restart();
        }
    }

    public void Restart()
    {
        // Seu código de reinício aqui

        // Após o reinício, você pode redefinir a contagem
        PlayerPrefs.SetInt(ExecutionCountKey, 0);
        PlayerPrefs.Save();
    }
}
