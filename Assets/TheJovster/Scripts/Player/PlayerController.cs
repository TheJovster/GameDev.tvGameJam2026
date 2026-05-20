using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _sprintMultiplier = 1.6f;
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Jump & Gravity")]
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _groundStickForce = -2f;
    [SerializeField] private float _coyoteTime = 0.15f;

    private CharacterController _characterController;
    private Transform _cameraTransform;
    private Vector3 _velocity;
    private Vector2 _moveInput;
    private bool _isSprinting;
    private float _coyoteTimer;
    private bool _hasJumped;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private InputActionReference _jumpAction;
    [SerializeField] private InputActionReference _sprintAction;

    public Vector3 CurrentVelocity => _velocity;
    public bool IsGrounded => _characterController.isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Camera cam = ServiceRegistry.Instance?.MainCamera;
        if (cam != null)
            _cameraTransform = cam.transform;
        else
            _cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        if (_moveAction != null)
        {
            _moveAction.action.Enable();
            _moveAction.action.performed += OnMove;
            _moveAction.action.canceled += OnMove;
        }

        if (_jumpAction != null)
        {
            _jumpAction.action.Enable();
            _jumpAction.action.performed += OnJump;
        }

        if (_sprintAction != null)
        {
            _sprintAction.action.Enable();
            _sprintAction.action.performed += OnSprintStart;
            _sprintAction.action.canceled += OnSprintEnd;
        }
    }

    private void OnDisable()
    {
        if (_moveAction != null)
        {
            _moveAction.action.performed -= OnMove;
            _moveAction.action.canceled -= OnMove;
        }

        if (_jumpAction != null)
        {
            _jumpAction.action.performed -= OnJump;
        }

        if (_sprintAction != null)
        {
            _sprintAction.action.performed -= OnSprintStart;
            _sprintAction.action.canceled -= OnSprintEnd;
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (_coyoteTimer > 0f && !_hasJumped)
        {
            _velocity.y = _jumpForce;
            _hasJumped = true;
            _coyoteTimer = 0f;
        }
    }

    private void OnSprintStart(InputAction.CallbackContext ctx) => _isSprinting = true;
    private void OnSprintEnd(InputAction.CallbackContext ctx) => _isSprinting = false;

    private void Update()
    {
        UpdateCoyoteTimer();
        HandleMovement();
        ApplyGravity();
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void UpdateCoyoteTimer()
    {
        if (_characterController.isGrounded)
        {
            _coyoteTimer = _coyoteTime;
            _hasJumped = false;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * _moveInput.y + camRight * _moveInput.x;

        float speed = _isSprinting ? _moveSpeed * _sprintMultiplier : _moveSpeed;

        _velocity.x = moveDir.x * speed;
        _velocity.z = moveDir.z * speed;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded && _velocity.y < 0f)
        {
            _velocity.y = _groundStickForce;
        }
        else
        {
            _velocity.y += _gravity * Time.deltaTime;
        }
    }
}