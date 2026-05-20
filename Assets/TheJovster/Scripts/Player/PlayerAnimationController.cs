using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;

    [Header("Smoothing")]
    [SerializeField] private float _speedDampTime = 0.1f;

    private Animator _animator;
    private static readonly int _speedHash = Animator.StringToHash("Speed");
    private static readonly int _jumpHash = Animator.StringToHash("Jump");
    private static readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");

    private bool _wasGrounded = true;

    private void Awake()
    {
        if(_playerController == null) 
        {
            _playerController = GetComponentInParent<PlayerController>();
        }
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_playerController == null) return;

        Vector3 vel = _playerController.CurrentVelocity;
        float horizontalSpeed = new Vector2(vel.x, vel.z).magnitude;

        _animator.SetFloat(_speedHash, horizontalSpeed, _speedDampTime, Time.deltaTime);
        _animator.SetBool(_isGroundedHash, _playerController.IsGrounded);

        if (_wasGrounded && !_playerController.IsGrounded && vel.y > 0f)
        {
            _animator.SetTrigger(_jumpHash);
        }

        _wasGrounded = _playerController.IsGrounded;
    }
}