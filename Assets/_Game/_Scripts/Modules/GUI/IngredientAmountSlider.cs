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
        if(value == 0)
        {
            gameObject.SetActive(false);
        }
        slider.value = value;
    }

}
