using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _mainCamera;
    void Start() => _mainCamera = Camera.main.transform;
    void LateUpdate() => transform.LookAt(transform.position + _mainCamera.forward);
}