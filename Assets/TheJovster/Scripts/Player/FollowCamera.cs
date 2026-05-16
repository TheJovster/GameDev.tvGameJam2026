using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _boomPivot; // empty child on the player

    [Header("Boom Arm")]
    [SerializeField] private float _distance = 5f;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, 0f);

    [Header("Mouse Look")]
    [SerializeField] private InputActionReference _lookAction;
    [SerializeField] private float _sensitivityX = 2f;
    [SerializeField] private float _sensitivityY = 1.5f;
    [SerializeField] private float _minPitch = -30f;
    [SerializeField] private float _maxPitch = 60f;

    [Header("Smoothing")]
    [SerializeField] private float _positionSmoothing = 8f;
    [SerializeField] private float _rotationSmoothing = 10f;

    private float _yaw;
    private float _pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (_boomPivot == null)
        {
            Debug.LogError("FollowCamera: Assign the boom pivot (empty child on player).");
            enabled = false;
            return;
        }

        // Init angles from current rotation
        Vector3 euler = transform.eulerAngles;
        _yaw = euler.y;
        _pitch = euler.x;
    }

    private void OnEnable()
    {
        if (_lookAction != null)
            _lookAction.action.Enable();
    }

    private void OnDisable()
    {
        if (_lookAction != null)
            _lookAction.action.Disable();
    }

    private void LateUpdate()
    {
        ReadLookInput();

        Vector3 pivotPos = _boomPivot.position + _offset;

        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 targetPosition = pivotPos - (targetRotation * Vector3.forward * _distance);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothing * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetPosition, _positionSmoothing * Time.deltaTime);
    }

    private void ReadLookInput()
    {
        if (_lookAction == null) return;

        Vector2 delta = _lookAction.action.ReadValue<Vector2>();
        _yaw += delta.x * _sensitivityX;
        _pitch -= delta.y * _sensitivityY;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
    }

    private void OnDrawGizmos()
    {
        if (_boomPivot == null) return;

        Vector3 pivotPos = _boomPivot.position + _offset;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, pivotPos);
        Gizmos.DrawWireSphere(pivotPos, 0.15f);

        // Draw desired position
        Gizmos.color = Color.yellow;
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 desiredPos = pivotPos - (rot * Vector3.forward * _distance);
        Gizmos.DrawWireSphere(desiredPos, 0.1f);
    }
}