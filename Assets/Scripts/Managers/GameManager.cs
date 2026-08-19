using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int startingCurrency = 0;

    [Header("Events")]
    [SerializeField] private IntEvent onCurrencyUpdate;
    [SerializeField] private Event onOrderFail;

    private int currency = 0;

    private void OnEnable()
    {
        onCurrencyUpdate.Subscribe(OnCurrencyUpdate);
    }

    private void OnDisable()
    {
        onCurrencyUpdate.Unsubscribe(OnCurrencyUpdate);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCurrencyUpdate(int changeValue)
    {
        currency += changeValue;
        onCurrencyUpdate.RaiseEvent(currency);
    }
}
