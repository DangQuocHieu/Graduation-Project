using UnityEngine;

public class TakeOrderButton : MonoBehaviour
{
    public bool orderAccepted = false;
    
    public void OnClick()
    {
        orderAccepted = true;
    }
}
