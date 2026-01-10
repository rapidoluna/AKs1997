using UnityEngine;

public class FirePointSettings : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Vector3 offset = new Vector3(0.5f, 2f, 1.5f);

    private WeaponAiming _aiming;

    private void Start()
    {
        _aiming = Object.FindFirstObjectByType<WeaponAiming>();
    }

    private void Update()
    {
        if (playerCamera == null || _aiming == null) return;

        Vector3 currentOffset = _aiming.CurrentOffset;
        transform.position = playerCamera.position +
                             playerCamera.right * currentOffset.x +
                             playerCamera.up * currentOffset.y +
                             playerCamera.forward * currentOffset.z;

        transform.rotation = playerCamera.rotation;
    }
}