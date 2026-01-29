using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Camera mainCamera;

    private WeaponRecoilCamera _recoilSystem;
    private float xRotation = 0f;
    private bool _isLocked = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (mainCamera != null)
            _recoilSystem = mainCamera.GetComponent<WeaponRecoilCamera>();
    }

    void Update()
    {
        if (_isLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Vector3 recoil = _recoilSystem != null ? _recoilSystem.CurrentRecoilRotation : Vector3.zero;

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation + recoil.x, recoil.y, recoil.z);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void SetLock(bool isLocked)
    {
        _isLocked = isLocked;
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}