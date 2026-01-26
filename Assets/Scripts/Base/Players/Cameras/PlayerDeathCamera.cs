using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerDeathCamera : MonoBehaviour
{
    public static PlayerDeathCamera Instance;

    [SerializeField] private float fallDuration = 1.25f;//사망 애니메이션 재생 속도
    [SerializeField] private float targetHeight = 0.1f;//사망 시 플레이어 카메라 높이
    [SerializeField] private Vector3 fallRotation = new Vector3(0, -10, 10f);//사망 시 회전

    [SerializeField] private float targetFOV = 75f; //사망 시 도달할 카메라 FOV 값
    [SerializeField] private AnimationCurve fovCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private string resultSceneName = "ResultScene";
    [SerializeField] private float delayAfterFall = 1.0f;

    private Camera _cam;
    private float _startFOV;

    private void Awake()
    {
        Instance = this;
        _cam = GetComponent<Camera>();
        if (_cam != null) _startFOV = _cam.fieldOfView;
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
            float curveT = fovCurve.Evaluate(t);

            // 위치 및 회전 애니메이션
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            // FOV 확대 애니메이션
            if (_cam != null)
            {
                _cam.fieldOfView = Mathf.Lerp(_startFOV, targetFOV, curveT);
            }

            yield return null;
        }

        // 연출 완료 후 대기 및 씬 전환
        yield return new WaitForSeconds(delayAfterFall);

        // 결과 씬에서 마우스 사용을 위해 커서 해제
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(resultSceneName);
    }
}