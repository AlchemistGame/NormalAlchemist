using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGrid : MonoBehaviour
{
    public int mapWidth = 10;
    public int mapHeight = 10;

    public MapCell cellPrefab;
    public Text cellLabelPrefab;

    private Canvas gridCanvas;
    
    private List<MapCell> mapCells = new List<MapCell>();

    private void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                CreateMapCell(x, z);
            }
        }
    }

    private void CreateMapCell(int x, int z)
    {
        Vector3 position = new Vector3(x * 10f, 0f, z * 10f);

        MapCell tempCell = Instantiate<MapCell>(cellPrefab);
        tempCell.transform.SetParent(transform, false);
        tempCell.transform.localPosition = position;

        Text tempLabel = Instantiate<Text>(cellLabelPrefab);
        tempLabel.transform.SetParent(gridCanvas.transform, false);
        tempLabel.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        tempLabel.text = "(" + x.ToString() + ", " + z.ToString() + ")";

        mapCells.Add(tempCell);
    }
}
