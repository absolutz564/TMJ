using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public float minWaitTimeStart = 4.0f;
    public float maxWaitTimeStart = 6.0f;
    public float minWaitTimeJudge = 4.0f;
    public float maxWaitTimeJudge = 6.0f;
    public float minWaitTimeDance = 13f;
    public float maxWaitTimeDance = 16f;

    public float randomizedTimeStart;
    public float randomizedTimeJudge;
    public float randomizedTimeDance;

    public Animator animVideo;
    public Animator animPreview;

    public List<AudioClip> Musics;
    public AudioSource audioSource;
    public string musicName;

    public List<Sprite> BottleSprites;
    public Image BottleImage;
    public string bottleName;

    private Dictionary<string, int> musicIndices = new Dictionary<string, int>();


    private Dictionary<string, int> bottleIndices = new Dictionary<string, int>();

    public void StartInteraction(Animator itemAnim)
    {
        musicIndices.Add("Asa Branca", 1);
        musicIndices.Add("Forró Pesado", 2);
        musicIndices.Add("Olha a Fogueira", 3);

        PlayMusicByName(musicName);

        bottleIndices.Add("MATUTA UMBURANA", 1);
        bottleIndices.Add("MATUTA CRISTAL", 2);
        bottleIndices.Add("MATUTA MEL & LIMAO", 3);

        PlayMusicByName(musicName);
        ShowBotttle(bottleName);

        randomizedTimeStart = Random.Range(minWaitTimeStart, maxWaitTimeStart);
        randomizedTimeJudge = Random.Range(minWaitTimeJudge, maxWaitTimeJudge);
        randomizedTimeDance = Random.Range(minWaitTimeDance, maxWaitTimeDance);
        StartCoroutine(ActivateObjectAfterRandomTime(itemAnim));
    }



    private void PlayMusicByName(string name)
    {
        // Verifica se o nome existe no dicionário
        if (musicIndices.TryGetValue(name, out int index))
        {
            // Verifica se o índice está dentro do intervalo da lista
            if (index >= 0 && index < Musics.Count)
            {
                // Define o clip de áudio e toca a música
                audioSource.clip = Musics[index];
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Índice fora do intervalo da lista de músicas.");
            }
        }
        else
        {
            Debug.LogError("Nome da música não encontrado.");
        }
    }

    private void ShowBotttle(string name)
    {
        if (bottleIndices.TryGetValue(name, out int index))
        {
            if (index >= 0 && index < BottleSprites.Count)
            {
                BottleImage.sprite = BottleSprites[index];
            }
            else
            {
                Debug.LogError("Índice fora do intervalo da lista de imagens.");
            }
        }
        else
        {
            Debug.LogError("Nome da imagem não encontrado.");
        }
    }

    private IEnumerator ActivateObjectAfterRandomTime(Animator itemAnim)
    {
        // Gera um valor aleatório entre minWaitTime e maxWaitTime

        // Aguarda o tempo gerado
        yield return new WaitForSeconds(randomizedTimeStart);

        // Ativa o GameObject
        itemAnim.gameObject.SetActive(true);
        StartCoroutine(WaitToJudge(itemAnim));
    }

    private IEnumerator WaitToJudge(Animator itemAnim)
    {
        // Aguarda o tempo gerado
        yield return new WaitForSeconds(randomizedTimeJudge);

        // Array com os nomes dos triggers
        string[] triggers = { "julgando1", "julgando2" };

        // Escolhe um índice aleatório entre 0 e 1
        int randomIndex = Random.Range(0, triggers.Length);

        // Define o trigger usando o nome aleatório escolhido
        itemAnim.SetTrigger(triggers[randomIndex]);
        StartCoroutine(WaitToDance(itemAnim));
    }
    private IEnumerator WaitToDance(Animator itemAnim)
    {
        // Aguarda o tempo gerado
        yield return new WaitForSeconds(randomizedTimeDance);

        // Array com os nomes dos triggers
        string[] triggers = { "dance1", "dance2", "dance3" };

        // Escolhe um índice aleatório entre 0 e 1
        int randomIndex = Random.Range(0, triggers.Length);

        // Define o trigger usando o nome aleatório escolhido
        itemAnim.SetTrigger(triggers[randomIndex]);
    }
}
