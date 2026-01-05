using UnityEngine;

public class PlayerWalking : MonoBehaviour
{
    public CharacterData characterData;
    private CharacterController controller;

    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float gravity = -19.62f;

    private float currentSpeed;
    private Vector3 velocity;

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
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}