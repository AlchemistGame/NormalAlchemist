using System;
using UnityEngine;
using UnityEngine.UI;

public class MapTile : MonoBehaviour
{
    public Image imgPreview;
    public Button btnCurrentTile;
    public Text txtTile;
    public Action onClickCallback;

    private void Start()
    {
        btnCurrentTile.onClick.AddListener(() =>
        {
            onClickCallback();
        });
    }
}
