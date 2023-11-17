using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
 
    public static InputComponent Instance;

    public delegate void VectorEvent(Vector3 value);
    public delegate void SingleEvent();
    public delegate void IntEvent(float value);

    public VectorEvent movementTrigger;
    public IntEvent cameraRotationTrigger;
    public SingleEvent interactTrigger;
    public SingleEvent pauseTrigger;
    private UserInputs inputActions;


    private void Awake()
    {
        Instance = this;
        inputActions = new UserInputs();
        inputActions.Enable();
    }

    private void Start()
    {
        inputActions.Gameplay.Interact.performed += _ => interactTrigger?.Invoke();
        inputActions.Gameplay.CameraControl.performed += _ => cameraRotationTrigger?.Invoke(inputActions.Gameplay.CameraControl.ReadValue<float>());
        inputActions.Gameplay.Pause.performed += _ => pauseTrigger?.Invoke() ;
    }

    private void Update()
    {
        movementTrigger?.Invoke(inputActions.Gameplay.Movement.ReadValue<Vector3>());
        
    }
}
