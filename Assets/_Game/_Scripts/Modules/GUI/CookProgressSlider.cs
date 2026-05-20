using UnityEngine;
using UnityEngine.UI;
public class CookProgressSlider : MonoBehaviour
{
    public Slider welldoneSlider;
    public Slider burnSlider;
    void Awake()
    {
        welldoneSlider.minValue = 0f;
        welldoneSlider.maxValue = 1f;
        
        burnSlider.minValue = 1f;
        burnSlider.maxValue = 2f;
    }
    public void Display(CookableObject cookableObject)
    {
        welldoneSlider.value = cookableObject.welldoneProgress;
        burnSlider.value = cookableObject.burnProgress;

    }

}
