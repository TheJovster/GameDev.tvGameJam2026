using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float _interactRange = 3f;
    [SerializeField] private float _rayRadius = 0.3f;
    [SerializeField] private LayerMask _interactLayer = ~0;
    [SerializeField] private Transform _rayOrigin;

    [Header("Tether")]
    [SerializeField] private Transform _cableAttachPoint;
    [SerializeField] private CableLine _cableLine;
    [SerializeField] private CableLine _permanentCablePrefab;

    [Header("Input")]
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private InputActionReference _disconnectAction;

    private SocketHub _tetheredDevice;
    private IInteractable _currentTarget;

    public bool IsTethered => _tetheredDevice != null;
    public PlugColor TetheredColor => _tetheredDevice != null ? _tetheredDevice.PlugColor : PlugColor.White;
    public SocketHub TetheredDevice => _tetheredDevice;
    public Transform CableAttachPoint => _cableAttachPoint != null ? _cableAttachPoint : transform;

    private void OnEnable()
    {
        if (_interactAction != null)
        {
            _interactAction.action.Enable();
            _interactAction.action.performed += OnInteract;
        }

        if (_disconnectAction != null)
        {
            _disconnectAction.action.Enable();
            _disconnectAction.action.performed += OnDisconnect;
        }
    }

    private void OnDisable()
    {
        if (_interactAction != null)
            _interactAction.action.performed -= OnInteract;

        if (_disconnectAction != null)
            _disconnectAction.action.performed -= OnDisconnect;
    }

    private void Update()
    {
        _currentTarget = Probe();
    }

    private IInteractable Probe()
    {
        Transform origin = _rayOrigin != null ? _rayOrigin : transform;
        Ray ray = new Ray(origin.position, origin.forward);

        if (Physics.SphereCast(ray, _rayRadius, out RaycastHit hit, _interactRange, _interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract(gameObject))
                return interactable;
        }

        return null;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_currentTarget == null) return;

        if (IsTethered)
        {
            CentralBattery battery = GetComponentFromTarget<CentralBattery>(_currentTarget);
            if (battery != null)
            {
                ConnectToBattery(battery);
                return;
            }
        }

        if (!IsTethered)
        {
            SocketHub device = GetComponentFromTarget<SocketHub>(_currentTarget);
            if (device != null)
            {
                TetherTo(device);
                return;
            }
        }
    }

    private void OnDisconnect(InputAction.CallbackContext ctx)
    {
        Disconnect();
    }

    private void TetherTo(SocketHub device)
    {
        _tetheredDevice = device;
        _tetheredDevice.SetTethered(true);

        if (_cableLine != null)
            _cableLine.Activate(device.CableAnchor, CableAttachPoint, device.PlugColor);
    }

    private void ConnectToBattery(CentralBattery battery)
    {
        if (_tetheredDevice == null) return;

        Transform snapPoint = battery.ConnectPlug(_tetheredDevice.PlugColor);
        if (snapPoint == null) return;

        if (_cableLine != null)
            _cableLine.Deactivate();

        if (_permanentCablePrefab != null)
        {
            CableLine permanent = Instantiate(_permanentCablePrefab);
            permanent.Activate(_tetheredDevice.CableAnchor, snapPoint, _tetheredDevice.PlugColor);
        }

        _tetheredDevice = null;
    }

    public void Disconnect()
    {
        if (_tetheredDevice == null) return;

        _tetheredDevice.SetTethered(false);
        _tetheredDevice = null;

        if (_cableLine != null)
            _cableLine.Deactivate();
    }

    private T GetComponentFromTarget<T>(IInteractable target) where T : Component
    {
        MonoBehaviour mb = target as MonoBehaviour;
        if (mb == null) return null;
        return mb.GetComponentInParent<T>();
    }

    public string GetCurrentPrompt()
    {
        if (_currentTarget == null)
        {
            if (IsTethered) return "Press [Q] to Disconnect";
            return null;
        }

        if (IsTethered && GetComponentFromTarget<CentralBattery>(_currentTarget) != null)
            return "Connect to Battery";

        if (!IsTethered)
            return _currentTarget.InteractionPrompt;

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = _rayOrigin != null ? _rayOrigin : transform;
        Gizmos.color = IsTethered ? Color.green : Color.red;
        Gizmos.DrawRay(origin.position, origin.forward * _interactRange);
        Gizmos.DrawWireSphere(origin.position + origin.forward * _interactRange, _rayRadius);
    }
}