using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SeedPlot : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Talkable;

    public bool CanInteract => !isSeedPlanted;

    [SerializeField] private TransformEvent onSeedPlanted;
    [SerializeField] private TransformEvent onSeedRemoved;

    private bool isSeedPlanted = false;

    private Collider2D _collider;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void Interact()
    {
        if (!isSeedPlanted)
        {
            onSeedPlanted.RaiseEvent(transform);
            onSeedRemoved.Subscribe(OnSeedRemoved); // Subscribe to the seed removed event
            isSeedPlanted = true;
        }
    }

    private void OnSeedRemoved(Transform seedTransform)
    {
        if (seedTransform == transform)
        {
            isSeedPlanted = false;
            onSeedRemoved.Unsubscribe(OnSeedRemoved); // Unsubscribe from the seed removed event
        }
    }
}
