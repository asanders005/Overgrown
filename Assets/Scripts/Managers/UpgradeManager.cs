using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UpgradeType
{
    SeedPlot,       // Unlock more seed plots
    SeedUnlock,     // Unlock new plants to grow
    SeedQuality,    // Improve sell value of fruit
    MoveSpeed,      // Increase player's move speed
    InteractSpeed,  // Increase interaction speed
    OrderExtension, // Increase time to fulfill orders
    OrderReward,    // Increase order reward
}

public class UpgradeManager : MonoBehaviour
{
    [Header("Game Object References")]
    [SerializeField] private List<GameObject> seedPlots;
    [SerializeField] private List<SeedBin> seedBins;

    [Header("Upgrade Settings")]
    [SerializeField] private UpgradeEvent upgradeEvent;
    [SerializeField] private int seedQualityUpgradeLimit = 3;       // Limit for seed quality upgrades per fruit type
    [SerializeField] private int moveSpeedUpgradeLimit = 3;         // Limit for move speed upgrades
    [SerializeField] private int interactSpeedUpgradeLimit = 3;     // Limit for interact speed upgrades
    [SerializeField] private int orderTimeLimitUpgradeLimit = 3;    // Limit for order time limit upgrades
    [SerializeField] private int orderRewardUpgradeLimit = 3;       // Limit for order reward upgrades

    [Header("Events")]
    [SerializeField] private FruitEvent onSeedQualityUpgrade;
    [SerializeField] private Event onMoveSpeedUpgrade;
    [SerializeField] private Event onInteractSpeedUpgrade;

    [SerializeField] private Event onOrderTimeLimitUpgrade;
    [SerializeField] private Event onOrderRewardUpgrade;

    public Dictionary<UpgradeType, bool> UpgradesAvailable { get => upgradesAvailable; }
    private Dictionary<UpgradeType, bool> upgradesAvailable;

    private int plotsUnlocked = 0;

    private Dictionary<FruitType, int> seedQualityUpgrades;
    private int moveSpeedUpgrades = 0;
    private int interactSpeedUpgrades = 0;
    private int orderTimeLimitUpgrades = 0;
    private int orderRewardUpgrades = 0;

    private void OnEnable()
    {
        upgradeEvent.Subscribe(ProcessUpgrade);
    }

    private void OnDisable()
    {
        upgradeEvent.Unsubscribe(ProcessUpgrade);
    }

    private void Awake()
    {
        upgradesAvailable = new Dictionary<UpgradeType, bool>();
        foreach (UpgradeType type in Enum.GetValues(typeof(UpgradeType)))
        {
            upgradesAvailable[type] = true;
        }

        seedQualityUpgrades = new Dictionary<FruitType, int>();
        foreach (FruitType fruitType in Enum.GetValues(typeof(FruitType)))
        {
            seedQualityUpgrades[fruitType] = 0;
        }
    }

    public ShopItem[] GetAvailableUpgrades(int count)
    {
        var availableUpgrades = new List<ShopItem>();
        foreach (var upgrade in upgradesAvailable)
        {
            if (upgrade.Value) // If the upgrade is available
            {
                if (upgrade.Key == UpgradeType.SeedUnlock || upgrade.Key == UpgradeType.SeedQuality)
                {
                    upgradesAvailable[upgrade.Key] = false; // Temporarily mark as unavailable to avoid duplicates
                    foreach (var seedBin in seedBins)
                    {
                        if (!seedBin.gameObject.activeSelf && upgrade.Key == UpgradeType.SeedUnlock) // If the seed bin is not unlocked
                        {
                            upgradesAvailable[upgrade.Key] = true; // Mark as available again for other seed bins
                            availableUpgrades.Add(new ShopItem
                            {
                                type = upgrade.Key,
                                fruitType = seedBin.SeedType,
                                name = $"{seedBin.SeedType} Seed Unlock",
                                description = $"Unlock {seedBin.SeedType} seeds for planting and orders.",
                                icon = null, // Assign appropriate icon based on upgradeType and fruitType
                                price = 100 // Set a default price or calculate based on upgrade type
                            });
                        }
                        else if (upgrade.Key == UpgradeType.SeedQuality && seedBin.gameObject.activeSelf && seedQualityUpgrades[seedBin.SeedType] < seedQualityUpgradeLimit) // If the seed bin is unlocked and upgradable
                        {
                            upgradesAvailable[upgrade.Key] = true; // Mark as available again for other seed bins
                            availableUpgrades.Add(new ShopItem
                            {
                                type = upgrade.Key,
                                fruitType = seedBin.SeedType,
                                name = $"{seedBin.SeedType} Seed Quality Upgrade",
                                description = $"Upgrade the quality of your {seedBin.SeedType} seeds.",
                                icon = null, // Assign appropriate icon based on upgradeType and fruitType
                                price = 100 // Set a default price or calculate based on upgrade type
                            });
                        }
                    }
                }
                else
                {
                    availableUpgrades.Add(new ShopItem
                    {
                        type = upgrade.Key,
                        fruitType = null,
                        name = $"{upgrade.Key} Upgrade",
                        description = $"Upgrade your general capabilities with this {upgrade.Key} upgrade.",
                        icon = null, // Assign appropriate icon based on upgradeType and fruitType
                        price = 100 // Set a default price or calculate based on upgrade type
                    });
                }
            }
        }
        // Shuffle the list to randomize the order of upgrades
        availableUpgrades = availableUpgrades.OrderBy(x => Guid.NewGuid()).ToList();
        // Return only the requested number of upgrades
        return availableUpgrades.Take(count).ToArray();
    }

    private void ProcessUpgrade(UpgradeType upgradeType, FruitType? fruitType)
    {
        if (!upgradesAvailable.ContainsKey(upgradeType) || !upgradesAvailable[upgradeType])
            return;

        switch (upgradeType)
        {
            case UpgradeType.SeedPlot:
                seedPlots[plotsUnlocked++].SetActive(true);
                if (plotsUnlocked >= seedPlots.Count)
                    upgradesAvailable[upgradeType] = false;
                break;
            case UpgradeType.SeedUnlock:
                if (fruitType == null)
                {
                    Debug.LogWarning("[PROCESS UPGRADE] Seed unlock failed; fruitType is null");
                    return;
                }
                var seedBin = seedBins.FirstOrDefault(bin => bin.SeedType == fruitType);
                if (seedBin == null)
                {
                    Debug.LogWarning($"[PROCESS UPGRADE] Seed unlock failed; no seed bin found for fruitType {fruitType}");
                    return;
                }
                seedBin.gameObject.SetActive(true);

                break;
            case UpgradeType.SeedQuality:
                if (fruitType == null)
                {
                    Debug.LogError("[PROCESS UPGRADE] Seed Quality failed; fruitType is null");
                    return;
                }

                if (!seedQualityUpgrades.ContainsKey(fruitType.Value))
                {
                    Debug.LogError($"[PROCESS UPGRADE] Seed Quality failed; no entry for fruitType {fruitType}");
                    return;
                }

                onSeedQualityUpgrade.RaiseEvent(fruitType.Value);
                seedQualityUpgrades[fruitType.Value]++;
                if (seedQualityUpgrades[fruitType.Value] >= seedQualityUpgradeLimit)
                    upgradesAvailable[upgradeType] = false;

                break;
            case UpgradeType.MoveSpeed:
                onMoveSpeedUpgrade.RaiseEvent();
                moveSpeedUpgrades++;
                if (moveSpeedUpgrades >= moveSpeedUpgradeLimit)
                    upgradesAvailable[upgradeType] = false;
                break;
            case UpgradeType.InteractSpeed:
                onInteractSpeedUpgrade.RaiseEvent();
                interactSpeedUpgrades++;
                if (interactSpeedUpgrades >= interactSpeedUpgradeLimit)
                    upgradesAvailable[upgradeType] = false;
                break;
            case UpgradeType.OrderExtension:
                onOrderTimeLimitUpgrade.RaiseEvent();
                orderTimeLimitUpgrades++;
                if (orderTimeLimitUpgrades >= orderTimeLimitUpgradeLimit)
                    upgradesAvailable[upgradeType] = false;
                break;
            case UpgradeType.OrderReward:
                onOrderRewardUpgrade.RaiseEvent();
                orderRewardUpgrades++;
                if (orderRewardUpgrades >= orderRewardUpgradeLimit)
                    upgradesAvailable[upgradeType] = false;
                break;
            default:
                Debug.LogWarning($"[PROCESS UPGRADE] Unhandled upgrade type: {upgradeType}");
                break;
        }
    }
}
