using UnityEngine;

public class SellBin : MonoBehaviour, IInteractable
{
    [SerializeField] private Event onSellBegin;

    public InteractableType Type => InteractableType.Talkable;

    public bool CanInteract => true;

    public void Interact()
    {
        onSellBegin.RaiseEvent();
    }
}
