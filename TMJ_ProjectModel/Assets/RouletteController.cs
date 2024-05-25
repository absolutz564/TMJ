using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool CanGetProduct = true;
    public bool CanGetSpecialGift = true;

    void Start()
    {
        VerificarLimite("ProductsDelivered", "ProductsLimit", "ProdutoSaoBraz");
        VerificarLimite("SpecialGiftDelivered", "SpecialGiftsLimit", "BrindeEspecial");
    }

    void VerificarLimite(string playerPrefKey, string playerPrefLimitKey, string elementoARemover)
    {
        int delivered = PlayerPrefs.GetInt(playerPrefKey, 0);
        int limit = PlayerPrefs.GetInt(playerPrefLimitKey, 0);
        if (delivered >= limit)
        {
            if (elementoARemover == "ProdutoSaoBraz")
            {
                CanGetProduct = false;
            }
            if (elementoARemover == "BrindeEspecial")
            {
                CanGetSpecialGift = false;
            }
            Debug.Log("Elemento " + elementoARemover + " removido");
        }
    }

    void Update()
    {
        if (girando)
        {
            roleta.Rotate(0f, 0f, velocidadeAtual * Time.deltaTime);

            float anguloAtual = roleta.eulerAngles.z % 360;
            float anguloDestino = Mathf.Round(anguloAtual / anguloPorItem) * anguloPorItem;

            // Ajuste para evitar os elementos removidos
            while (!elementos.Contains(elementos[Mathf.RoundToInt(anguloDestino / anguloPorItem) % elementos.Length]))
            {
                anguloDestino += anguloPorItem;
            }

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
                    IncrementarPlayerPrefs(elementoSelecionado);
                    girando = false;
                    StartCoroutine(WaitToEnd());
                }
            }
        }
    }

    public void ButtonClick()
    {
        if (!girando)
        {
            //Elemento 3 = Tente novamente
            //Elemento 4 = Produto São Braz
            //Elemento 5 = Não foi dessa vez
            //Elemento 6 = Brinde Especial
            //Elemento 7 = Tente novamente
            //random.range(3,8)
            List<int> randomRotateValues = new List<int> { 3, 5, 7 };

            if (CanGetSpecialGift)
            {
                randomRotateValues.Add(6);
                Debug.Log("Pode achar Brinde");
            } 
            if (CanGetProduct) {
                randomRotateValues.Add(4);
                Debug.Log("Pode achar São Braz");
            }

            int valorAleatorio = GetRandomValue(randomRotateValues);
            Debug.Log("Valor aleatório: " + valorAleatorio);
            if (valorAleatorio == 3)
            {
                Debug.Log("Tente Novamente");
            }
            if (valorAleatorio == 4)
            {
                Debug.Log("Produto São Brazz");
            }
            if (valorAleatorio == 5)
            {
                Debug.Log("Não Foi dEssa vez");
            }
            if (valorAleatorio == 6)
            {
                Debug.Log("Brinde Especial");
            }
            if (valorAleatorio == 7)
            {
                Debug.Log("Tente Novamente");
            }

            velocidadeInicial = valorAleatorio * 100;
            desaceleracao = velocidadeInicial + 5;
            Tittle.text = "''Girando...''";
            Bottom2.SetActive(true);
            ArrowAnim.SetTrigger("Start");
            StartCoroutine(WaitToStart());
        }
    }
    private int GetRandomValue(List<int> randomRotateValues)
    {
        int index = Random.Range(0, randomRotateValues.Count);
        return randomRotateValues[index];
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(0.5f);

        girando = true;
        tempoDecorrido = 0f;
        velocidadeAtual = velocidadeInicial;
    }
    private void IncrementarPlayerPrefs(string elemento)
    {
        switch (elemento)
        {
            case "ProdutoSaoBraz":
                IncrementPlayerPrefs("ProductsDelivered");
                Debug.Log("ProdutoSaoBraz rewarded, ProductsDelivered incremented");
                break;
            case "BrindeEspecial":
                IncrementPlayerPrefs("SpecialGiftDelivered");
                Debug.Log("BrindeEspecial rewarded, SpecialGiftDelivered incremented");
                break;
            default:
                Debug.Log("No special reward");
                break;
        }
    }

    private void IncrementPlayerPrefs(string key)
    {
        int currentValue = PlayerPrefs.GetInt(key, 0);
        PlayerPrefs.SetInt(key, currentValue + 1);
        PlayerPrefs.Save();
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
}
