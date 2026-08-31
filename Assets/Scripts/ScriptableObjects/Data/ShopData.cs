using UnityEngine;

// Attribute that allows creating instances of this ScriptableObject through the Unity menu
// Creates a menu item at "Assets > Create > Data > ShopData" with default filename "ShopData"
[CreateAssetMenu(fileName = "ShopData", menuName = "Data/ShopData")]
public class ShopData : ScriptableObjectBase
{
    // The main integer value stored in this ScriptableObject
    // SerializeField ensures it's editable in the Inspector despite being private
    [SerializeField] string itemName; // Name of the item in the shop

    [SerializeField] UpgradeType upgradeType;

    [SerializeField] FruitType fruitType; // Optional: The specific fruit type this upgrade applies to (if applicable)

    [SerializeField] int[] pricesByTier; // Prices for upgrades based on their tier (index corresponds to tier level)

    [SerializeField] Sprite icon; // Icon representing the item in the shop

    // Public property to get/set the value
    public string ItemName { get => itemName; set => itemName = value; }
    public string Description { get => description; set => description = value; }
    public UpgradeType UpgradeType { get => upgradeType; set => upgradeType = value; }
    public FruitType FruitType { get => fruitType; set => fruitType = value; }
    public int[] PricesByTier { get => pricesByTier; set => pricesByTier = value; }
    public Sprite Icon { get => icon; set => icon = value; }
}