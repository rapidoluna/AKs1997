using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerWalking : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _controller;
    private Transform _transform;
    private Vector3 _velocity;
    private float _speedMultiplier = 1f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = transform;
    }

    private void Update()
    {
        if (PlayerHealth.IsDead) return;

        bool isGrounded = _controller.isGrounded;
        if (isGrounded && _velocity.y < 0) _velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            Vector3 move = _transform.right * x + _transform.forward * z;
            if (move.magnitude > 1f) move.Normalize();
            _controller.Move(move * (walkSpeed * _speedMultiplier) * Time.deltaTime);
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    public void SetSpeedMultiplier(float multi) => _speedMultiplier = multi;
}