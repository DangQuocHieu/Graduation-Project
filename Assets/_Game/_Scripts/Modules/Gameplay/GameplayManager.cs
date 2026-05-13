using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Title("References")]
    public DataManager dataManager;
    public EconomySystem economySystem;
    public OrderManager orderManager;
    public GameLoopManager gameLoopManager;
    public LevelStatisticsManager levelStatisticsManager;


    [Title("GUI")]
    public MoneyUIPanel moneyUIPanel;
    public SummaryScreen summaryScreen;

    void Awake()
    {
        CursorHelper.HideCursor();
        dataManager.LoadData();
        economySystem.Initialize(dataManager);
        moneyUIPanel.Initialize(dataManager);
        gameLoopManager.Initialize(orderManager);
        levelStatisticsManager.Initialize(gameLoopManager.currentLevel);
        summaryScreen.Initialize(levelStatisticsManager);

    }
}
