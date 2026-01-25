using UnityEngine;

public class CashItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData Data => itemData;
    public Sprite Sprite => Data != null ? Data.itemIcon : null;

    public void SetData(ItemData data)
    {
        itemData = data;
    }

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}