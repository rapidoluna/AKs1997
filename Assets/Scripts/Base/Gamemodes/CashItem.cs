using UnityEngine;

public class CashItem : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float yOffset = 0.2f;

    public ItemData Data => itemData;
    public Sprite Sprite => Data != null ? Data.itemIcon : null;

    private void Start()
    {
        SnapToGround();
    }

    public void SetData(ItemData data)
    {
        itemData = data;
    }

    public void Collect()
    {
        gameObject.SetActive(false);
    }

    private void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out hit, 5f, groundLayer))
        {
            transform.position = hit.point + Vector3.up * yOffset;
        }
    }
}