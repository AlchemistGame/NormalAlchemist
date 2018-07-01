using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewMapPanel : MonoBehaviour
{
    public Button closeBtn;
    public Dropdown mapSizeDropdown;
    public Button finishBtn;

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
            Destroy(gameObject);
        });

        List<string> mapSizeDropOptions = new List<string>
        {
            "100x100x100",
            "500x500x500",
        };
        mapSizeDropdown.ClearOptions();
        mapSizeDropdown.AddOptions(mapSizeDropOptions);

        finishBtn.onClick.AddListener(CreateMap);
    }

    /// <summary>
    /// 创建地图
    /// </summary>
    void CreateMap()
    {
        this.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
