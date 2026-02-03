using UnityEngine;

public enum availableModes
{
    cash,
    pvp
}

[CreateAssetMenu(fileName = "MapData", menuName = "AKs97/MapData")]
public class MapData : ScriptableObject
{
    public availableModes gameMode;

    public string mapName;
    public GameObject mapPrefab;
    public Sprite mapImage;
}
