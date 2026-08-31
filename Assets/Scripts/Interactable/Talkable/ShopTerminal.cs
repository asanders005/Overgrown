using UnityEngine;

public class ShopTerminal : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Talkable;

    public bool CanInteract => true;

    public void Interact()
    {
        onShopOpened.RaiseEvent();
    }

    [SerializeField] private Event onShopOpened;
}
