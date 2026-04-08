using DQHieu.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Title("References")]
    public DataManager dataManager;
    public EconomySystem economySystem;

    void Awake()
    {
        dataManager.LoadData();
        economySystem.Initialize(dataManager);
    }
}
