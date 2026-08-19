using UnityEngine;

public abstract class Tendable : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Tendable;
    public abstract float TimeToInteract { get; }

    public abstract bool CanInteract { get; }

    public abstract void Interact();
}
