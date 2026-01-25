using UnityEngine;

public class ItemInitializer : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    private void Awake()
    {
        if (itemData == null || itemData.modelPrefab == null) return;

        GameObject model = Instantiate(itemData.modelPrefab, transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        if (model.TryGetComponent<Collider>(out var col)) col.enabled = false;

        CashItem cashItem = GetComponent<CashItem>();
        if (cashItem != null) cashItem.SetData(itemData);
    }
}