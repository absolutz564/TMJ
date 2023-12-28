using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CadencedObjectController : MonoBehaviour
{
    public List<GameObject> targetObjects;
    public float interval = 1.0f;
    public float startDelay = 1.0f; // Tempo de espera inicial

    private int currentIndex = 0;
    private float timer = 0.0f;

    private void Start()
    {
        foreach (GameObject obj in targetObjects)
        {
            obj.SetActive(false);
        }
        
        StartCoroutine(StartDelayed()); // Inicia a corrotina de atraso inicial
    }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForSeconds(startDelay); // Espera pelo tempo de atraso inicial

        // Agora que o atraso inicial passou, começa o processo de ativação dos objetos
        StartCoroutine(ActivateObjects());
    }

    private IEnumerator ActivateObjects()
    {
        while (currentIndex < targetObjects.Count)
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                targetObjects[currentIndex].SetActive(true);
                currentIndex++;
                timer = 0.0f;
            }

            yield return null;
        }
    }
}
