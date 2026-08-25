using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum OrderType
{
    Normal,
    Large,
    Rush,
    Variety,
    Bulk
}

public class Order
{
    public Dictionary<FruitType, int> Items { get; private set; }
    public float TimeLimit { get; private set; } = 1.0f;
    private float timer = 0.0f;
    public int Reward { get; private set; } = 10;
    public bool TimeExpired => timer <= 0.0f;
    public Order(Dictionary<FruitType, int> items, float timeLimit, int reward)
    {
        Items = items;
        TimeLimit = timeLimit;
        timer = TimeLimit;
        Reward = reward;
    }

    public void UpdateTimer(float deltaTime)
    {
        timer -= deltaTime;
    }
}

public class OrderManager : MonoBehaviour
{
    [Header("Order Generation Settings")]
    [SerializeField] private float orderGenerationInterval = 15.0f;
    [SerializeField] private float orderGenerationVariance = 5.0f;

    [SerializeField] private int maxOrders = 5;
    [SerializeField] private List<OrderDefinitionData> orderDefinitions;
    [SerializeField] private float orderCompleteAnimDuration = 1.0f;

    [Header("Events")]
    [SerializeField] private FruitEvent onOrderUpdate;
    [SerializeField] private Event onOrderUpdateComplete;
    [SerializeField] private IntEvent onOrderDeliver;
    [SerializeField] private Event onOrderFail;

    [Header("UI References")]
    [SerializeField] private GameObject orderUIPrefab;
    [SerializeField] private RectTransform orderUIParent;

    private List<Order> orders = new List<Order>();
    private Queue<Order> orderPool = new Queue<Order>();
    private List<OrderUIController> orderUIControllers = new List<OrderUIController>();

    private float OrderUIHeight => orderUIPrefab.GetComponent<RectTransform>().rect.height;

    private void OnEnable()
    {
        onOrderUpdate.Subscribe(UpdateOrder);
    }

    private void OnDisable()
    {
        onOrderUpdate.Unsubscribe(UpdateOrder);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GenerateOrdersCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var order in orders.ToList())
        {
            order.UpdateTimer(Time.deltaTime);
            if (order.TimeExpired)
            {
                FailOrder(order);
            }
        }
    }

    private void AddOrder(Order order)
    {
        if (orders.Count < maxOrders && orderPool.Count == 0)
        {
            orders.Add(order);
            UpdateUI();
        }
        else
        {
            orderPool.Enqueue(order);
        }
    }

    private void UpdateOrder(FruitType fruitType)
    {
        if (orders.Count == 0) return;

        int index = 0;
        do
        {
            if (orders[index].Items.ContainsKey(fruitType))
            {
                orders[index].Items[fruitType]--;
                UpdateOrderUI(index, fruitType, orders[index].Items[fruitType]);
                if (orders[index].Items[fruitType] <= 0)
                {
                    orders[index].Items.Remove(fruitType);
                }
                if (orders[index].Items.Count == 0)
                {
                    DeliverOrder(index);
                }
                onOrderUpdateComplete.RaiseEvent();
                break;
            }
            index++;
        } while (index < orders.Count);
    }

    private void DeliverOrder(int index)
    {
        if (index < 0 || index >= orders.Count)
        {
            Debug.LogError("Invalid order index.");
            return;
        }
        Order order = orders[index];
        orders.RemoveAt(index);
        StartCoroutine(CompleteOrder(index));

        onOrderDeliver.RaiseEvent(order.Reward);
    }

    private void FailOrder(Order order)
    {
        var index = orders.IndexOf(order);
        if (index >= 0)
        {
            orders.RemoveAt(index);
            onOrderFail.RaiseEvent();
            if (orderPool.Count > 0)
            {
                orders.Add(orderPool.Dequeue());
            }
            UpdateUI();
        }
    }

    private IEnumerator GenerateOrdersCoroutine()
    {
        while (true)
        {
            Order newOrder = GenerateOrder();
            if (newOrder != null)
            {
                AddOrder(newOrder);
            }
            float waitTime = orderGenerationInterval + Random.Range(-orderGenerationVariance, orderGenerationVariance);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator CompleteOrder(int index)
    {
        orderUIControllers[index].CompleteOrder();
        yield return new WaitForSeconds(orderCompleteAnimDuration);
        if (orderPool.Count > 0)
        {
            orders.Add(orderPool.Dequeue());
        }

        UpdateUI();
    }

    private Order GenerateOrder()
    {
        OrderType orderType = (OrderType)Random.Range(0, System.Enum.GetValues(typeof(OrderType)).Length);

        OrderDefinitionData orderDefinition = orderDefinitions.FirstOrDefault(def => def.Type == orderType);
        if (orderDefinition == null)
        {
            Debug.LogError($"No order definition found for order type: {orderType}");
            return null;
        }

        var items = GenerateOrderItems(orderDefinition);
        float timeLimit = Mathf.Max(5, orderDefinition.TimeLimit + Random.Range(-orderDefinition.TimeLimitVariance, orderDefinition.TimeLimitVariance));
        int reward = Mathf.RoundToInt(Mathf.Max(1, orderDefinition.Reward + Random.Range(-orderDefinition.RewardVariance, orderDefinition.RewardVariance)));

        return new Order(items, timeLimit, reward);
    }

    private Dictionary<FruitType, int> GenerateOrderItems(OrderDefinitionData orderDefinition)
    {
        Dictionary<FruitType, int> items = new Dictionary<FruitType, int>();
        List<FruitType> fruitTypes = System.Enum.GetValues(typeof(FruitType)).Cast<FruitType>().ToList();
        int fruitVarietyCount = Mathf.Max(1, orderDefinition.FruitVarietyCount + Random.Range(-orderDefinition.FruitVarietyVariance, orderDefinition.FruitVarietyVariance + 1));
        for (int i = 0; i < fruitVarietyCount; i++)
        {
            FruitType fruitType;
            do
            {
                fruitType = fruitTypes[Random.Range(0, fruitTypes.Count)];
            } while (items.ContainsKey(fruitType));
            int fruitCount = orderDefinition.FruitCount + Random.Range(-orderDefinition.FruitCountVariance, orderDefinition.FruitCountVariance + 1);
            items[fruitType] = fruitCount;
        }
        return items;
    }

    // TODO: Fix Add/Remove order restarting timer UI on all orders
    private void UpdateUI()
    {
        // Clear existing UI elements
        foreach (Transform child in orderUIParent)
        {
            Destroy(child.gameObject);
        }
        // Create new UI elements for each order
        for (int i = 0; i < orders.Count; i++)
        {
            Order order = orders[i];
            GameObject orderUI = Instantiate(orderUIPrefab, orderUIParent);
            orderUI.transform.localPosition = new Vector3(0, -i * OrderUIHeight, 0);
            OrderUIController uiController = orderUI.GetComponent<OrderUIController>();
            if (uiController != null)
            {
                uiController.SetOrder(order);
                orderUIControllers.Add(uiController);
            }
            else
            {
                Debug.LogError("OrderUIController component not found on the order UI prefab.");
            }
        }
    }

    private void UpdateOrderUI(int index, FruitType fruitType, int count)
    {
        if (index < 0 || index >= orderUIControllers.Count)
        {
            Debug.LogError("Invalid order UI index.");
            return;
        }
        OrderUIController uiController = orderUIControllers[index];
        uiController.UpdateFruitCount(fruitType, count);
    }
}
