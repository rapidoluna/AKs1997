using UnityEngine;

public class PlayerRunning : MonoBehaviour
{
    private PlayerWalking walking;
    [SerializeField] private float runMultiplier = 1.6f;

    void Awake()
    {
        walking = GetComponent<PlayerWalking>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0 && !Input.GetKey(KeyCode.LeftControl))
        {
            walking.MoveSpeed = walking.BaseSpeed * runMultiplier;
        }
        else if (!Input.GetKey(KeyCode.LeftControl))
        {
            walking.MoveSpeed = walking.BaseSpeed;
        }
    }
}