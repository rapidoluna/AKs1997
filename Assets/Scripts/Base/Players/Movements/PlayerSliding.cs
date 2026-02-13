using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    public CharacterController controller;
    public MonoBehaviour[] scriptsToDisable;

    public float initialSlideSpeed = 18f;
    public float slideFriction = 7f;
    public float minSlideSpeed = 6f;
    public float slideHeight = 0.5f;
    public float defaultGravity = -19.62f;

    public bool IsSliding { get; private set; }

    private float currentSpeed;
    private float originalHeight;
    private Vector3 slideDirection;

    private void Start()
    {
        if (controller != null) originalHeight = controller.height;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && controller.isGrounded && !IsSliding)
        {
            StartSlide();
        }

        if (IsSliding)
        {
            currentSpeed -= slideFriction * Time.deltaTime;

            Vector3 moveVelocity = slideDirection * currentSpeed;
            moveVelocity.y = defaultGravity;

            controller.Move(moveVelocity * Time.deltaTime);

            if (Input.GetKeyUp(KeyCode.LeftControl) || currentSpeed <= minSlideSpeed)
            {
                StopSlide();
            }
        }
    }

    private void StartSlide()
    {
        IsSliding = true;
        currentSpeed = initialSlideSpeed;
        slideDirection = transform.forward;
        controller.height = slideHeight;

        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }
    }

    private void StopSlide()
    {
        IsSliding = false;
        controller.height = originalHeight;

        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = true;
        }
    }
}