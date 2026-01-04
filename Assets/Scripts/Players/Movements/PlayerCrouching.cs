using UnityEngine;

public class PlayerCrouching : MonoBehaviour
{
    private CharacterController controller;
    private PlayerWalking walking;
    private PlayerSliding sliding;

    [SerializeField] private float crouchHeight = 1.5f;
    [SerializeField] private float normalHeight = 2f;
    [SerializeField] private float crouchSpeedMultiplier = 0.5f;
    [SerializeField] private float lerpSpeed = 15f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        walking = GetComponent<PlayerWalking>();
        sliding = GetComponent<PlayerSliding>();
    }

    void Update()
    {
        if (sliding != null && sliding.IsSliding) return;

        float targetHeight = Input.GetKey(KeyCode.LeftControl) ? crouchHeight : normalHeight;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            walking.MoveSpeed = walking.BaseSpeed * crouchSpeedMultiplier;
        }

        float lastHeight = controller.height;
        controller.height = Mathf.Lerp(controller.height, targetHeight, lerpSpeed * Time.deltaTime);
        controller.center += new Vector3(0, (lastHeight - controller.height) / 2, 0);
    }
}