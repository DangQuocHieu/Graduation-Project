using UnityEngine;

public class CookingOil : MonoBehaviour
{
    public FryingPan attachedFryingPan;

    public bool isHot = false;

    void Update()
    {
        isHot = attachedFryingPan != null && attachedFryingPan.isHot;
    }

}
