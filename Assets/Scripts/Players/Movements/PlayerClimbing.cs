using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    private CharacterController controller;
    private PlayerWalking walking;

    [SerializeField] private float wallClimbForce = 7f;
    [SerializeField] private float wallCheckDistance = 0.7f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Climb Settings")]
    [SerializeField] private float maxClimbTime = 1.5f;
    private float climbTimer;
    private bool isClimbing;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        walking = GetComponent<PlayerWalking>();
    }

    void Update()
    {
        if (Input.GetButton("Jump") && CheckWall())
        {
            if (climbTimer < maxClimbTime)
            {
                StartClimb();
            }
            else
            {
                StopClimb();
            }
        }
        else
        {
            StopClimb();
        }

        if (controller.isGrounded && !Input.GetButton("Jump"))
        {
            climbTimer = 0f;
        }
    }

    private bool CheckWall()
    {
        return Physics.Raycast(transform.position, transform.forward, wallCheckDistance, wallLayer);
    }

    private void StartClimb()
    {
        isClimbing = true;
        climbTimer += Time.deltaTime;
        walking.VerticalVelocity = wallClimbForce;
    }

    private void StopClimb()
    {
        if (isClimbing)
        {
            isClimbing = false;
        }
    }
}