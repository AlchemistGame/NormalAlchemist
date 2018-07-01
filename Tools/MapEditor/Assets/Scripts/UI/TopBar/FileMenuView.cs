using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileMenuView : MonoBehaviour
{
    public Button newMapBtn;
    public Button openMapBtn;
    public Button saveMapBtn;
    public GameObject menuContent;

    private Toggle fileMenuToggle;
    private GameObject newMapPanelInstance;

	// Use this for initialization
	void Start ()
    {
        fileMenuToggle = this.GetComponent<Toggle>();
        fileMenuToggle.onValueChanged.AddListener((isOn) =>
        {
            menuContent.SetActive(isOn);
        });

        newMapBtn.onClick.AddListener(() =>
        {
            if (newMapPanelInstance == null)
            {
                newMapPanelInstance = Instantiate(Resources.Load<GameObject>("UI/NewMapPanel"));
            }

            fileMenuToggle.isOn = false;
        });

        openMapBtn.onClick.AddListener(() =>
        {
            fileMenuToggle.isOn = false;
        });

        saveMapBtn.onClick.AddListener(() =>
        {
            VoxelFramework.Engine.SaveWorldInstant();

            fileMenuToggle.isOn = false;
        });
    }

}
