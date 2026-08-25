using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int startingCurrency = 0;
    [SerializeField] private int startingLife = 3;

    [Header("UI References")]
    [SerializeField] private TMP_Text currencyText;

    [Header("Events")]
    [SerializeField] private IntEvent onCurrencyUpdate;
    [SerializeField] private Event onOrderFail;

    private int currency = 0;
    private int life = 0;

    private void OnEnable()
    {
        onCurrencyUpdate.Subscribe(OnCurrencyUpdate);
        onOrderFail.Subscribe(OnOrderFail);
    }

    private void OnDisable()
    {
        onCurrencyUpdate.Unsubscribe(OnCurrencyUpdate);
        onOrderFail.Unsubscribe(OnOrderFail);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currency = startingCurrency;
        currencyText.text = currency.ToString();
        life = startingLife;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCurrencyUpdate(int changeValue)
    {
        currency += changeValue;
        currencyText.text = currency.ToString();
    }

    private void OnOrderFail()
    {
        life--;
        if (life <= 0)
        {
            // Game Over
            Debug.Log("Game Over");
        }
    }
}
