using System.Collections.Generic;
using UnityEngine;

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

            // 1. 아이템 체크
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

            // 2. STS//NG 장치 체크
            STSNGStation station = currentHit.GetComponent<STSNGStation>();
            if (station != null)
            {
                _lastTarget = currentHit;

                // 장치가 가동 중이 아닐 때만 메시지 표시
                if (!station.IsProcessing)
                {
                    if (_inventory.Count > 0)
                        InteractHUD.Instance.ShowPrompt("[F] STS//NG에 물자 전송");
                    else
                        InteractHUD.Instance.ShowPrompt("전송할 물자가 없음");
                }
                else
                {
                    // 장치가 이미 가동 중이면 프롬프트를 숨김
                    InteractHUD.Instance.HidePrompt();
                }
                return;
            }
        }

        // 아무것도 바라보지 않을 때
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