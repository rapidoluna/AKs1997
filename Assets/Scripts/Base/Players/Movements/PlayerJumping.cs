using UnityEngine;

public class PlayerJumping : MonoBehaviour
{
    private CharacterController controller;
    private PlayerWalking walking;

    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravityForCalculation = -19.62f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        walking = GetComponent<PlayerWalking>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            walking.VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravityForCalculation);
        }
    }
}