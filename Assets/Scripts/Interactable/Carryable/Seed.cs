using UnityEngine;

public class Seed : Carryable
{
    [SerializeField] private TransformEvent onSeedPlanted;
    [SerializeField] private string seedName;
    [SerializeField] private GameObject plantPrefab;

    public override void Interact()
    {
        base.Interact();
        if (isCarried)
        {
            onSeedPlanted.Subscribe(PlantSeed);
            Debug.Log($"Carrying {seedName}. Waiting for planting...");
        }
        else
        {
            onSeedPlanted.Unsubscribe(PlantSeed);
            Debug.Log($"Dropped {seedName}. No longer waiting for planting.");
        }
    }

    private void PlantSeed(Transform location)
    {
        Instantiate(plantPrefab, location.position, Quaternion.identity);
        onSeedPlanted.Unsubscribe(PlantSeed);
        Destroy(gameObject);
    }
}
