using UnityEngine;
using UnityEngine.Events;

public class SocketHub : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _plugPoint;
    [SerializeField] private string _promptWhenEmpty = "Plug In";
    [SerializeField] private bool _isOccupied = false;

    public UnityEvent<GameObject> OnObjectPlugged;

    public string InteractionPrompt => _isOccupied ? "" : _promptWhenEmpty;

    public bool CanInteract(GameObject interactor)
    {
        if (_isOccupied) return false;

        PlayerInteraction interaction = interactor.GetComponent<PlayerInteraction>();
        return interaction != null && interaction.IsHolding;
    }

    public void Interact(GameObject interactor)
    {
        // Actual plug-in is handled by PlayerInteraction.PlugIn()
        // This exists to satisfy the interface
    }

    public Transform GetPlugPoint()
    {
        return _plugPoint != null ? _plugPoint : transform;
    }

    public void OnPlugged(GameObject pluggedObject)
    {
        _isOccupied = true;
        OnObjectPlugged?.Invoke(pluggedObject);
    }

    private void OnDrawGizmos()
    {
        Transform point = _plugPoint != null ? _plugPoint : transform;
        Gizmos.color = _isOccupied ? Color.red : Color.green;
        Gizmos.DrawWireCube(point.position, Vector3.one * 0.3f);
    }
}