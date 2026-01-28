using UnityEngine;

public class WeaponRecoilCamera : MonoBehaviour
{
    [SerializeField] private float snappiness = 6f;
    [SerializeField] private float returnSpeed = 8f;

    [SerializeField] private float recoilX = -1f;
    [SerializeField] private float recoilY = 1f;
    [SerializeField] private float recoilZ = 1f;

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

    public Vector3 CurrentRecoilRotation => _currentRotation;

    private void Update()
    {
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, snappiness * Time.fixedDeltaTime);
    }

    public void TriggerRecoil(float multiplier = 1f)
    {
        _targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ)) * multiplier;
    }
}