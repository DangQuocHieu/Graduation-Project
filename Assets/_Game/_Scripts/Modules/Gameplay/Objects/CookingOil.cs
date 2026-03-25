using UnityEngine;

public class CookingOil : MonoBehaviour
{
    public FryingPan attachedFryingPan;

    public bool isHot = false;

    void Update()
    {
        isHot = attachedFryingPan != null && attachedFryingPan.isHot;
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if(!isHot) return;
        if(other.TryGetComponent<CookableObject>(out var cookableObject))
        {
            cookableObject.StartCooking();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<CookableObject>(out var cookableObject))
        {
            cookableObject.PauseCooking();
        }
    }
}
