using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private Transform player;
    private Vector3 targetPos;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public float duration = 2.5f;
    public float radius = 350f;
    private float timer;

    public void SetTarget(Vector3 attackerPos, Transform playerTransform)
    {
        targetPos = attackerPos;
        player = playerTransform;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        timer = duration;
        UpdatePosition();
    }

    private void Update()
    {
        if (player == null) { Destroy(gameObject); return; }
        UpdatePosition();
        timer -= Time.deltaTime;
        canvasGroup.alpha = timer / duration;
        if (timer <= 0) Destroy(gameObject);
    }

    private void UpdatePosition()
    {
        Vector3 directionToTarget = (targetPos - player.position).normalized;
        directionToTarget.y = 0;
        Vector3 playerForward = player.forward;
        playerForward.y = 0;
        float angle = Vector3.SignedAngle(playerForward, directionToTarget, Vector3.up);
        rectTransform.localPosition = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad) * radius, Mathf.Cos(angle * Mathf.Deg2Rad) * radius, 0);
        rectTransform.localRotation = Quaternion.Euler(0, 0, -angle);
    }
}