using DQHieu.Framework;
using UnityEngine;

public class EconomySystem : MonoBehaviour
{
    private DataManager dataManager;

    void OnEnable()
    {
        EventBus.Subcribe<InteractWithShopItemEvent>(HandleInteractWithShopItemEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<InteractWithShopItemEvent>(HandleInteractWithShopItemEvent);
    }

    public void Initialize(DataManager dataManager)
    {
        this.dataManager = dataManager;
    }
    public bool TrySpendMoney(int money)
    {
        if(dataManager.playerData.Money >= money)
        {
            dataManager.playerData.Money -= money;
            return true;
        }
        return false;
    }

    public void TryPurchaseItem(ShopItem shopItem)
    {
        if(TrySpendMoney(shopItem.shopItemSO.grabbableObjectSO.price))
        {
            shopItem.HandlePurchaseItem(true);   
        }
        else
        {
            shopItem.HandlePurchaseItem(false);
        }
    }

    public void HandleInteractWithShopItemEvent(InteractWithShopItemEvent evt)
    {
        TryPurchaseItem(evt.shopItem);
    }
}
