using UnityEngine;
using UnityEngine.UI;

public class InteractHUD : MonoBehaviour
{
    public static InteractHUD Instance;

    [SerializeField] private GameObject interactPanel;
    [SerializeField] private Text interactText;

    private void Awake()
    {
        Instance = this;
        if (interactPanel != null) interactPanel.SetActive(false);
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
        if (interactPanel != null) interactPanel.SetActive(false);
    }
}