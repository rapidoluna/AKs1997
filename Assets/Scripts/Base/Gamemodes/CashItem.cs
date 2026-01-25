using UnityEngine;

public class CashItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData Data => itemData;
    public Sprite Sprite => Data.itemIcon;

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}