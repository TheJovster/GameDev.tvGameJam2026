using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private float _interactRange = 3f;
    [SerializeField] private float _rayRadius = 0.3f;
    [SerializeField] private LayerMask _interactLayer = ~0;
    [SerializeField] private Transform _rayOrigin; 

    [Header("Carry Socket")]
    [SerializeField] private Transform _carrySocket;

    [Header("Input")]
    [SerializeField] private InputActionReference _interactAction;

    private GameObject _heldObject;
    private IInteractable _currentTarget;

    public bool IsHolding => _heldObject != null;
    public GameObject HeldObject => _heldObject;

    private void OnEnable()
    {
        if (_interactAction != null)
        {
            _interactAction.action.Enable();
            _interactAction.action.performed += OnInteract;
        }
    }

    private void OnDisable()
    {
        if (_interactAction != null)
        {
            _interactAction.action.performed -= OnInteract;
        }
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
            {
                return interactable;
            }
        }

        return null;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (_currentTarget == null) return;

        if (IsHolding)
        {
            SocketHub socket = GetSocketHubFromTarget(_currentTarget);
            if (socket != null)
            {
                PlugIn(socket);
                return;
            }
        }

        if (!IsHolding)
        {
            _currentTarget.Interact(gameObject);
            PickUp((_currentTarget as MonoBehaviour)?.gameObject);
        }
    }

    private void PickUp(GameObject obj)
    {
        if (obj == null) return;

        _heldObject = obj;

        _heldObject.transform.SetParent(_carrySocket);
        _heldObject.transform.localPosition = Vector3.zero;
        _heldObject.transform.localRotation = Quaternion.identity;

        Rigidbody rb = _heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider col = _heldObject.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    public void PlugIn(SocketHub socket)
    {
        if (_heldObject == null || socket == null) return;

        Transform plugPoint = socket.GetPlugPoint();

        _heldObject.transform.SetParent(plugPoint);
        _heldObject.transform.localPosition = Vector3.zero;
        _heldObject.transform.localRotation = Quaternion.identity;

        Collider col = _heldObject.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        socket.OnPlugged(_heldObject);

        _heldObject = null;
    }

    public void Drop()
    {
        if (_heldObject == null) return;

        _heldObject.transform.SetParent(null);

        Rigidbody rb = _heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider col = _heldObject.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        _heldObject = null;
    }

    private SocketHub GetSocketHubFromTarget(IInteractable target)
    {
        MonoBehaviour mb = target as MonoBehaviour;
        if (mb == null) return null;
        return mb.GetComponentInParent<SocketHub>();
    }

    private void OnDrawGizmosSelected()
    {
        Transform origin = _rayOrigin != null ? _rayOrigin : transform;
        Gizmos.color = IsHolding ? Color.green : Color.red;
        Gizmos.DrawRay(origin.position, origin.forward * _interactRange);
        Gizmos.DrawWireSphere(origin.position + origin.forward * _interactRange, _rayRadius);
    }

    public string GetCurrentPrompt()
    {
        if (_currentTarget == null) return null;

        if (IsHolding && GetSocketHubFromTarget(_currentTarget) != null)
            return "Plug In";

        if (!IsHolding)
            return _currentTarget.InteractionPrompt;

        return null;
    }
}