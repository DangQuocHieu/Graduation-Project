using UnityEngine;

public class GrabbableObjectHover : MonoBehaviour
{
    public GrabbableObject grabbableObject;
    
    void Awake()
    {
        grabbableObject = GetComponent<GrabbableObject>();
    }
}
