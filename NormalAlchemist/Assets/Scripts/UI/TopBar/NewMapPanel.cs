using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewMapPanel : MonoBehaviour
{
    public Button closeBtn;
    public Button finishBtn;

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            this.gameObject.SetActive(false);
            Destroy(gameObject);
        });

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
