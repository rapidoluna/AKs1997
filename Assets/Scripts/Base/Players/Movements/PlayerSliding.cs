using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    private CharacterController controller;
    private PlayerWalking walking;

    [SerializeField] private float slideSpeedMultiplier = 2.2f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideHeight = 1.5f;
    [SerializeField] private float normalHeight = 2f;
    [SerializeField] private float lerpSpeed = 15f;

    private bool isSliding;
    private float slideTimer;

    public bool IsSliding => isSliding;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        walking = GetComponent<PlayerWalking>();
    }

    void Update()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0;

        if (isRunning && Input.GetKeyDown(KeyCode.LeftControl) && !isSliding)
        {
            StartSlide();
        }

        if (isSliding)
        {
            SlideUpdate();
        }
        else if (isSliding == false && controller.height < normalHeight - 0.01f && !Input.GetKey(KeyCode.LeftControl))
        {
            ResetHeight();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;
        walking.MoveSpeed = walking.BaseSpeed * slideSpeedMultiplier;
    }

    private void SlideUpdate()
    {
        slideTimer -= Time.deltaTime;

        float lastHeight = controller.height;
        controller.height = Mathf.Lerp(controller.height, slideHeight, lerpSpeed * Time.deltaTime);
        controller.center += new Vector3(0, (lastHeight - controller.height) / 2, 0);

        if (slideTimer <= 0)
        {
            isSliding = false;
        }
    }

    private void ResetHeight()
    {
        float lastHeight = controller.height;
        controller.height = Mathf.Lerp(controller.height, normalHeight, lerpSpeed * Time.deltaTime);
        controller.center += new Vector3(0, (lastHeight - controller.height) / 2, 0);
    }
}