using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private int maxInventorySize = 3;
    [SerializeField] private float holdRequiredTime = 3.0f;

    private List<ItemData> _inventory = new List<ItemData>();
    private GameObject _currentInteractTarget;
    private string _lastDisplayedMessage;
    private float _holdTimer = 0f;
    private bool _isHolding = false;

    void Update()
    {
        CheckForInteractable();
        if (Input.GetKeyDown(KeyCode.F)) TrySingleInteract();
        HandleHoldInteraction();
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            GameObject hitObject = hit.collider.gameObject;
            string newMessage = GetInteractMessage(hitObject);

            if (hitObject != _currentInteractTarget || newMessage != _lastDisplayedMessage)
            {
                _currentInteractTarget = hitObject;
                _lastDisplayedMessage = newMessage;

                if (!string.IsNullOrEmpty(newMessage))
                    InteractHUD.Instance.ShowPrompt(newMessage);
                else
                    InteractHUD.Instance.HidePrompt();
            }
        }
        else
        {
            if (_currentInteractTarget != null)
            {
                _currentInteractTarget = null;
                _lastDisplayedMessage = "";
                InteractHUD.Instance.HidePrompt();
            }
        }
    }

    private string GetInteractMessage(GameObject obj)
    {
        CashItem item = obj.GetComponent<CashItem>();
        if (item != null) return _inventory.Count < maxInventorySize ? $"[F] {item.Data.itemName} È¹µæ" : "ÀÎº¥Åä¸®°¡ °¡µæ Âü";

        STSNGStation station = obj.GetComponent<STSNGStation>();
        if (GameStateManager.Instance != null && GameStateManager.Instance.IsEscapeReady)
            return "Àü¼ÛÀÌ ºñÈ°¼ºÈ­µÊ";
        if (station != null) return _inventory.Count > 0 ? "[F] STS//NG¿¡ ¹°ÀÚ Àü¼Û" : "Àü¼ÛÇÒ ¹°ÀÚ°¡ ¾øÀ½";

        MetroEscape metro = obj.GetComponentInParent<MetroEscape>();
        if (metro != null) return GameStateManager.Instance.IsEscapeReady ? "[È¦µå F] Å»Ãâ" : "Å»ÃâÀÌ ºñÈ°¼ºÈ­µÊ";

        return null;
    }

    private void HandleHoldInteraction()
    {
        if (_currentInteractTarget != null)
        {
            MetroEscape metro = _currentInteractTarget.GetComponentInParent<MetroEscape>();
            if (metro != null && GameStateManager.Instance.IsEscapeReady && Input.GetKey(KeyCode.F))
            {
                _isHolding = true;
                _holdTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(_holdTimer / holdRequiredTime);
                InteractHUD.Instance.UpdateInteractProgress(progress);

                if (_holdTimer >= holdRequiredTime)
                {
                    metro.OnInteractComplete();

                    FirstPersonCamera fpsCam = GetComponent<FirstPersonCamera>();
                    if (fpsCam == null) fpsCam = GetComponentInParent<FirstPersonCamera>();
                    if (fpsCam != null) fpsCam.SetLock(true);

                    PlayerWalking move = GetComponent<PlayerWalking>();
                    if (move != null) move.enabled = false;

                    ResetHold();
                    InteractHUD.Instance.HidePrompt();
                    _currentInteractTarget = null;
                    _lastDisplayedMessage = "";
                    this.enabled = false;
                }
                return;
            }
        }

        if (Input.GetKeyUp(KeyCode.F) || _isHolding) ResetHold();
    }

    private void ResetHold()
    {
        _holdTimer = 0f;
        _isHolding = false;
        if (InteractHUD.Instance != null) InteractHUD.Instance.UpdateInteractProgress(0f);
    }

    private void TrySingleInteract()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            CashItem itemScript = hit.collider.GetComponent<CashItem>();
            if (itemScript != null && _inventory.Count < maxInventorySize)
            {
                _inventory.Add(itemScript.Data);
                if (ItemInventoryUI.Instance != null) ItemInventoryUI.Instance.AddItemIcon(itemScript.Data.itemIcon);
                if (CashRushHUD.Instance != null) CashRushHUD.Instance.ShowNotification($"{itemScript.Data.itemName} È¹µæ");
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