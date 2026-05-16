using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PickupableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptText = "Pick Up";
    [SerializeField] private bool _canBePickedUp = true;

    public UnityEvent OnPickedUp;
    public UnityEvent OnPluggedIn;

    public string InteractionPrompt => _promptText;

    public bool CanInteract(GameObject interactor)
    {
        if (!_canBePickedUp) return false;

        PlayerInteraction interaction = interactor.GetComponent<PlayerInteraction>();
        return interaction != null && !interaction.IsHolding;
    }

    public void Interact(GameObject interactor)
    {
        _canBePickedUp = false;
        OnPickedUp?.Invoke();
    }

    public void NotifyPluggedIn()
    {
        OnPluggedIn?.Invoke();
    }

    public void ResetPickupable()
    {
        _canBePickedUp = true;
    }
}