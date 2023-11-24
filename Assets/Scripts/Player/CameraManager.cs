using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    [SerializeField] private float orbitTime = 1.0f;
    [SerializeField] private float orbitAmount = 45.0f;
    private bool Rotating = false;
    [SerializeField] private CinemachineVirtualCamera explorationVCamera;
    [SerializeField] private CinemachineVirtualCamera combatVCamera;
    private CinemachineOrbitalTransposer orbitalTransposer;

    private void Awake()
    {
        orbitalTransposer = explorationVCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    private IEnumerator Start()
    {
        combatVCamera.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        InputComponent.Instance.cameraRotationTrigger += HandleCameraRotation;
        GameManager.Instance.onGameStateChangeTrigger += HandleCameraChange;
    }

    private void OnEnable()
    {
        if (InputComponent.Instance == null) return;
        InputComponent.Instance.cameraRotationTrigger += HandleCameraRotation;
        if (GameManager.Instance == null) return;
        GameManager.Instance.onGameStateChangeTrigger += HandleCameraChange;

    }

    private void OnDisable()
    {
        InputComponent.Instance.cameraRotationTrigger -= HandleCameraRotation;
        GameManager.Instance.onGameStateChangeTrigger -= HandleCameraChange;

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

    public void HandleCameraChange(GameState gameState)
    {
        if (gameState == GameState.paused || gameState == GameState.messagePrompt) return;
        explorationVCamera.gameObject.SetActive(false);
        combatVCamera.gameObject.SetActive(false);
        switch (gameState)
        {
            case GameState.exploration:
                explorationVCamera.gameObject.SetActive(true);
                break;
            case GameState.combatPreparation:
                combatVCamera.gameObject.SetActive(true);
                break;
        }
    }
}
