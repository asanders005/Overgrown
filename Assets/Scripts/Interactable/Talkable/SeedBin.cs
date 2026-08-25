using UnityEngine;

public class SeedBin : MonoBehaviour, IInteractable
{
    public InteractableType Type => InteractableType.Talkable;

    public bool CanInteract => !playerController.isCarryingObject;

    [SerializeField] private GameObject seedPrefab;

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("[SeedBin]: PlayerController not found in scene");
        }
    }

    public void Interact()
    {
        var seed = Instantiate(seedPrefab);
        if (!seed.TryGetComponent<IInteractable>(out var interactable) || !playerController.SetCarriedObj(interactable))
        {
            Debug.LogWarning("[SeedBin]: Unable to assign carried object to player");
            Destroy(seed);
        }
    }
}
