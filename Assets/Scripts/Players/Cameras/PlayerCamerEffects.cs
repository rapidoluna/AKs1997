using UnityEngine;

public class PlayerCameraEffects : MonoBehaviour
{
    private PlayerWalking walking;
    private CharacterController controller;
    private PlayerSliding sliding;

    [Header("Head Bobbing")]
    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float runBobSpeed = 13f;
    [SerializeField] private float runBobAmount = 0.065f;
    [SerializeField] private float crouchBobSpeed = 7f;
    [SerializeField] private float crouchBobAmount = 0.03f;

    [Header("Landing Impact")]
    [SerializeField] private float landAmount = 0.05f;
    [SerializeField] private float landDuration = 0.05f;

    private float defaultY;
    private float timer;
    private float landTimer;
    private bool wasGrounded;

    void Awake()
    {
        walking = GetComponentInParent<PlayerWalking>();
        controller = GetComponentInParent<CharacterController>();
        sliding = GetComponentInParent<PlayerSliding>();
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        HandleHeadBob();
        HandleLanding();
    }

    private void HandleHeadBob()
    {
        float inputMagnitude = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;

        bool isSliding = sliding != null && sliding.IsSliding;
        bool canBob = inputMagnitude > 0 && controller.isGrounded && !isSliding;

        if (canBob)
        {
            float speed = walkBobSpeed;
            float amount = walkBobAmount;

            if (Input.GetKey(KeyCode.LeftControl))
            {
                speed = crouchBobSpeed;
                amount = crouchBobAmount;
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runBobSpeed;
                amount = runBobAmount;
            }

            timer += Time.deltaTime * speed;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                defaultY + Mathf.Sin(timer) * amount,
                transform.localPosition.z
            );
        }
        else
        {
            timer = 0;
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                Mathf.Lerp(transform.localPosition.y, defaultY, Time.deltaTime * 10f),
                transform.localPosition.z
            );
        }
    }

    private void HandleLanding()
    {
        if (!wasGrounded && controller.isGrounded)
        {
            landTimer = landDuration;
        }

        if (landTimer > 0)
        {
            landTimer -= Time.deltaTime;
            float fallOffset = Mathf.Sin((landTimer / landDuration) * Mathf.PI) * landAmount;
            transform.localPosition -= new Vector3(0, fallOffset, 0);
        }

        wasGrounded = controller.isGrounded;
    }
}