using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementController : MonoBehaviour
{

    public Sprite Front;
    public Sprite versoImage;

    public bool Flipped = false;

    public bool Finded = false;

    public string Id = "";

    public RectTransform currentTransform;
    public Vector3 baseScale;

    public Image imagemBotao;
    Vector3 escala;

    void Awake()
    {
        currentTransform = this.GetComponent<RectTransform>();
        baseScale = new Vector3(2f, 2f, 2f);
    }
    // Start is called before the first frame update
    void Start()
    {
        imagemBotao = this.GetComponent<Image>();
        escala = imagemBotao.GetComponent<RectTransform>().localScale;
        //MemoryGameController.Instance.scaletext.text = escala.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //if (imagemBotao)
        //{
        //    MemoryGameController.Instance.scaletext.text = imagemBotao.GetComponent<RectTransform>().localScale.ToString();
        //}
    }
    public void OnButtonClick()
    {
        if (MemoryGameController.Instance.gameStarted)
        {
            if (MemoryGameController.Instance.CanClick && (MemoryGameController.Instance.card1 == null ||
                                                           MemoryGameController.Instance.card2 == null))
            {
                if (!Flipped && !Finded)
                {
                    MemoryGameController.Instance.ResetCanClick();
                    StartCoroutine(GirarBotao(versoImage));
                    MemoryGameController.Instance.AddFlipped(this);
                }
            }
        }
        else
        {
            if (!MemoryGameController.Instance.CounterText.gameObject.activeSelf)
            {
                MemoryGameController.Instance.SendMessage("Toque no bot�o come�ar", false);

            }
        }
    }

    public void Unflip()
    {
        StartCoroutine(VoltarBotao(Front));
    }

    public void Reset()
    {
        StartCoroutine(VoltarBotao(Front));
    }

    public IEnumerator VoltarBotao(Sprite spriteInicial)
    {
        float lastTime = Time.realtimeSinceStartup;

        float duracao = 0.5f; // Dura��o da anima��o de giro
        float anguloInicial = 0; // Angulo inicial do bot�o
        float anguloFinal = 180; // Angulo final do bot�o

        // Anima��o de giro
        float tempoDecorrido = 0;
        bool jaGirou = false;
        while (tempoDecorrido <= duracao / 2)
        {
            float t = tempoDecorrido / (duracao / 2);
            float angulo = Mathf.Lerp(anguloInicial, anguloFinal, t);
            currentTransform.rotation = Quaternion.Euler(0, angulo, 0);

            if (!jaGirou && angulo >= 90)
            {
                jaGirou = true;
                // Trocar a imagem do bot�o
                imagemBotao.sprite = spriteInicial;
                escala.x = 1f;
                imagemBotao.GetComponent<RectTransform>().localScale = escala;
            }

            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        currentTransform.rotation = Quaternion.Euler(0, 0, 0);
        imagemBotao.GetComponent<RectTransform>().localScale = baseScale;
    }

    public IEnumerator GirarBotao(Sprite sprite)
    {
        float lastTime = Time.realtimeSinceStartup;

        float duracao = 0.5f; // Dura��o da anima��o de giro
        float anguloInicial = 0; // Angulo inicial do bot�o
        float anguloFinal = 90; // Angulo final do bot�o

        // Referencia ao componente Image do bot�o
        Image imagemBotao = GetComponent<Image>();
        Vector3 escala = imagemBotao.GetComponent<RectTransform>().localScale;

        // Anima��o de giro
        float tempoDecorrido = 0;
        while (tempoDecorrido <= duracao / 2)
        {
            float t = tempoDecorrido / (duracao / 2);
            float angulo = Mathf.Lerp(anguloInicial, anguloFinal, t);
            currentTransform.rotation = Quaternion.Euler(0, angulo, 0);

            tempoDecorrido += Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }

        // Trocar a imagem do bot�o
        imagemBotao.sprite = sprite;
        escala.x *= -1;
        imagemBotao.GetComponent<RectTransform>().localScale = escala;

        // Anima��o de giro de volta
        anguloInicial = 90;
        anguloFinal = 180;
        tempoDecorrido = 0;
        while (tempoDecorrido <= duracao / 2)
        {
            float t = tempoDecorrido / (duracao / 2);
            float angulo = Mathf.Lerp(anguloInicial, anguloFinal, t);
            currentTransform.rotation = Quaternion.Euler(0, angulo, 0);

            tempoDecorrido += Time.realtimeSinceStartup - lastTime;
            lastTime = Time.realtimeSinceStartup;
            yield return null;
        }

        // Corre��o da rota��o
        currentTransform.rotation = Quaternion.identity;
        escala.x *= -1;
        imagemBotao.GetComponent<RectTransform>().localScale = escala;
        currentTransform.rotation = Quaternion.Euler(0, 0, 0);

    }
}
