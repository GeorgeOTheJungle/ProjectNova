using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public static CharacterMovement Instance;
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private Transform spawnPoint;
    private Vector3 input;
    private CharacterController m_characterController;

    [SerializeField] private float gravityMultiplier = 3.0f;
    private float _gravity = -9.81f;
    private float _velocity;

    [SerializeField] private Transform cam;
    [SerializeField] private Transform visual;
    [SerializeField] private float hitDetection = 1.0f;
    [SerializeField] private LayerMask layerMask;

    Vector3 forwardRelative;
    Vector3 rightRelative;
    Vector3 movementDirection;
    private PlayerAnimatorController m_animatorController;

    private float lastMovement = 0;
    private bool isFacingRight = true;
    private void Awake()
    {
        Instance = this;
        m_characterController = GetComponent<CharacterController>();
        m_animatorController = GetComponent<PlayerAnimatorController>();
        cam = Camera.main.transform;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        InputComponent.Instance.movementTrigger += HandleMovementInput;
    }

    private void OnEnable()
    {
        if (!InputComponent.Instance) return;
        InputComponent.Instance.movementTrigger += HandleMovementInput;
    }

    private void OnDisable()
    {
        InputComponent.Instance.movementTrigger -= HandleMovementInput;
    }

    private void HandleMovementInput(Vector3 value)
    {
        // Check if we are in exploration
        if (GameManager.Instance._gameState != Enums.GameState.exploration) return;
        // Input
        input = new Vector3(value.x, input.y, value.z);
        input.Normalize();

        // Camera Direction
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0.0f;
        camRight.y = 0.0f;

        // Create relative camera direction
        forwardRelative = input.z * camForward;
        rightRelative = input.x * camRight;

         movementDirection = forwardRelative + rightRelative;

        // Gravity
        if (m_characterController.isGrounded && _velocity <= 0.0f) _velocity = -1f;
        else
        {
            _velocity += _gravity * gravityMultiplier * Time.deltaTime;
            movementDirection.y = _velocity;
        }

 
        movementDirection.Normalize();

        if (movementDirection.z != 0) lastMovement = movementDirection.z;

        // Animations
        m_animatorController.SetIdleAnimation(lastMovement);
        if (input.z != 0.0f || input.x != 0.0f) m_animatorController.SetRunAnimation(true, input.z);
        else m_animatorController.SetRunAnimation(false, input.z);

        // Movement
        if (NearEdge()) return;
        m_characterController.Move(movementDirection * movementSpeed * Time.deltaTime);
    }

    private void SetRespawn(Transform spawn)
    {
        spawnPoint = spawn;
    }

    public void HandleRespawn()
    {
        // Respawn Player in nearest sanctuary;
        if(spawnPoint == null) return;
        m_characterController.Move(spawnPoint.position);
       // transform.position = spawnPoint.position;
    }

    public void MovePlayerToSpawn(Transform spawn)
    {
        SetRespawn(spawn);
        if (spawnPoint == null) return;
        m_characterController.enabled = false;
        Vector3 newPosition = spawnPoint.position;
        newPosition.y = transform.position.y;
        //m_characterController.Move(spawnPoint.position);
        transform.position = newPosition;
        m_characterController.enabled = true;
    }

    private void FixedUpdate()
    {
        if (input != Vector3.zero) FlipCheck();
    }

    private bool NearEdge()
    {
        RaycastHit hit;
        Vector3 facingDir = new Vector3(0f + rightRelative.x/3, 0f, 0f + forwardRelative.z/3);
        if (Physics.Raycast(transform.position + facingDir, transform.TransformDirection(Vector3.down), out hit, hitDetection, layerMask))
        {
            Debug.DrawRay(transform.position + facingDir, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
  
            return false;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitDetection, Color.white);

            return true;
        }
    }

    private void FlipCheck()
    {
        if (input.x > 0.0f && isFacingRight == false) Flip();
        else if(input.x < 0.0f && isFacingRight == true) Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 temp = visual.localScale;
        if (isFacingRight)
        {
            temp.x = 1;
    
        }
        else temp.x = -1;

        visual.localScale = temp;
    }
}
