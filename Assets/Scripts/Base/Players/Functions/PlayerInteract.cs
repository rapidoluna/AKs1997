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
        Camera mainCamera = Camera.main;
        InteractHUD hud = InteractHUD.Instance;
        if (mainCamera == null)
        {
            if (hud != null) hud.HidePrompt();
            return;
        }

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            GameObject hitObject = hit.collider.gameObject;
            string newMessage = GetInteractMessage(hitObject);

            if (hitObject != _currentInteractTarget || newMessage != _lastDisplayedMessage)
            {
                _currentInteractTarget = hitObject;
                _lastDisplayedMessage = newMessage;

                if (hud != null)
                {
                    if (!string.IsNullOrEmpty(newMessage))
                        hud.ShowPrompt(newMessage);
                    else
                        hud.HidePrompt();
                }
            }
        }
        else
        {
            if (_currentInteractTarget != null)
            {
                _currentInteractTarget = null;
                _lastDisplayedMessage = "";
                if (hud != null) hud.HidePrompt();
            }
        }
    }

    private string GetInteractMessage(GameObject obj)
    {
        bool isEscapeReady = GameStateManager.Instance != null && GameStateManager.Instance.IsEscapeReady;

        if (obj.CompareTag("Station"))
        {
            STSNGStation station = obj.GetComponent<STSNGStation>();
            if (station != null)
            {
                if (isEscapeReady)
                {
                    return "탈출이 활성화되었습니다";
                }

                if (station.IsProcessing)
                {
                    return "캐시러시 진행 중";
                }

                return _inventory.Count > 0 ? "[F] STS//NG에 아이템 맡기기" : "소지한 아이템이 없음";
            }
        }

        if (obj.CompareTag("Item"))
        {
            CashItem item = obj.GetComponent<CashItem>();
            if (item != null)
            {
                return _inventory.Count < maxInventorySize ? $"[F] {item.Data.itemName} 획득" : "인벤토리가 가득 참";
            }
        }

        if (obj.CompareTag("Metro"))
        {
            MetroEscape metro = obj.GetComponentInParent<MetroEscape>();
            if (metro != null)
            {
                return isEscapeReady ? "[길게 F] 탈출" : "탈출이 비활성화되었습니다";
            }
        }

        return null;
    }

    private void HandleHoldInteraction()
    {
        bool isEscapeReady = GameStateManager.Instance != null && GameStateManager.Instance.IsEscapeReady;
        InteractHUD hud = InteractHUD.Instance;

        if (_currentInteractTarget != null)
        {
            MetroEscape metro = _currentInteractTarget.GetComponentInParent<MetroEscape>();
            if (metro != null && isEscapeReady && Input.GetKey(KeyCode.F))
            {
                _isHolding = true;
                _holdTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(_holdTimer / holdRequiredTime);
                if (hud != null) hud.UpdateInteractProgress(progress);

                if (_holdTimer >= holdRequiredTime)
                {
                    metro.OnInteractComplete();

                    FirstPersonCamera fpsCam = GetComponent<FirstPersonCamera>();
                    if (fpsCam == null) fpsCam = GetComponentInParent<FirstPersonCamera>();
                    if (fpsCam != null) fpsCam.SetLock(true);

                    PlayerWalking move = GetComponent<PlayerWalking>();
                    if (move != null) move.enabled = false;

                    ResetHold();
                    if (hud != null) hud.HidePrompt();
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
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            CashItem itemScript = hit.collider.GetComponent<CashItem>();
            if (itemScript != null && _inventory.Count < maxInventorySize)
            {
                _inventory.Add(itemScript.Data);
                if (ItemInventoryUI.Instance != null) ItemInventoryUI.Instance.AddItemIcon(itemScript.Data.itemIcon);
                if (CashRushHUD.Instance != null) CashRushHUD.Instance.ShowNotification($"{itemScript.Data.itemName} 획득");
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
