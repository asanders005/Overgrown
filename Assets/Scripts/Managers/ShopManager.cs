using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopItem
{
    public UpgradeType type; // Type of upgrade this shop item represents
    public FruitType? fruitType; // Optional: The specific fruit type this upgrade applies to (if applicable)

    public string name;
    public string description;
    public Sprite icon; // Icon representing the item in the shop
    public int price; // Price of the item in the shop

    public int level; // Level of the upgrade, if applicable - 0 if not applicable
}

[RequireComponent(typeof(UpgradeManager))]
public class ShopManager : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private float restockInterval = 120f; // Time in seconds between restocks
    [SerializeField] private int maxItemsInShop = 5; // Maximum number of items in the shop at any time

    [Header("Price Settings")]
    [SerializeField] private List<ShopData> upgradePrices; // Default prices for upgrades based on their tier (index corresponds to tier level)

    [Header("UI References")]
    [SerializeField] private GameObject shopUI; // Reference to the shop UI GameObject
    [SerializeField] private Transform shopItemContainer; // Parent transform for shop item UI elements
    [SerializeField] private GameObject shopItemPrefab; // Prefab for individual shop item UI elements

    [Header("Events")]
    [SerializeField] private Event onShopRestocked; // Event triggered when the shop is restocked
    [SerializeField] private UpgradeEvent onUpgradePurchased; // Event to trigger upgrade purchase logic
    [SerializeField] private IntEvent onShopItemPurchased; // Event to trigger when a shop item is purchased, passing the index of the purchased item
    [SerializeField] private IntEvent onCurrencyUpdate; // Event to trigger when the player's currency is updated

    [SerializeField] private Event onShopOpened; // Event to trigger when the shop is opened
    [SerializeField] private Event onShopClosed; // Event to trigger when the shop is closed

    private ShopItem[] shopItems; // List of all possible shop items

    private UpgradeManager upgradeManager;

    private ShopItemUIController[] shopItemUIControllers; // Array of UI controllers for the shop items

    private void OnEnable()
    {
        onShopItemPurchased.Subscribe(OnShopItemPurchased);
        onShopOpened.Subscribe(OnShopOpen);
        onShopClosed.Subscribe(OnShopClose);
    }

    private void OnDisable()
    {
        onShopItemPurchased.Unsubscribe(OnShopItemPurchased);
        onShopOpened.Unsubscribe(OnShopOpen);
        onShopClosed.Unsubscribe(OnShopClose);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeManager = GetComponent<UpgradeManager>();
        shopItems = new ShopItem[maxItemsInShop];
        shopItemUIControllers = new ShopItemUIController[maxItemsInShop];
        InitializeUI();
        RestockShop();
        StartCoroutine(RestockCoroutine());
    }

    #region Restock Logic
    private IEnumerator RestockCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(restockInterval);
            RestockShop();
        }
    }

    private void RestockShop()
    {
        var upgrades = upgradeManager.GetAvailableUpgrades(maxItemsInShop);
        shopItems = new ShopItem[upgrades.Length];

        for (int i = 0; i < upgrades.Length; i++)
        {
            var upgrade = upgrades[i];
            var shopData = upgradePrices.FirstOrDefault(data => data.UpgradeType == upgrade.type && (upgrade.fruitType == null || data.FruitType == upgrade.fruitType));
            if (shopData == null)
            {
                Debug.LogWarning($"No shop data found for upgrade type {upgrade.type} and fruit type {upgrade.fruitType}");
                continue;
            }

            ShopItem item = new ShopItem
            {
                type = upgrade.type,
                fruitType = upgrade.fruitType,
                name = shopData.ItemName,
                description = shopData.Description,
                icon = shopData.Icon,
                price = shopData.PricesByTier[upgrade.level]
            };
            shopItems[i] = item;
        }
        UpdateShopUI();

        onShopRestocked.RaiseEvent();
    }
    #endregion

    #region Purchase Logic
    private void OnShopItemPurchased(int index)
    {
        if (index < 0 || index >= shopItems.Length)
        {
            Debug.LogWarning("Invalid shop item index: " + index);
            return;
        }
        ShopItem purchasedItem = shopItems[index];
        onUpgradePurchased.RaiseEvent(purchasedItem.type, purchasedItem.fruitType);
        onCurrencyUpdate.RaiseEvent(-purchasedItem.price);
    }
    #endregion

    #region UI Logic
    private void InitializeUI()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            GameObject itemUIObj = Instantiate(shopItemPrefab, shopItemContainer);
            itemUIObj.transform.localPosition = new Vector3(0, -i * itemUIObj.GetComponent<RectTransform>().rect.height, 0);
            ShopItemUIController itemUIController = itemUIObj.GetComponent<ShopItemUIController>();
            if (itemUIController != null)
            {
                shopItemUIControllers[i] = itemUIController;
            }
            else
            {
                Debug.LogError("ShopItemUIController component not found on the instantiated prefab.");
            }
        }
    }

    private void UpdateShopUI()
    {
        for (int i = 0; i < shopItems.Length; i++)
        {
            shopItemUIControllers[i].Set(shopItems[i], i);
        }
    }

    private void OnShopOpen()
    {
        shopUI.SetActive(true);
    }

    private void OnShopClose()
    {
        shopUI.SetActive(false);
    }
    #endregion
}
