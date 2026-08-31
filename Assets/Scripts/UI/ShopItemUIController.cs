using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemPriceText;
    [SerializeField] private Image itemIconImage;

    [SerializeField] private IntEvent onShopItemPurchased; // Event to trigger when a shop item is purchased, passing the index of the purchased item

    private int itemIndex; // Index of the shop item in the ShopManager's list

    public void Set(ShopItem shopItem, int index)
    {
        itemIndex = index;
        itemNameText.text = shopItem.name;
        itemDescriptionText.text = shopItem.description;
        itemPriceText.text = $"Price: {shopItem.price}";
        itemIconImage.sprite = shopItem.icon;
    }

    public void OnPurchaseButtonClicked()
    {
        onShopItemPurchased.RaiseEvent(itemIndex);
    }
}
