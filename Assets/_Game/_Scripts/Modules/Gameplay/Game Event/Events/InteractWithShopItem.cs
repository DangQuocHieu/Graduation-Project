using DQHieu.Framework;
using UnityEngine;

public struct InteractWithShopItemEvent : IGameEvent
{
    public ShopItem shopItem;
    public InteractWithShopItemEvent(ShopItem shopItem)
    {
        this.shopItem = shopItem;
    }
}
