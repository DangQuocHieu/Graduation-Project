using UnityEngine;
using UnityEngine.UI;
public class IngredientAmountSlider : MonoBehaviour
{
    public Slider slider;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
    }
    public void UpdateValue(int value)
    {
        slider.value = value;
    }

}
