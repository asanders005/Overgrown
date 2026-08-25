using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TimerManager))]
public class OrderUIController : MonoBehaviour
{
    [SerializeField] private GameObject fruitCountUIPrefab;

    [Header("UI References")]
    [SerializeField] private RectTransform fruitCountUIParent;
    [SerializeField] private TMP_Text rewardText;

    private TimerManager timerManager;

    private Dictionary<FruitType, FruitCountUIController> fruitCountUIs = new Dictionary<FruitType, FruitCountUIController>();

    private float fruitCountUIHeight => fruitCountUIPrefab.GetComponent<RectTransform>().rect.width;

    private void Awake()
    {
        timerManager = GetComponent<TimerManager>();
    }

    public void SetOrder(Order order)
    {
        timerManager.SetTimer(order.TimeLimit);
        int index = 0;
        foreach (var item in order.Items)
        {
            var fruitCountUI = Instantiate(fruitCountUIPrefab, fruitCountUIParent);
            fruitCountUI.transform.localPosition = new Vector3(index++ * fruitCountUIHeight, 0, 0);

            var fruitCountUIController = fruitCountUI.GetComponent<FruitCountUIController>();
            fruitCountUIController.Initialize(item.Key, item.Value);
            fruitCountUIs[item.Key] = fruitCountUIController;
        }
    }

    public void UpdateFruitCount(FruitType fruitType, int count)
    {
        if (fruitCountUIs.ContainsKey(fruitType))
        {
            fruitCountUIs[fruitType].UpdateCount(count);
        }
        else
        {
            Debug.LogWarning($"Fruit type {fruitType} not found in the order.");
        }
    }

    public void CompleteOrder()
    {
        // Handle order completion animation here
    }
}
