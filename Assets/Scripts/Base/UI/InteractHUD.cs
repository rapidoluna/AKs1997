using UnityEngine;
using UnityEngine.UI;

public class InteractHUD : MonoBehaviour
{
    public static InteractHUD Instance;

    [SerializeField] private GameObject interactPanel;
    [SerializeField] private Text interactText;
    [SerializeField] private GameObject interactProgressRoot;
    [SerializeField] private Image progressFillImage;

    private void Awake()
    {
        Instance = this;
        if (interactPanel != null) interactPanel.SetActive(false);
        if (interactProgressRoot != null) interactProgressRoot.SetActive(false);
    }

    public void ShowPrompt(string message)
    {
        if (interactPanel == null || interactText == null) return;
        if (interactPanel.activeSelf && interactText.text == message) return;

        interactText.text = message;
        interactPanel.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactPanel != null && interactPanel.activeSelf)
            interactPanel.SetActive(false);
    }

    public void UpdateInteractProgress(float progress)
    {
        if (interactProgressRoot == null || progressFillImage == null) return;

        if (progress > 0)
        {
            if (!interactProgressRoot.activeSelf) interactProgressRoot.SetActive(true);
            progressFillImage.fillAmount = progress;
        }
        else
        {
            if (interactProgressRoot.activeSelf) interactProgressRoot.SetActive(false);
        }
    }
}