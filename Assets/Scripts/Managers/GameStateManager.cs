public class GameStateManager
{
    public static GameStateManager Instance { get; private set; } = new GameStateManager();

    public bool IsPaused { get; set; } = false;

    public bool IsShopOpen { get; set; } = false;

    public int GameStage { get => gameStage; }

    private int gameStage = 0;
    private int ordersPerStage = 5;
    private int ordersCompleted = 0;

    private GameManager gameManager;

    public void Initialize(GameManager manager, int ordersPerStage)
    {
        gameManager = manager;
        this.ordersPerStage = ordersPerStage;
    }

    public void IncrementGameStage()
    {
        gameStage++;
        gameManager.UpdateGameStage();
    }

    public void IncrementOrdersCompleted()
    {
        ordersCompleted++;
        if (ordersCompleted % ordersPerStage == 0)
        {
            IncrementGameStage();
        }
    }

    private GameStateManager()
    {
        // Private constructor to prevent instantiation
    }
}
