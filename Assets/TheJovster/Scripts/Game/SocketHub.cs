using UnityEngine;
using UnityEngine.Events;

public class SocketHub : MonoBehaviour, IInteractable
{
    [Header("Plug Settings")]
    [SerializeField] private PlugColor _plugColor = PlugColor.Red;
    [SerializeField] private Renderer _colorRenderer;

    [Header("Connection")]
    [SerializeField] private Transform _cableAnchor;
    [SerializeField] private bool _isConnected = false;

    public UnityEvent OnTethered;

    public PlugColor PlugColor => _plugColor;
    public bool IsConnected => _isConnected;
    public Transform CableAnchor => _cableAnchor != null ? _cableAnchor : transform;

    public string InteractionPrompt => _isConnected ? "" : "Grab Cable";

    private void Start()
    {
        ApplyColor();
    }

    public bool CanInteract(GameObject interactor)
    {
        if (_isConnected) return false;

        PlayerInteraction interaction = interactor.GetComponent<PlayerInteraction>();
        return interaction != null && !interaction.IsTethered;
    }

    public void Interact(GameObject interactor)
    {
        // PlayerInteraction handles the actual tether setup
        ServiceRegistry.Instance.Get<AudioManager>()?.PlayPlugIn();
    }

    public void SetTethered(bool tethered)
    {
        _isConnected = tethered;
        if (tethered)
            OnTethered?.Invoke();
    }

    private void ApplyColor()
    {
        if (_colorRenderer != null)
            PlugColorHelper.ApplyColor(_plugColor, _colorRenderer);
    }

    private void OnValidate()
    {
        ApplyColor();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = PlugColorHelper.ToColor(_plugColor);
        Vector3 anchor = _cableAnchor != null ? _cableAnchor.position : transform.position;
        Gizmos.DrawWireSphere(anchor, 0.25f);

        if (_isConnected)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireCube(anchor, Vector3.one * 0.15f);
        }
    }
}