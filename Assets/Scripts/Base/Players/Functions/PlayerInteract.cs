using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 3.5f;
    [SerializeField] private int maxInventorySize = 3;
    private List<ItemData> _inventory = new List<ItemData>();
    private GameObject _lastTarget;

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryInteract();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            GameObject currentHit = hit.collider.gameObject;

            CashItem item = currentHit.GetComponent<CashItem>();
            if (item != null)
            {
                _lastTarget = currentHit;
                if (_inventory.Count < maxInventorySize)
                {
                    InteractHUD.Instance.ShowPrompt($"[F] {item.Data.itemName} È¹µæ");
                }
                else
                    InteractHUD.Instance.ShowPrompt("ÀÎº¥Åä¸® °¡µæ Âü");
                return;
            }

            STSNGStation station = currentHit.GetComponent<STSNGStation>();
            if (station != null)
            {
                _lastTarget = currentHit;

                if (!station.IsProcessing)
                {
                    if (_inventory.Count > 0)
                        InteractHUD.Instance.ShowPrompt("[F] STS//NG¿¡ ¹°ÀÚ Àü¼Û");
                    else
                        InteractHUD.Instance.ShowPrompt("Àü¼ÛÇÒ ¹°ÀÚ°¡ ¾øÀ½");
                }
                else
                {
                    InteractHUD.Instance.HidePrompt();
                }
                return;
            }
        }

        _lastTarget = null;
        InteractHUD.Instance.HidePrompt();
    }

    private void TryInteract()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            CashItem itemScript = hit.collider.GetComponent<CashItem>();
            if (itemScript != null && _inventory.Count < maxInventorySize)
            {
                _inventory.Add(itemScript.Data);

                if (ItemInventoryUI.Instance != null)
                    ItemInventoryUI.Instance.AddItemIcon(itemScript.Data.itemIcon);

                if (CashRushHUD.Instance != null)
                    CashRushHUD.Instance.ShowNotification($"{itemScript.Data.itemName} È¹µæ");

                itemScript.Collect();
                return;
            }

            STSNGStation station = hit.collider.GetComponent<STSNGStation>();
            if (station != null && _inventory.Count > 0 && station.CanAcceptItems(_inventory.Count))
            {
                station.DepositItems(new List<ItemData>(_inventory));
                _inventory.Clear();
            }
        }
    }
}