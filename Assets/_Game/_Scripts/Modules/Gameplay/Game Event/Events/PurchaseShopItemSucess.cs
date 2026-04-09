using DQHieu.Framework;
using UnityEngine;

public struct PurchaseShopItemSucess : IGameEvent
{
    public GrabbableObject purchasedObject;
    public PurchaseShopItemSucess(GrabbableObject purchasedObject)
    {
        this.purchasedObject = purchasedObject;
    }
}
