using UnityEngine;

public class CookingOil : MonoBehaviour
{
    public FryingPan attachedFryingPan;

    public bool isHot = false;
    public MeshCollider meshCollider;

    void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    void Update()
    {
        isHot = attachedFryingPan != null && attachedFryingPan.isHot;
    }

}
