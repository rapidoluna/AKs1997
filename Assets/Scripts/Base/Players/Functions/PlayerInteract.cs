using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 5f;
    private List<ItemData> _inventory = new List<ItemData>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            CashItem itemScript = hit.collider.GetComponent<CashItem>();
            if (itemScript != null)
            {
                _inventory.Add(itemScript.Data);
                if (CashRushHUD.Instance != null)
                {
                    CashRushHUD.Instance.ShowNotification($"{itemScript.Data.itemName} È¹µæ");
                }
                itemScript.Collect();
                return;
            }

            STSNGStation station = hit.collider.GetComponent<STSNGStation>();
            if (station != null && _inventory.Count > 0)
            {
                station.DepositItems(_inventory);
                _inventory.Clear();
            }
        }
    }
}