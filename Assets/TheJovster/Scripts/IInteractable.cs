using UnityEngine;

public interface IInteractable
{
    public string InteractionPrompt { get; }
    bool CanInteract(GameObject interactor);
    void Interact(GameObject interactor);
}

