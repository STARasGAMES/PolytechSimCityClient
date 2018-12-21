using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scriptables.Data
{
    [CreateAssetMenu(menuName = "GAME/Tile Data")]
    public class TileData : ScriptableObject
    {
        [SerializeField] string _name;
        [SerializeField] string _description;

        [SerializeField] string _serverName;
        [SerializeField] TileBase _tile;

        [SerializeField] int _maxCount;

        public string Name => _name;
        public string Description  => _description;
        public string ServerName => _serverName;
        public TileBase Tile => _tile;

        public int MaxCount { get => _maxCount; set => _maxCount = value; }
    }
}
