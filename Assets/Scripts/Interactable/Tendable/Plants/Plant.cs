using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PlantState
{
    Seed,
    Sprout,
    Mature,
    Spoiled,
    Withered
}

[RequireComponent(typeof(PlantAnimator))]
[RequireComponent(typeof(TimerManager))]
public class Plant : Tendable
{
    public override bool CanInteract => needsWatering || currentState == PlantState.Mature || currentState == PlantState.Spoiled || currentState == PlantState.Withered;

    [Header("Plant Settings")]
    [SerializeField] private float timeToSprout = 10f; // Time in seconds to grow from seed to sprout
    [SerializeField] private float timeToMature = 15f; // Time in seconds to mature after sprouting
    [SerializeField] private float timeToSpoil = 20f; // Time in seconds to spoil after maturity
    [SerializeField] private float timeToWither = 5f; // Time in seconds to wither if not watered after sprouting
    [SerializeField] private float timeVariance = 2f; // Variance in time for each growth stage

    [SerializeField] private float waterFrequency = 5f; // Time in seconds before the plant needs watering again
    [SerializeField] private float waterVariance = 1f; // Variance in time before the plant needs watering again

    [SerializeField] private GameObject fruitPrefab; // Prefab for the fruit that appears when the plant is mature

    [Header("Interaction Settings")]
    [SerializeField] private float timeToWater = 5f; // Time in seconds to water the plant
    [SerializeField] private float timeToHarvest = 5f; // Time in seconds to harvest the plant
    [SerializeField] private float timeToClean = 5f; // Time in seconds to clean the plant

    [SerializeField] private Vector2 harvestDirection = Vector2.up; // Direction in which the fruit is harvested

    [Header("Events")]
    [SerializeField] private GameObjectEvent onPlantRemoved; // Event raised when the plant is harvested or cleaned up

    [Header("UI References")]
    [SerializeField] private Image timer;
    [SerializeField] private Sprite waterTimerSprite;
    [SerializeField] private Sprite harvestTimerSprite;

    private PlantState currentState = PlantState.Seed;
    private bool needsWatering = true;

    private PlantAnimator plantAnimator;
    private TimerManager timerManager;

    private Coroutine waterCoroutine;
    private Coroutine witherCoroutine;

    private GameObject seedPlot;

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

    public void AssignSeedPlot(GameObject seedPlot)
    {
        this.seedPlot = seedPlot;
    }

    protected virtual void OnSeedInteract()
    {
        // Default behavior for seed interaction
        needsWatering = false; // The plant has been watered
        StartCoroutine(GrowthCoroutine());
    }

    protected virtual void OnSproutInteract()
    {
        // Default behavior for sprout interaction
        needsWatering = false; // The plant has been watered
        WaterPlant();
    }

    protected virtual void OnMatureInteract() {
        // Default behavior for mature interaction
        Vector2 harvestDir = harvestDirection.normalized; // Normalize the harvest direction
        var fruit = Instantiate(fruitPrefab, transform.position + (Vector3)harvestDir, Quaternion.identity); // Spawn fruit
        if (fruit.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.AddForce(harvestDir * 5f, ForceMode2D.Impulse); // Apply force to the fruit
        }
        timerManager.StopTimer(); // Stop the spoil timer

        onPlantRemoved.RaiseEvent(seedPlot); // Notify that the plant is being removed
        StopAllCoroutines();
        Destroy(gameObject); // Remove the mature plant
    }

    protected virtual void OnSpoiledInteract()
    {
        // Default behavior for spoiled interaction
        onPlantRemoved.RaiseEvent(seedPlot); // Notify that the plant is being removed
        StopAllCoroutines();
        Destroy(gameObject); // Remove the spoiled plant
    }

    protected virtual void OnWitheredInteract()
    {
        // Default behavior for withered interaction
        onPlantRemoved.RaiseEvent(seedPlot); // Notify that the plant is being removed
        StopAllCoroutines();
        Destroy(gameObject); // Remove the withered plant
    }

    void Start()
    {
        timeToSprout += Random.Range(-timeVariance, timeVariance);
        timeToMature += Random.Range(-timeVariance, timeVariance);
        timeToSpoil += Random.Range(-timeVariance, timeVariance);

        plantAnimator = GetComponent<PlantAnimator>();
        timerManager = GetComponent<TimerManager>();
    }

    private void WaterPlant()
    {
        StopCoroutine(witherCoroutine);
        timerManager.StopTimer();
        waterCoroutine = StartCoroutine(WaterTimer(waterFrequency + Random.Range(-waterVariance, waterVariance)));
    }

    private void UpdateState(PlantState newState)
    {
        currentState = newState;
        plantAnimator.UpdateSprite(currentState);
    }

    private void SetHarvestReady()
    {
        if (waterCoroutine != null) StopCoroutine(waterCoroutine);
        if (witherCoroutine != null) StopCoroutine(witherCoroutine);
        waterCoroutine = null;
        timer.sprite = harvestTimerSprite;
        timerManager.SetTimer(timeToSpoil);
    }

    private IEnumerator WaterTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        needsWatering = true;
        if (timeToWither > 0)
            witherCoroutine = StartCoroutine(WitherCoroutine(timeToWither + Random.Range(-timeVariance, timeVariance)));
        waterCoroutine = null;
    }

    protected virtual IEnumerator GrowthCoroutine()
    {
        yield return new WaitForSeconds(timeToSprout);
        UpdateState(PlantState.Sprout);
        if (waterFrequency > 0)
            waterCoroutine = StartCoroutine(WaterTimer(waterFrequency + Random.Range(-waterVariance, waterVariance)));
        yield return new WaitForSeconds(timeToMature);
        UpdateState(PlantState.Mature);
        SetHarvestReady();
        yield return new WaitForSeconds(timeToSpoil);
        UpdateState(PlantState.Spoiled);
    }

    private IEnumerator WitherCoroutine(float witheringTimer)
    {
        timerManager.SetTimer(witheringTimer);
        yield return new WaitForSeconds(witheringTimer);
        UpdateState(PlantState.Withered);
        StopAllCoroutines();
        witherCoroutine = null;
    }
}
