public enum InteractableType
{
    None,
    Tendable,
    Carryable,
    Talkable
}

public interface IInteractable
{
    InteractableType Type { get; }
    bool CanInteract { get; }

    void Interact();
}
