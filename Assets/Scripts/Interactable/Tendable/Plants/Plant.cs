using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum PlantState
{
    Seed,
    Sprout,
    Mature,
    Spoiled,
    Withered
}

[RequireComponent(typeof(PlantAnimator))]
public class Plant : Tendable
{
    [Header("Plant Settings")]
    [SerializeField] private float timeToSprout = 10f; // Time in seconds to grow from seed to sprout
    [SerializeField] private float timeToMature = 15f; // Time in seconds to mature after sprouting
    [SerializeField] private float timeToSpoil = 20f; // Time in seconds to spoil after maturity
    [SerializeField] private float timeToWither = 5f; // Time in seconds to wither if not watered after sprouting
    [SerializeField] private float timeVariance = 2f; // Variance in time for each growth stage

    [SerializeField] private GameObject fruitPrefab; // Prefab for the fruit that appears when the plant is mature

    [Header("Interaction Settings")]
    [SerializeField] private float timeToWater = 5f; // Time in seconds to water the plant
    [SerializeField] private float timeToHarvest = 5f; // Time in seconds to harvest the plant
    [SerializeField] private float timeToClean = 5f; // Time in seconds to clean the plant

    [Header("Events")]
    [SerializeField] private TransformEvent onPlantRemoved; // Event raised when the plant is harvested or cleaned up

    private PlantState currentState = PlantState.Seed;
    private float witheringTimer = 0f;

    private PlantAnimator plantAnimator;

    public override float TimeToInteract
    {
        get
        {
            switch(currentState)
            {
                case PlantState.Seed:
                case PlantState.Sprout:
                    return timeToWater;
                case PlantState.Mature:
                    return timeToHarvest;
                case PlantState.Spoiled:
                    return timeToClean;
                default:
                    return 0f;
            }
        }
    }

    public override void Interact()
    {
        switch (currentState)
        {
            case PlantState.Seed:
                OnSeedInteract();
                break;
            case PlantState.Sprout:
                OnSproutInteract();
                break;
            case PlantState.Mature:
                OnMatureInteract();
                break;
            case PlantState.Spoiled:
                OnSpoiledInteract();
                break;
            case PlantState.Withered:
                OnWitheredInteract();
                break;
             
        }
    }

    protected virtual void OnSeedInteract()
    {
        // Default behavior for seed interaction
        StartCoroutine(GrowthCoroutine());
    }

    protected virtual void OnSproutInteract()
    {
        // Default behavior for sprout interaction
        ResetWitherCoroutine();
    }

    protected virtual void OnMatureInteract() {
        // Default behavior for mature interaction
        // Harvest plant and create carryable item (not implemented in this example)
        throw new System.NotImplementedException("Harvesting not implemented yet.");
    }

    protected virtual void OnSpoiledInteract()
    {
        // Default behavior for spoiled interaction
        onPlantRemoved.RaiseEvent(transform); // Notify that the plant is being removed
        Destroy(gameObject); // Remove the spoiled plant
    }

    protected virtual void OnWitheredInteract()
    {
        // Default behavior for withered interaction
        onPlantRemoved.RaiseEvent(transform); // Notify that the plant is being removed
        Destroy(gameObject); // Remove the withered plant
    }

    void Start()
    {
        timeToSprout += Random.Range(-timeVariance, timeVariance);
        timeToMature += Random.Range(-timeVariance, timeVariance);
        timeToSpoil += Random.Range(-timeVariance, timeVariance);

        plantAnimator = GetComponent<PlantAnimator>();
    }

    private void ResetWitherCoroutine()
    {
        StopCoroutine(WitherCoroutine());
        witheringTimer = timeToWither + Random.Range(-timeVariance, timeVariance);
        StartCoroutine(WitherCoroutine());
    }

    private void UpdateState(PlantState newState)
    {
        currentState = newState;
        plantAnimator.UpdateSprite(currentState);
    }

    private IEnumerator GrowthCoroutine()
    {
        yield return new WaitForSeconds(timeToSprout);
        UpdateState(PlantState.Sprout);
        StartCoroutine(WitherCoroutine());
        yield return new WaitForSeconds(timeToMature);
        UpdateState(PlantState.Mature);
        yield return new WaitForSeconds(timeToSpoil);
        UpdateState(PlantState.Spoiled);
    }

    private IEnumerator WitherCoroutine()
    {
        yield return new WaitForSeconds(witheringTimer);
        // Transition to Withered state
    }
}
