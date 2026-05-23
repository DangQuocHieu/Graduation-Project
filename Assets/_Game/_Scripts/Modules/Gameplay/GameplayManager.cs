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
    public TutorialManager tutorialManager;


    [Title("GUI")]
    public MoneyUIPanel moneyUIPanel;
    public SummaryScreen summaryScreen;
    public DishStatisticsScreen dishStatisticsScreen;


    void Awake()
    {
        CursorHelper.HideCursor();
        dataManager.LoadData();
        economySystem.Initialize(dataManager);
        moneyUIPanel.Initialize(dataManager);
        gameLoopManager.Initialize(orderManager, dataManager);
        levelStatisticsManager.Initialize(gameLoopManager.currentLevel);
        summaryScreen.Initialize(levelStatisticsManager);
        dishStatisticsScreen.Initialize(gameLoopManager);

        if(tutorialManager != null)
        {
            tutorialManager.StartTutorial();
        }

    }
}
