using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRange = 3.5f;
    [SerializeField] private int maxInventorySize = 3;
    [SerializeField] private float holdRequiredTime = 3.0f;

    private List<ItemData> _inventory = new List<ItemData>();
    private GameObject _lastTarget;
    private float _holdTimer = 0f;
    private bool _isHolding = false;

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryInteract();
        }

        HandleHoldInteraction();
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
                    InteractHUD.Instance.ShowPrompt($"[F] {item.Data.itemName} 획득");
                else
                    InteractHUD.Instance.ShowPrompt("인벤토리 가득 참");
                return;
            }

            STSNGStation station = currentHit.GetComponent<STSNGStation>();
            if (station != null)
            {
                _lastTarget = currentHit;
                if (!station.IsProcessing)
                {
                    if (_inventory.Count > 0)
                        InteractHUD.Instance.ShowPrompt("[F] STS//NG에 물자 전송");
                    else
                        InteractHUD.Instance.ShowPrompt("전송할 물자가 없음");
                }
                else
                    InteractHUD.Instance.HidePrompt();
                return;
            }

            MetroEscape metro = currentHit.GetComponentInParent<MetroEscape>();
            if (metro != null)
            {
                _lastTarget = currentHit;
                if (GameStateManager.Instance != null && GameStateManager.Instance.IsEscapeReady)
                    InteractHUD.Instance.ShowPrompt("Hold [F] Metro를 통해 탈출");
                else
                    InteractHUD.Instance.ShowPrompt("현재 탈출 불가능");
                return;
            }
        }

        _lastTarget = null;
        InteractHUD.Instance.HidePrompt();
    }

    private void HandleHoldInteraction()
    {
        if (_lastTarget != null)
        {
            MetroEscape metro = _lastTarget.GetComponentInParent<MetroEscape>();
            if (metro != null && GameStateManager.Instance.IsEscapeReady && Input.GetKey(KeyCode.F))
            {
                _isHolding = true;
                _holdTimer += Time.deltaTime;

                float progress = Mathf.Clamp01(_holdTimer / holdRequiredTime);

                if (InteractHUD.Instance != null)
                    InteractHUD.Instance.UpdateInteractProgress(progress);

                if (_holdTimer >= holdRequiredTime)
                {
                    metro.OnInteractComplete();
                    ResetHold();

                    this.enabled = false;
                }
                return;
            }
        }

        if (Input.GetKeyUp(KeyCode.F) || _isHolding)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        _holdTimer = 0f;
        _isHolding = false;
        if (InteractHUD.Instance != null)
            InteractHUD.Instance.UpdateInteractProgress(0f);
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
                    CashRushHUD.Instance.ShowNotification($"{itemScript.Data.itemName} 획득");

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