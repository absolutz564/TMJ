using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RouletteController : MonoBehaviour
{
    public RectTransform roleta; // Referência ao RectTransform que representa a roleta
    public float velocidadeInicial = 500f; // Velocidade inicial do giro
    public float desaceleracao = 1500f; // Taxa de desaceleração
    public float tempoDeGiro = 2f; // Tempo em segundos que a roleta vai girar
    public string[] elementos; // Nomes dos elementos da roleta

    private bool girando = false;
    private const float anguloPorItem = 90f;
    private float tempoDecorrido = 0f;
    private float velocidadeAtual = 0f;

    public Animator ArrowAnim;

    public GameObject Bottom2;
    public TextMeshProUGUI Tittle;
    void Update()
    {
        if (girando)
        {
            roleta.Rotate(0f, 0f, velocidadeAtual * Time.deltaTime);

            float anguloAtual = roleta.eulerAngles.z % 360;
            float anguloDestino = Mathf.Round(anguloAtual / anguloPorItem) * anguloPorItem;

            if (tempoDecorrido < tempoDeGiro)
            {
                tempoDecorrido += Time.deltaTime;
            }
            else if (Mathf.Abs(velocidadeAtual) > 0)
            {
                velocidadeAtual = Mathf.MoveTowards(velocidadeAtual, 0f, desaceleracao * Time.deltaTime);

                if (Mathf.Abs(velocidadeAtual) <= 1f)
                {
                    roleta.rotation = Quaternion.Euler(0f, 0f, anguloDestino);

                    int indiceElemento = Mathf.RoundToInt(anguloAtual / anguloPorItem) % elementos.Length;
                    string elementoSelecionado = elementos[indiceElemento];
                    Debug.Log("Elemento selecionado: " + elementoSelecionado);
                    PlayerPrefs.SetInt("Element", indiceElemento);
                    girando = false;
                    StartCoroutine(WaitToEnd());
                }
            }
        }
    }

    public IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(2f);
        if (PlayerPrefs.GetInt("Element") == 1 || PlayerPrefs.GetInt("Element") == 3)
        {
            SceneManager.LoadScene("Roulette");
        }
        else
        {
            SceneManager.LoadScene("Screen2 - PhotoTaker");
        }
    }

    public void ButtonClick()
    {
        if (!girando)
        {
            velocidadeInicial = Random.Range(3, 8) * 100;
            desaceleracao = velocidadeInicial + 5;
            Tittle.text = "''Girando...''";
            Bottom2.SetActive(true);
            ArrowAnim.SetTrigger("Start");
            StartCoroutine(WaitToStart());
        }
    }
    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(0.5f);

        girando = true;
        tempoDecorrido = 0f;
        velocidadeAtual = velocidadeInicial;
    }
}
