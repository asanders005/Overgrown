using UnityEngine;

public class SeedPlot : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Talkable;

    public bool CanInteract => !isSeedPlanted;

    [SerializeField] private GameObjectEvent onSeedPlanted;
    [SerializeField] private GameObjectEvent onSeedRemoved;

    private bool isSeedPlanted = false;

    public void Interact()
    {
        if (!isSeedPlanted)
        {
            isSeedPlanted = true;
            onSeedPlanted.RaiseEvent(gameObject);
            onSeedRemoved.Subscribe(OnSeedRemoved); // Subscribe to the seed removed event
        }
    }

    private void OnSeedRemoved(GameObject seedPlot)
    {
        if (seedPlot == gameObject)
        {
            onSeedRemoved.Unsubscribe(OnSeedRemoved); // Unsubscribe from the seed removed event
            isSeedPlanted = false;
        }
    }
}
