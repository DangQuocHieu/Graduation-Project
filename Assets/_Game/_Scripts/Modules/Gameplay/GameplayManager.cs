using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Title("References")]
    public DataManager dataManager;
    public EconomySystem economySystem;

    [Title("GUI")]
    public MoneyUIPanel moneyUIPanel;

    void Awake()
    {
        dataManager.LoadData();
        economySystem.Initialize(dataManager);
        moneyUIPanel.Initialize(dataManager);
    }
}
