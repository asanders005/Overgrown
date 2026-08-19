using UnityEngine;

// Attribute that allows creating instances of this ScriptableObject through the Unity menu
// Creates a menu item at "Assets > Create > Data > OrderDefinition" with default filename "OrderDefinitionData"
[CreateAssetMenu(fileName = "OrderDefinitionData", menuName = "Data/OrderDefinitionData")]
public class OrderDefinitionData : ScriptableObjectBase
{
	[SerializeField] private OrderType type;

    [Header("Order Content Settings")]
    [SerializeField] private int fruitVarietyCount;
    [SerializeField] private int fruitVarietyVariance;
    [SerializeField] private int fruitCount;
    [SerializeField] private int fruitCountVariance;

    [Header("Time Limit Settings")]
    [SerializeField] private float timeLimit;
    [SerializeField] private float timeLimitVariance;

    [Header("Reward Settings")]
    [SerializeField] private int reward;
    [SerializeField] private float rewardVariance;

    public OrderType Type { get => type; }

    public int FruitVarietyCount { get => fruitVarietyCount; }
    public int FruitVarietyVariance { get => fruitVarietyVariance; }
    public int FruitCount { get => fruitCount; }
    public int FruitCountVariance { get => fruitCountVariance; }

    public float TimeLimit { get => timeLimit; }
    public float TimeLimitVariance { get => timeLimitVariance; }

    public int Reward { get => reward; }
    public float RewardVariance { get => rewardVariance; }
}