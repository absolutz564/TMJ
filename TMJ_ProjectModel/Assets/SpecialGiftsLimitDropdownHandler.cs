using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SpecialGiftsTMPDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown tmpDropdown; // Referência ao TMP_Dropdown

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
    public void LoadNextScene()
    {
        SceneManager.LoadScene("Screen");
    }
}
