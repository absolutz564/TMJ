using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SpecialGiftsTMPDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown tmpDropdown; // Referência ao TMP_Dropdown
    public TMP_InputField textProductsLimit;
    public TMP_InputField textSpecialGiftsLimit;

    private void Awake()
    {
        Screen.SetResolution(1080, 1920, true);
    }
    void Start()
    {
        PlayerPrefs.SetInt("ProductsDelivered", 0);
        PlayerPrefs.SetInt("SpecialGiftDelivered", 0);

        // Inicializar o TMP_Dropdown com valores de 1 a 20
        tmpDropdown.options.Clear();
        for (int i = 0; i <= 20; i++)
        {
            tmpDropdown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }

        // Adicionar listener para quando o valor for alterado
        tmpDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(tmpDropdown);
        });
        if (!PlayerPrefs.HasKey("SpecialGiftsLimit"))
        {
            PlayerPrefs.SetInt("SpecialGiftsLimit", 0);
            PlayerPrefs.Save(); // Salvar mudanças
        }
        // Setar o valor inicial baseado no PlayerPrefs
        int savedValue = PlayerPrefs.GetInt("SpecialGiftsLimit", 0); // Valor padrão é 1
        tmpDropdown.SetValueWithoutNotify(savedValue - 1); // Ajuste para índice 0 baseado
        tmpDropdown.RefreshShownValue();

    }

    public void DropdownValueChanged(TMP_Dropdown change)
    {
        // Atualizar o PlayerPrefs com o novo valor
        int selectedValue = change.value; // Ajuste para valor real baseado no índice
        PlayerPrefs.SetInt("SpecialGiftsLimit", selectedValue);
        PlayerPrefs.Save(); // Salvar mudanças
    }

    private void Update()
    {
        //if (Input.anyKeyDown)
        //{
        //    if (!(Input.GetKeyDown(KeyCode.Alpha0) ||
        //          Input.GetKeyDown(KeyCode.Alpha1) ||
        //          Input.GetKeyDown(KeyCode.Alpha2) ||
        //          Input.GetKeyDown(KeyCode.Alpha3) ||
        //          Input.GetKeyDown(KeyCode.Alpha4) ||
        //          Input.GetKeyDown(KeyCode.Alpha5) ||
        //          Input.GetKeyDown(KeyCode.Alpha6) ||
        //          Input.GetKeyDown(KeyCode.Alpha7) ||
        //          Input.GetKeyDown(KeyCode.Alpha8) ||
        //          Input.GetKeyDown(KeyCode.Alpha9)))
        //    {
        //        LoadNextScene();
        //    }
        //}
    }

    public IEnumerator WaitToNext()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Screen");
    }
    public void LoadNextScene()
    {
        if (textProductsLimit.text.Length > 0)
        {
            int productsLimit = int.Parse(textProductsLimit.text);
            PlayerPrefs.SetInt("ProductsLimit", productsLimit);
        }
        if (textSpecialGiftsLimit.text.Length > 0)
        {
            int SpecialGiftsLimit = int.Parse(textSpecialGiftsLimit.text);
            PlayerPrefs.SetInt("SpecialGiftsLimit", SpecialGiftsLimit);
        }
        if (textProductsLimit.text.Length > 0 && textSpecialGiftsLimit.text.Length > 0)
        {
            StartCoroutine(WaitToNext());
        }
    }
}
