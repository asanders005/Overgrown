using System.Collections;
using UnityEngine;

public enum FruitType
{
    None,
    Carrot,
    Tomato,
}

public abstract class Fruit : MonoBehaviour
{
    public abstract FruitType Type { get; }

    [SerializeField] private int sellValue = 1;
    [SerializeField] private int spoilSellValue = 0;
    [SerializeField] private float spoilTime = 10f;
    [SerializeField] private float spoilTimeVariance = 5f;
    [SerializeField] private FloatEvent onSell;

    private bool isSpoiled = false;

    private void Start()
    {
        if (spoilTime <= 0f) return;

        StartCoroutine(SpoilCoroutine(spoilTime + Random.Range(-spoilTimeVariance, spoilTimeVariance)));
    }

    public void Sell()
    {
        onSell.RaiseEvent(isSpoiled ? spoilSellValue : sellValue);
    }

    private IEnumerator SpoilCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        isSpoiled = true;
    }
}