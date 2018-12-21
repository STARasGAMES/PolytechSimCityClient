using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Core;

public class BuildingTilesController : MonoBehaviour
{
    [System.Serializable]
    public class BuildingInfo
    {
        public string name;
        public TileBase tile;
    }

    [SerializeField] Tilemap _tilemap;
    [SerializeField] Vector3Int _startCell;
    [SerializeField] int sizeX, sizeY;
    [SerializeField] List<BuildingInfo> _buildingInfos;

    void Start()
    {
        GameSystem.StartMainCoroutine(
            GameSystem.ServerCommunication.GetTestRoute(res =>
            {
                Debug.Log("TEST ROUTE " + res.success);
            }
            )
            );
        //var req = UnityEngine.Networking.UnityWebRequest.Get("http://localhost:5000/api/values");
        ////req.SetRequestHeader("Content-Type", "application/json");
        ////req.url = "http://localhost:5001/api/values";
        //var op = req.SendWebRequest();
        //op.completed += res => { Debug.Log($"isNetworkError: {req.isNetworkError}, isHttpError: {req.isHttpError},"); };
    }

    public void SetBuildings(string[] buildings)
    {
        
        _tilemap.ClearAllTiles();
        int length = sizeX * sizeY;
        if (buildings.Length != length)
            throw new System.Exception("Wrong buildings number. Expect: " + length);
        TileBase[] tiles = new TileBase[length];
        Vector3Int[] positions = new Vector3Int[length];
        for (int i = 0; i < length; i++)
        {
            var tile = GetTileByName(buildings[i]);
            tiles[i] = tile;
            positions[i] = _startCell + new Vector3Int(i / sizeX, i % sizeY, 0);
        }
        _tilemap.SetTiles(positions, tiles);
    }

    private TileBase GetTileByName(string name)
    {
        foreach (var item in _buildingInfos)
        {
            if (item.name.Equals(name))
                return item.tile;
        }
        Debug.LogFormat("Can't find building with name '{0}'", name);
        return null;
    }

    [ContextMenu("Test Fill Random")]
    public void TestFillRandom()
    {
        string[] buildings = new string[sizeX * sizeY];
        for (int i = 0; i < sizeX*sizeY; i++)
        {
            buildings[i] = _buildingInfos[Random.Range(0, _buildingInfos.Count)].name;
        }
        SetBuildings(buildings);
    }

    [ContextMenu("Test Fill Once")]
    public void TestFillOnce()
    {
        string[] buildings = new string[sizeX * sizeY];
        foreach (var item in _buildingInfos)
        {
            int index = Random.Range(0, sizeX * sizeY);
            buildings[index] = item.name;
        }
        SetBuildings(buildings);
    }
}
