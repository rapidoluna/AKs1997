using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerWallRunning : MonoBehaviour
{
    public LayerMask wallLayer;
    public float wallRunSpeed = 14f;
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 12f;

    public bool IsWallRunning { get; private set; }
    public bool IsWallLeft { get; private set; }
    public bool IsWallRight { get; private set; }

    private CharacterController controller;
    private Vector3 moveVelocity;
    private Vector3 wallNormal;
    private float wallRunTimer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        CheckWall();

        if (!controller.isGrounded && (IsWallLeft || IsWallRight) && Input.GetAxisRaw("Vertical") > 0)
        {
            if (!IsWallRunning) StartWallRun();
            WallRunMovement();
        }
        else if (IsWallRunning)
        {
            StopWallRun();
        }
    }

    private void CheckWall()
    {
        float checkDist = controller.radius + 0.5f;
        IsWallRight = Physics.Raycast(transform.position, transform.right, out RaycastHit rightHit, checkDist, wallLayer);
        IsWallLeft = Physics.Raycast(transform.position, -transform.right, out RaycastHit leftHit, checkDist, wallLayer);

        if (IsWallRight) wallNormal = rightHit.normal;
        else if (IsWallLeft) wallNormal = leftHit.normal;
    }

    private void StartWallRun()
    {
        IsWallRunning = true;
        wallRunTimer = 0f;
        moveVelocity.y = 0f;
    }

    private void WallRunMovement()
    {
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);
        if (Vector3.Dot(transform.forward, wallForward) < 0)
        {
            wallForward = -wallForward;
        }

        wallRunTimer += Time.deltaTime;

        moveVelocity = wallForward * wallRunSpeed;

        moveVelocity += -wallNormal * 4f;

        moveVelocity.y -= (wallRunTimer * 3f) * Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
        {
            moveVelocity = wallNormal * wallJumpSideForce + Vector3.up * wallJumpUpForce + transform.forward * (wallRunSpeed * 0.5f);
            StopWallRun();
        }

        controller.Move(moveVelocity * Time.deltaTime);
    }

    private void StopWallRun()
    {
        IsWallRunning = false;
    }
}