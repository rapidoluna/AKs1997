using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "AKs97/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int cashValue;
    public Sprite icon;
    public GameObject modelPrefab;
}