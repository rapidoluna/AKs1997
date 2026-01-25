using UnityEngine;
using System.Collections;

public class PlayerDeathCamera : MonoBehaviour
{
    public static PlayerDeathCamera Instance;

    [SerializeField] private float fallDuration = 1.5f;
    [SerializeField] private float targetHeight = 0.2f;
    [SerializeField] private Vector3 fallRotation = new Vector3(5, -10, 10f);

    private void Awake()
    {
        Instance = this;
    }

    public void PlayDeathAnimation()
    {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        Vector3 endPos = new Vector3(startPos.x, targetHeight, startPos.z);
        Quaternion endRot = startRot * Quaternion.Euler(fallRotation);

        float elapsed = 0;

        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fallDuration;

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }
    }
}