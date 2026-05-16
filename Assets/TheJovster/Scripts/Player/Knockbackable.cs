using UnityEngine;

public class Knockbackable : MonoBehaviour
{
    [SerializeField] private float _knockbackForce = 5.0f;
    [SerializeField] private float _knockbackDuration = 0.2f;

    private CharacterController _characterController = null;

    private bool _isKnockedBack = false;
    private float _knockbackTimer = 0.0f;
    private Vector3 _knockbackDirection = Vector3.zero;
    private float _currentKnockbackForce = 0.0f;

    public bool IsKnockedBack => _isKnockedBack;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!_isKnockedBack)
        {
            return;
        }

        if (_characterController == null || !_characterController.enabled)
        {
            _isKnockedBack = false;
            return;
        }

        Vector3 movement = _knockbackDirection * _currentKnockbackForce * Time.deltaTime;
        _characterController.Move(movement);

        _knockbackTimer -= Time.deltaTime;

        if (_knockbackTimer <= 0.0f)
        {
            _isKnockedBack = false;
        }
    }

    public void ApplyKnockback(Vector3 sourcePosition)
    {
        if (_characterController == null || !_characterController.enabled)
        {
            return;
        }

        _knockbackDirection = transform.position - sourcePosition;
        _knockbackDirection.y = 0.0f;
        _knockbackDirection.Normalize();

        _isKnockedBack = true;
        _knockbackTimer = _knockbackDuration;
        _currentKnockbackForce = _knockbackForce;
    }

    public void ApplyKnockback(Vector3 direction, float forceMultiplier)
    {
        if (_characterController == null || !_characterController.enabled)
        {
            return;
        }

        _knockbackDirection = direction;
        _knockbackDirection.y = 0.0f;
        _knockbackDirection.Normalize();

        _isKnockedBack = true;
        _knockbackTimer = _knockbackDuration;
        _currentKnockbackForce = _knockbackForce * forceMultiplier;
    }
}