using System.Collections.Generic;
using DQHieu.Framework;
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
    Buy,
    Error,
    Pour
}

public class CrosshairManager : SerializedMonoBehaviour
{
    public Dictionary<CrosshairType, Image> crossHairImageDic = new();
    public PickupAndDropHandler pickupAndDropHandler;
    public float rayDistance;
    public bool isPouring = false;

    [ShowInInspector] public GrabbableObject objectInHand => pickupAndDropHandler._objectInHand;

    private Camera mainCam;
    private CrosshairType currentCrosshairType = CrosshairType.Dot; // Lưu trạng thái hiện tại để tránh lặp UI liên tục

    void Awake()
    {
        mainCam = Camera.main;
        ResetAllCrosshairs();
    }

    void OnEnable()
    {
        EventBus.Subcribe<PourLiquid>(HandlePourLiquidEvent);
        EventBus.Subcribe<PourLiquidComplete>(HandlePourLiquidCompleteEvent);
    }

    void OnDisable()
    {
        EventBus.UnSubcribe<PourLiquid>(HandlePourLiquidEvent);
        EventBus.UnSubcribe<PourLiquidComplete>(HandlePourLiquidCompleteEvent);
    }
    void Update()
    {

        // Mặc định ban đầu nếu không bắn trúng gì là Dot
        CrosshairType targetCrosshair = CrosshairType.Dot;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));


        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            targetCrosshair = EvaluateCrosshairTarget(hit);

        }

        if (targetCrosshair != currentCrosshairType)
        {
            SetCrosshairActive(targetCrosshair);
        } 


    }

    // Tách riêng logic xử lý Raycast để code dễ đọc, dễ bảo trì
    private CrosshairType EvaluateCrosshairTarget(RaycastHit hit)
    {
        if (isPouring)
        {
            return CrosshairType.Pour;
        }
        Collider hitCollider = hit.collider;

        // 1. Kiểm tra GrabbableObject
        if (hitCollider.TryGetComponent<GrabbableObject>(out var grabbableObject) ||
            (hitCollider.attachedRigidbody != null && hitCollider.attachedRigidbody.TryGetComponent(out grabbableObject)))
        {
            if (objectInHand == null)
                return CrosshairType.Pickup;

            if (objectInHand is Ingredient ingredient)
            {
                if (grabbableObject is CuttingBoard)
                    return CrosshairType.Place;
                if (ingredient.cookableObject == null && grabbableObject is FryingPan)
                {
                    return CrosshairType.Error;
                }
                if (ingredient.cookableObject != null && grabbableObject is FryingPan fryingPan)
                {
                    if (fryingPan.containCookingOil)
                    {
                        return CrosshairType.Place;
                    }
                    else
                    {
                        return CrosshairType.Error;
                    }
                }

                return CrosshairType.Dot;
            }

            if (objectInHand is SauceBottle && grabbableObject is BambooTray)
                return CrosshairType.Place;

            if (objectInHand is KnifeObject && grabbableObject.canSliced)
                return CrosshairType.Knife;

            if (objectInHand is CookingOilBottle)
            {
                if (grabbableObject is FryingPan fryingPan)
                {
                    if (!fryingPan.containCookingOil)
                    {
                        return CrosshairType.Pour;
                    }
                    else
                    {
                        return CrosshairType.Error;
                    }
                }
            }

            if (objectInHand is SauceBottle)
            {
                if (grabbableObject is SauceBowl sauceBowl)
                {
                    if (!sauceBowl.containSauce)
                    {
                        return CrosshairType.Pour;
                    }
                    else
                    {
                        return CrosshairType.Error;
                    }
                }
            }

            return CrosshairType.Dot;
        }

        // 2. Kiểm tra ShopItem
        if (hitCollider.TryGetComponent<ShopItem>(out _))
        {
            return objectInHand == null ? CrosshairType.Buy : CrosshairType.Dot;
        }

        // 3. Kiểm tra Customer
        if (hitCollider.TryGetComponent<Customer>(out _))
        {
            return (objectInHand != null && objectInHand is BambooTray) ? CrosshairType.Served : CrosshairType.Dot;
        }

        // 4. Kiểm tra Spawner
        if (hitCollider.TryGetComponent<GrabbableObjectSpawner>(out _))
        {
            if (objectInHand == null)
                return CrosshairType.Pickup;
        }

        // 5. Kiểm tra CookingZone
        if (hitCollider.TryGetComponent<CookingZone>(out _))
        {
            if (objectInHand != null && objectInHand is FryingPan)
                return CrosshairType.Place;
        }


        return CrosshairType.Dot;
    }

    private void SetCrosshairActive(CrosshairType crosshairType)
    {
        // Tắt crosshair cũ
        if (crossHairImageDic.TryGetValue(currentCrosshairType, out var oldImage) && oldImage != null)
        {
            oldImage.gameObject.SetActive(false);
        }

        // Bật crosshair mới
        if (crossHairImageDic.TryGetValue(crosshairType, out var newImage) && newImage != null)
        {
            newImage.gameObject.SetActive(true);
        }

        currentCrosshairType = crosshairType;
    }

    private void ResetAllCrosshairs()
    {
        foreach (var pair in crossHairImageDic)
        {
            if (pair.Value != null)
                pair.Value.gameObject.SetActive(pair.Key == currentCrosshairType);
        }
    }

    private void HandlePourLiquidEvent(PourLiquid evt)
    {
        isPouring = true;
    }

    private void HandlePourLiquidCompleteEvent(PourLiquidComplete evt)
    {
        isPouring = false;
    }
}