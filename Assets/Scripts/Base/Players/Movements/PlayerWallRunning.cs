using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerWallRunning : MonoBehaviour
{
    public LayerMask wallLayer;
    public float wallCheckDistance = 1.0f;
    public float stickToWallForce = 3f;
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 12f;
    public float maxWallRunTime = 2.5f;

    public bool IsWallRunning { get; private set; }

    private CharacterController controller;
    private Vector3 currentVelocity;
    private Vector3 lastWallNormal;
    private float wallRunTimer;
    private float wallJumpCooldown;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (wallJumpCooldown > 0f)
        {
            wallJumpCooldown -= Time.deltaTime;
        }

        CheckForWallRun();

        if (IsWallRunning)
        {
            ExecuteWallRun();
        }
    }

    private void CheckForWallRun()
    {
        bool hasWallLeft = Physics.Raycast(transform.position, -transform.right, out RaycastHit leftHit, wallCheckDistance, wallLayer);
        bool hasWallRight = Physics.Raycast(transform.position, transform.right, out RaycastHit rightHit, wallCheckDistance, wallLayer);

        bool isTryingToWallRun = !controller.isGrounded && Input.GetAxisRaw("Vertical") > 0;

        if (isTryingToWallRun && (hasWallLeft || hasWallRight))
        {
            Vector3 wallNormal = hasWallLeft ? leftHit.normal : rightHit.normal;

            if (wallJumpCooldown > 0f && Vector3.Dot(wallNormal, lastWallNormal) > 0.9f)
            {
                return;
            }

            if (!IsWallRunning)
            {
                StartWallRun(wallNormal);
            }
        }
        else
        {
            if (IsWallRunning)
            {
                StopWallRun();
            }
        }
    }

    private void StartWallRun(Vector3 wallNormal)
    {
        IsWallRunning = true;
        lastWallNormal = wallNormal;
        wallRunTimer = 0f;

        currentVelocity = controller.velocity;
        currentVelocity.y = 0f;
    }

    private void ExecuteWallRun()
    {
        wallRunTimer += Time.deltaTime;

        Vector3 wallForward = Vector3.Cross(lastWallNormal, Vector3.up);

        if (Vector3.Dot(transform.forward, wallForward) < 0)
        {
            wallForward = -wallForward;
        }

        float currentHorizontalSpeed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
        currentVelocity = wallForward * currentHorizontalSpeed;

        currentVelocity -= lastWallNormal * stickToWallForce;

        if (wallRunTimer > maxWallRunTime * 0.5f)
        {
            currentVelocity.y -= (wallRunTimer * 4f) * Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            WallJump();
            return;
        }

        controller.Move(currentVelocity * Time.deltaTime);
    }

    private void WallJump()
    {
        IsWallRunning = false;
        wallJumpCooldown = 0.25f;

        float currentSpeed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;
        currentVelocity = (lastWallNormal * wallJumpSideForce) + (Vector3.up * wallJumpUpForce) + (transform.forward * currentSpeed * 0.5f);

        controller.Move(currentVelocity * Time.deltaTime);
    }

    private void StopWallRun()
    {
        IsWallRunning = false;
    }
}