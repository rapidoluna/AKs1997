using UnityEngine;

public class PlayerWalking : MonoBehaviour
{
    public CharacterData characterData;
    private CharacterController controller;

    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float gravity = -19.62f;

    [Header("Movement Multipliers")]
    [SerializeField] private float adsSpeedMultiplier = 0.5f;
    [SerializeField] private float shootingSpeedMultiplier = 0.5f;

    private float currentSpeed;
    private Vector3 velocity;

    private WeaponAiming _weaponAiming;
    private WeaponShooting _weaponShooting;

    public float MoveSpeed
    {
        get => currentSpeed;
        set => currentSpeed = value;
    }

    public float BaseSpeed => baseSpeed;

    public float VerticalVelocity
    {
        get => velocity.y;
        set => velocity.y = value;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        UpdateReferences();
        HandleMovement();
    }

    private void UpdateReferences()
    {
        if (_weaponAiming == null || _weaponShooting == null)
        {
            _weaponAiming = GetComponentInChildren<WeaponAiming>();
            _weaponShooting = GetComponentInChildren<WeaponShooting>();
        }
    }

    private void HandleMovement()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float targetSpeed = baseSpeed;

        if (_weaponAiming != null && _weaponAiming.IsAiming)
        {
            targetSpeed *= adsSpeedMultiplier;
        }
        else if (_weaponShooting != null && _weaponShooting.IsShooting)
        {
            targetSpeed *= shootingSpeedMultiplier;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}