using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int startingCurrency = 0;
    [SerializeField] private int startingLife = 3;

    [Header("Game Stage Settings")]
    [SerializeField] private int ordersPerStage = 5; // Number of orders to complete before advancing to the next stage
    [SerializeField] private float timeScaleIncrement = 0.1f; // Amount to increase time scale each stage

    [Header("UI References")]
    [SerializeField] private TMP_Text currencyText;

    [Header("Events")]
    [SerializeField] private IntEvent onCurrencyUpdate;
    [SerializeField] private Event onOrderFail;
    [SerializeField] private Event onGameOver;
    [SerializeField] private Event onPause;

    [SerializeField] private Event onShopOpen;
    [SerializeField] private Event onShopClose;
    [SerializeField] private Event onGameStageIncrement;


    private int currency = 0;
    private int life = 0;

    private float timeScale = 1f;

    public void UpdateGameStage()
    {
        timeScale += timeScaleIncrement;
        Time.timeScale = timeScale;
        onGameStageIncrement?.RaiseEvent();
    }

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
        GameStateManager.Instance.Initialize(this, ordersPerStage);
        currency = startingCurrency;
        currencyText.text = currency.ToString();
        life = startingLife;
    }

    private void OnShopOpen()
    {
        SetGamePaused(true);
        GameStateManager.Instance.IsShopOpen = true;
    }

    private void OnShopClose()
    {
        SetGamePaused(false);
        GameStateManager.Instance.IsShopOpen = false;
    }

    private void OnPauseEvent()
    {
        SetGamePaused(!GameStateManager.Instance.IsPaused);
    }

    private void SetGamePaused(bool isPaused)
    {
        Time.timeScale = isPaused ? 0f : 1f;
        GameStateManager.Instance.IsPaused = isPaused;
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
