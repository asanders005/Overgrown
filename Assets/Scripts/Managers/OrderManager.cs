using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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
    public int Reward { get; private set; } = 10;
    public Order(Dictionary<FruitType, int> items, float timeLimit, int reward)
    {
        Items = items;
        TimeLimit = timeLimit;
        Reward = reward;
    }
}

public class OrderManager : MonoBehaviour
{
    [SerializeField] private FruitEvent onOrderUpdate;
    [SerializeField] private IntEvent onOrderDeliver;

    [SerializeField] private float orderGenerationInterval = 15.0f;
    [SerializeField] private float orderGenerationVariance = 5.0f;

    [SerializeField] private int maxOrders = 5;
    [SerializeField] private List<OrderDefinitionData> orderDefinitions;

    private List<Order> orders = new List<Order>();
    private Queue<Order> orderPool = new Queue<Order>();

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

    }

    private void UpdateOrder(FruitType fruitType)
    {
        int index = 0;
        do
        {
            if (orders[index].Items.ContainsKey(fruitType))
            {
                orders[index].Items[fruitType]--;
                if (orders[index].Items[fruitType] <= 0)
                {
                    orders[index].Items.Remove(fruitType);
                }
                if (orders[index].Items.Count == 0)
                {
                    DeliverOrder(index);
                }
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
        if (orderPool.Count > 0)
        {
            orders.Add(orderPool.Dequeue());
        }

        onOrderDeliver.RaiseEvent(order.Reward);
    }

    private IEnumerator GenerateOrdersCoroutine()
    {
        while (true)
        {
            Order newOrder = GenerateOrder();
            if (newOrder != null && orders.Count < maxOrders)
            {
                orders.Add(newOrder);
            }
            else
            {
                orderPool.Enqueue(newOrder);
            }
            float waitTime = orderGenerationInterval + Random.Range(-orderGenerationVariance, orderGenerationVariance);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Order GenerateOrder()
    {
        OrderType orderType = (OrderType)Random.Range(0, System.Enum.GetValues(typeof(OrderType)).Length);

        switch(orderType)
        {
            case OrderType.Normal:
                return GenerateNormalOrder();
            case OrderType.Large:
                return GenerateLargeOrder();
            case OrderType.Rush:
                return GenerateRushOrder();
            case OrderType.Variety:
                return GenerateVarietyOrder();
            case OrderType.Bulk:
                return GenerateBulkOrder();
        }
        return null;
    }

    private Order GenerateNormalOrder()
    {
        throw new System.NotImplementedException("Normal order generation is not implemented yet.");    
    }

    private Order GenerateLargeOrder()
    {
        throw new System.NotImplementedException("Large order generation is not implemented yet.");
    }

    private Order GenerateRushOrder()
    {
        throw new System.NotImplementedException("Rush order generation is not implemented yet.");
    }

    private Order GenerateVarietyOrder()
    {
        throw new System.NotImplementedException("Variety order generation is not implemented yet.");
    }
    
    private Order GenerateBulkOrder()
    {
        throw new System.NotImplementedException("Bulk order generation is not implemented yet.");
    }
}
