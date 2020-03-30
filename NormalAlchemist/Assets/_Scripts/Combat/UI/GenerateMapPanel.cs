using UnityEngine;
using UnityEngine.UI;

public class GenerateMapPanel : MonoBehaviour
{
    public InputField SeedInputField;
    public Button GenerateBtn;

    private void Awake()
    {
        GenerateBtn.onClick.AddListener(() =>
        {
            int.TryParse(SeedInputField.text, out int seed);
            EventManager.Broadcast(EventsEnum.StartGenerateGridMap, seed);
            gameObject.SetActive(false);
        });
    }
}
