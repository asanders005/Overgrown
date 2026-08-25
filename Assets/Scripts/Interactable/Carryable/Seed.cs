using UnityEngine;

public class Seed : Carryable
{
    [SerializeField] private GameObjectEvent onSeedPlanted;
    [SerializeField] private string seedName;
    [SerializeField] private GameObject plantPrefab;

    public override void Interact()
    {
        base.Interact();
        if (isCarried)
        {
            onSeedPlanted.Subscribe(PlantSeed);
        }
        else
        {
            onSeedPlanted.Unsubscribe(PlantSeed);
        }
    }

    private void PlantSeed(GameObject seedPlot)
    {
        var plant = Instantiate(plantPrefab, seedPlot.transform.position, Quaternion.identity).GetComponent<Plant>();
        plant.AssignSeedPlot(seedPlot);
        onSeedPlanted.Unsubscribe(PlantSeed);
        Destroy(gameObject);
    }
}
