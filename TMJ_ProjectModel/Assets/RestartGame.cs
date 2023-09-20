using UnityEngine;

public class RestartGame : MonoBehaviour
{
    private const string ExecutionCountKey = "ExecutionCount";
    private const int MaxExecutionsBeforeRestart = 3;

    private void Start()
    {
        int executionCount = PlayerPrefs.GetInt(ExecutionCountKey, 0);
        executionCount++;
        Debug.Log("execu��o " + executionCount);
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
        // Seu c�digo de rein�cio aqui

        // Ap�s o rein�cio, voc� pode redefinir a contagem
        PlayerPrefs.SetInt(ExecutionCountKey, 0);
        PlayerPrefs.Save();
    }
}
