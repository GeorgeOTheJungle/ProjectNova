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
    [SerializeField] private GameObject uiVisual;
    [Space(10)]
    [SerializeField] private CinemachineVirtualCamera explorationVCamera;
    [SerializeField] private CinemachineVirtualCamera combatVCamera;
    [SerializeField] private CinemachineVirtualCamera playerIdleCombatVCamera;
    [SerializeField] private CinemachineVirtualCamera enemyIdleCombatVCamera;

    private CinemachineOrbitalTransposer orbitalTransposer;
    private CinemachineOrbitalTransposer playerIdleOrbitalTransposer;
    private CinemachineOrbitalTransposer enemyOrbitalTransposer;

    private CinemachineOrbitalTransposer currentOrbital;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float idleTime = 3.0f; // Time before entering orbit mode
    private float currentIdleTime = 0.0f;
    [Space(5)]
    [SerializeField] private float orbitIdleTime = 15.0f; // Time before changing to another entity
    private float currentOrbitIdleTime = 0.0f;
    [SerializeField] private float minIdleOrbitSpeed = -3.0f;
    [SerializeField] private float maxIdleOrbitSpeed = 3.0f;
    [Space(10)]
    [SerializeField] private float zoomInTime = 2.5f;
    [SerializeField] private LeanTweenType zoomEase;
    private bool isPlayerFocus = false;
    private float currentOrbitAmount = 0.0f;

    Vector3 initialCombatPosition;
    Vector3 targetCombatPosition;
    private void Awake()
    {
        orbitalTransposer = explorationVCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        playerIdleOrbitalTransposer = playerIdleCombatVCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        enemyOrbitalTransposer = enemyIdleCombatVCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    private IEnumerator Start()
    {
        ChangeToCamera(Constants.EXPLORATION_CAMERA);
        isPlayerFocus = Random.Range(0, 2) == 0;

        initialCombatPosition = combatVCamera.transform.position;
        targetCombatPosition = combatVCamera.transform.position;
        targetCombatPosition.z += 3;

        if (explorationVCamera.m_Follow == null)
        {
            Transform follow = GameObject.Find("Player_Exploration").GetComponent<Transform>(); 
            explorationVCamera.m_Follow = follow;
            explorationVCamera.m_LookAt = follow;
        }

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

    private void Update()
    {
        if (CombatManager.Instance.entityTurn != CombatTurn.playerTurn) return;

        // Check if any key is being pressed, if yes, then reset timer and everything else.
        if (Input.anyKey)
        {
            currentIdleTime = 0.0f;
            currentOrbitAmount = 0.0f;
            currentOrbitIdleTime = 0.0f;
            ChangeToCamera(Constants.COMBAT_CAMERA);
            currentOrbital = null;
            isPlayerFocus = Random.Range(0, 2) == 0;
            uiVisual.SetActive(true);
        }

        // If timer is more than the designated time, then set an orbit timer

        if(currentIdleTime < idleTime) currentIdleTime += Time.deltaTime;
        else
        {
            if (currentOrbital == null)
            {
                GetIdleOrbit();
                uiVisual.SetActive(false);
            }
            else
            {
                currentOrbitAmount -= Time.deltaTime * rotationSpeed;
                currentOrbital.m_XAxis.Value = currentOrbitAmount;


                if (currentOrbitIdleTime < orbitIdleTime) currentOrbitIdleTime += Time.deltaTime;
                else
                {
                    currentOrbitIdleTime = 0.0f;
                    GetIdleOrbit();
                }
            }
        }

    }

    private void GetIdleOrbit()
    {
        isPlayerFocus = !isPlayerFocus;
        ChangeToCamera(isPlayerFocus?Constants.PLAYER_ORBIT_CAMERA:Constants.ENEMY_ORBIT_CAMERA);
        currentOrbital = isPlayerFocus ? playerIdleOrbitalTransposer : enemyOrbitalTransposer;

        rotationSpeed = Mathf.CeilToInt(Random.Range(minIdleOrbitSpeed, maxIdleOrbitSpeed));
        if (rotationSpeed == 0) rotationSpeed = 1;
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
        
        switch (gameState)
        {
            case GameState.exploration:
                ChangeToCamera(Constants.EXPLORATION_CAMERA);
                combatVCamera.transform.position = initialCombatPosition;
                break;
            case GameState.combatPreparation:
                ChangeToCamera(Constants.COMBAT_CAMERA);
                break;

            case GameState.combatReady:
                LeanTween.move(combatVCamera.gameObject, targetCombatPosition, zoomInTime).setEase(zoomEase);
                break;

        }
    }

    private void ChangeToCamera(string targetCamera)
    {
        explorationVCamera.Priority = 0;
        combatVCamera.Priority = 0;

        playerIdleCombatVCamera.Priority=0;
        enemyIdleCombatVCamera.Priority =0;

        switch (targetCamera)
        {
            case "Exploration":
                explorationVCamera.Priority = 1;
                break;
            case "Combat":
                combatVCamera.Priority = 1;
                break;
            case "OrbitPlayer":
                playerIdleCombatVCamera.Priority = 1;
                break;
            case "OrbitEnemy":
                enemyIdleCombatVCamera.Priority = 1;
                break;
        }
    }
}
