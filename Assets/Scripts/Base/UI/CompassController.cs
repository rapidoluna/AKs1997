using UnityEngine;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    public RawImage compassStrip;
    public Transform playerCamera;

    void Update()
    {
        float uvX = playerCamera.eulerAngles.y / 360f;
        compassStrip.uvRect = new Rect(uvX, 0, 1, 1);
    }
}