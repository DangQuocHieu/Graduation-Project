using DQHieu.Framework;
using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    private DataManager dataManager;

    public void Initialize(DataManager dataManager)
    {
        this.dataManager = dataManager;
    }
    public bool TrySpendMoney(int money)
    {
        if(dataManager.playerData.money >= money)
        {
            dataManager.playerData.money -= money;
            return true;
        }
        return false;
    }
}
