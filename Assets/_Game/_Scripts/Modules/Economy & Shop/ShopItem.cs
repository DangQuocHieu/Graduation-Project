using UnityEngine;
using DQHieu.Framework;

[RequireComponent(typeof(ObjectHover))]
public class ShopItem : MonoBehaviour
{
    public ShopItemSO shopItemSO;

    public void HandlePurchaseItem(bool success)
    {
        if (success)
        {
            GrabbableObject purchasedObject = Instantiate(shopItemSO.itemPrefab, transform.position, Quaternion.identity, null);
            Debug.Log("Purchase: " + purchasedObject);
            EventBus.SendMessage<PurchaseShopItemSucess>(new PurchaseShopItemSucess(purchasedObject));
        }
        else
        {

        }
    }
}
