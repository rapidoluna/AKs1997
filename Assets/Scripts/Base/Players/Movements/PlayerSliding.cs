using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    public CharacterController controller;
    public MonoBehaviour[] scriptsToDisable;

    public float initialSlideSpeed = 14f;
    public float slideFriction = 6f;
    public float minSlideSpeed = 7f;
    public float slideHeight = 1.7f;
    public float defaultGravity = -19.62f;

    public bool IsSliding { get; private set; }

    private float currentSpeed;
    private float originalHeight;
    private Vector3 slideDirection;
    private PlayerWalking walking;

    private void Awake()
    {
        walking = GetComponent<PlayerWalking>();
    }

    private void Start()
    {
        if (controller != null) originalHeight = controller.height;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = (transform.right * horizontal + transform.forward * vertical).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && vertical > 0;

        if (Input.GetKeyDown(KeyCode.LeftControl) && controller.isGrounded && !IsSliding && isRunning && inputDir.magnitude > 0.1f)
        {
            StartSlide(inputDir);
        }

        if (IsSliding)
        {
            currentSpeed -= slideFriction * Time.deltaTime;

            Vector3 moveVelocity = slideDirection * currentSpeed;
            moveVelocity.y += defaultGravity * Time.deltaTime;

            controller.Move(moveVelocity * Time.deltaTime);

            if (Input.GetKeyUp(KeyCode.LeftControl) || currentSpeed <= minSlideSpeed)
            {
                StopSlide();
            }
        }
    }

    private void StartSlide(Vector3 direction)
    {
        IsSliding = true;
        currentSpeed = initialSlideSpeed;
        slideDirection = direction;
        controller.height = slideHeight;

        if (walking != null) walking.MoveSpeed = 0;

        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }
    }

    private void StopSlide()
    {
        IsSliding = false;
        controller.height = originalHeight;

        if (walking != null)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0;
            walking.MoveSpeed = isRunning ? walking.BaseSpeed * 1.6f : walking.BaseSpeed;
        }

        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = true;
        }
    }
}