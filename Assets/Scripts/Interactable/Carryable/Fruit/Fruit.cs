using System.Collections;
using UnityEngine;

public enum FruitType
{
    Carrot,
    Tomato,
    Potato,
}

public class Fruit : MonoBehaviour
{
    public FruitType Type { get => type; }

    [SerializeField] private FruitType type;
    [SerializeField] private int sellValue = 1;
    [SerializeField] private int spoilSellValue = 0;
    [SerializeField] private float spoilTime = 10f;
    [SerializeField] private float spoilTimeVariance = 5f;
    [SerializeField] private IntEvent onSell;

    private bool isSpoiled = false;

    private void Start()
    {
        if (spoilTime <= 0f) return;

        StartCoroutine(SpoilCoroutine(spoilTime + Random.Range(-spoilTimeVariance, spoilTimeVariance)));
    }

    public void Sell()
    {
        onSell.RaiseEvent(isSpoiled ? spoilSellValue : sellValue);
        Destroy(gameObject);
    }

    private IEnumerator SpoilCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        isSpoiled = true;
    }
}