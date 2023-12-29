using UnityEngine;
using System.Collections;

public class CreateCircle : MonoBehaviour
{
    public GameObject objectPrefab;
    public int numberOfObjects = 10;
    public float radius = 5f;

    private bool[] isObjectActive;
    private Coroutine idleCoroutine;
    private Coroutine clockwiseCoroutine;

    public float minBlinkInterval = 0.5f;
    public float maxBlinkInterval = 2f;
    public float clockwiseBlinkSpeed = 0.5f; // Ajuste a velocidade aqui
    public float clockwiseStageDuration = 5f; // Duração em segundos, ajuste conforme necessário

    private void OnEnable()
    {
        isObjectActive = new bool[numberOfObjects];
        CreateCircleOfObjects();
        StartRandomBlinking();
    }

    void CreateCircleOfObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * (360f / numberOfObjects);
            Vector3 position = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            GameObject instantiatedObject = Instantiate(objectPrefab);
            instantiatedObject.transform.parent = transform;
            instantiatedObject.transform.localPosition = position;
            instantiatedObject.transform.localRotation = rotation;

            instantiatedObject.SetActive(false);
            isObjectActive[i] = false;
        }
    }

    public void StartRandomBlinking()
    {
        StopAllCoroutines(); // Parar qualquer corrotina em execução
        idleCoroutine = StartCoroutine(IdleStage());
    }

    public void StartClockwiseBlinking()
    {
        StopAllCoroutines(); // Parar qualquer corrotina em execução
        clockwiseCoroutine = StartCoroutine(ClockwiseStage());
    }

    IEnumerator IdleStage()
    {
        while (true)
        {
            int randomIndex = Random.Range(0, numberOfObjects);
            isObjectActive[randomIndex] = !isObjectActive[randomIndex];
            transform.GetChild(randomIndex).gameObject.SetActive(isObjectActive[randomIndex]);

            yield return new WaitForSeconds(Random.Range(minBlinkInterval, maxBlinkInterval));
        }
    }

    IEnumerator ClockwiseStage()
    {
        // Desliga todos os objetos no início do estágio ClockwiseStage
        for (int i = 0; i < numberOfObjects; i++)
        {
            isObjectActive[i] = false;
            transform.GetChild(i).gameObject.SetActive(isObjectActive[i]);
        }

        float elapsedTime = 0f;

        while (elapsedTime < clockwiseStageDuration)
        {
            for (int i = 0; i < numberOfObjects; i++)
            {
                isObjectActive[i] = true;
                transform.GetChild(i).gameObject.SetActive(isObjectActive[i]);

                yield return new WaitForSeconds(clockwiseBlinkSpeed);

                isObjectActive[i] = false;
                transform.GetChild(i).gameObject.SetActive(isObjectActive[i]);
            }

            elapsedTime += clockwiseBlinkSpeed * numberOfObjects;
            yield return null; // Aguarda um quadro para atualizar elapsedTime
        }

        // No final do ClockwiseStage, liga e desliga todos os objetos simultaneamente
        for (int i = 0; i < numberOfObjects; i++)
        {
            isObjectActive[i] = true;
            transform.GetChild(i).gameObject.SetActive(isObjectActive[i]);
        }

        yield return new WaitForSeconds(Random.Range(minBlinkInterval, maxBlinkInterval));

        for (int i = 0; i < numberOfObjects; i++)
        {
            isObjectActive[i] = false;
            transform.GetChild(i).gameObject.SetActive(isObjectActive[i]);
        }
    }
}
