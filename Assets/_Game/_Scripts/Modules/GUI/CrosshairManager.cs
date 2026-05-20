using System.Collections.Generic;
using DG.Tweening.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
public enum CrosshairType
{
    Dot,
    Pickup,
    Place,
    Knife,
    Served,
    Buy
}
public class CrosshairManager : SerializedMonoBehaviour
{
    public Dictionary<CrosshairType, Image> crossHairImageDic = new();
    public PickupAndDropHandler pickupAndDropHandler;
    private Camera mainCam;
    public float rayDistance;

    [ShowInInspector] public GrabbableObject objectInHand => pickupAndDropHandler._objectInHand;


    void Awake()
    {
        mainCam = Camera.main;
    } 
 
    void Update()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.TryGetComponent<GrabbableObject>(out var grabbableObject))
            {
                if(objectInHand == null)
                {
                    DisplayCrosshair(CrosshairType.Pickup);
                }
                else
                {
                    if(objectInHand is Ingredient && (grabbableObject is CuttingBoard or BambooTray))
                    {
                        DisplayCrosshair(CrosshairType.Place);
                    }
                
                    else if(objectInHand is SauceBottle && grabbableObject is BambooTray)
                    {
                        DisplayCrosshair(CrosshairType.Place);
                    }
                    else if(objectInHand is KnifeObject && grabbableObject.canSliced)
                    {
                        DisplayCrosshair(CrosshairType.Knife);
                    }
                    else
                    {
                        DisplayCrosshair(CrosshairType.Dot);
                    }
                }
            }
            else if (hit.collider.TryGetComponent<ShopItem>(out var shopItem))
            {
                if(objectInHand == null)
                {
                    DisplayCrosshair(CrosshairType.Buy);
                }
                else
                {
                    DisplayCrosshair(CrosshairType.Dot);
                }
            }
            else if(hit.collider.TryGetComponent<Customer>(out var customer))
            {
                if(objectInHand != null && objectInHand is BambooTray)
                {
                    DisplayCrosshair(CrosshairType.Served);
                }
                else
                {
                    DisplayCrosshair(CrosshairType.Dot);
                }
            }
            else if(hit.collider.TryGetComponent<GrabbableObjectSpawner>(out var spawner))
            {
                if(objectInHand == null)
                DisplayCrosshair(CrosshairType.Pickup);
            }
            else if(hit.collider.TryGetComponent<CookingZone>(out var cookingZone))
            {
                if(objectInHand != null && objectInHand is FryingPan)
                {
                    DisplayCrosshair(CrosshairType.Place);
                }
            }
            else
            {
                DisplayCrosshair(CrosshairType.Dot);
            }

          
        }
    


    }

    private void DisplayCrosshair(CrosshairType crosshairType)
    {
        foreach (var image in crossHairImageDic.Values)
        {
            image.gameObject.SetActive(false);
        }
        crossHairImageDic[crosshairType].gameObject.SetActive(true);
    }
}
