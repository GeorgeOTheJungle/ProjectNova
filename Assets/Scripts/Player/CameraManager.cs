using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    [SerializeField] private float orbitTime = 1.0f;
    [SerializeField] private float orbitAmount = 45.0f;
    private bool Rotating = false;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineOrbitalTransposer orbitalTransposer;

    private void Awake()
    {
        orbitalTransposer = virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InputComponent.Instance.cameraRotationTrigger += HandleCameraRotation;
    }

    private void OnEnable()
    {
        if (InputComponent.Instance == null) return;
        InputComponent.Instance.cameraRotationTrigger += HandleCameraRotation;
    }

    private void OnDisable()
    {
        InputComponent.Instance.cameraRotationTrigger -= HandleCameraRotation;
    }

    private void HandleCameraRotation(float value)
    {
        if (Rotating) return;
        StartCoroutine(CameraRotationAnimation(value));
    }

    private IEnumerator CameraRotationAnimation(float value)
    {
        Rotating = true;
        float rotationValue = orbitalTransposer.m_XAxis.Value;
        float targetValue;
        float velocity = 0.0f;
        float treshold = 0.2f;
        if(value > 0.0f)
        {
            targetValue = rotationValue - orbitAmount;
            while (rotationValue > targetValue + treshold)
            {
                rotationValue = Mathf.SmoothDamp(rotationValue, targetValue,ref velocity, orbitTime);
                orbitalTransposer.m_XAxis.Value = rotationValue;
                yield return new WaitForEndOfFrame();
            }
        } else
        {
            targetValue = rotationValue + orbitAmount;
            while (rotationValue < targetValue - treshold)
            {
                rotationValue = Mathf.SmoothDamp(rotationValue, targetValue, ref velocity, orbitTime);
                orbitalTransposer.m_XAxis.Value = rotationValue;
                yield return new WaitForEndOfFrame();
            }
        }
        orbitalTransposer.m_XAxis.Value = rotationValue;
        Rotating = false;
    }
}
