using System;

[Serializable]
public class TileData
{
    public string id;
    public string LoadPath;
}

[Serializable]
public class CategoryTilesData
{
    public string CategoryName;
    public TileData[] tiles;
}

[Serializable]
public class MapTilesData
{
    public CategoryTilesData[] categories;
}