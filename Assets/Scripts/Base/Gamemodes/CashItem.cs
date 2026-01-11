using UnityEngine;

public class CashItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData Data => itemData;

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}