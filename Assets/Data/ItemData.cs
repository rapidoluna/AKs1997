using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "AKs97/ItemData")]
public class ItemData : ScriptableObject
{
    //이름
    public string itemName;
    //캐시러시 후 획득 점수(가치)
    public int cashValue;
    //아이콘
    public Sprite itemIcon;
    //외형 프리펩
    public GameObject modelPrefab;
}