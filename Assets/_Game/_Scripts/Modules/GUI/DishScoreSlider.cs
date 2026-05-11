using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text; // Thêm thư viện này

public class DishScoreSlider : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI scoreText;
    public float displayValue;
    private StringBuilder _sb = new StringBuilder(3);

    public void SetUp(float displayValue)
    {
        slider.value = 0f;
        this.displayValue = displayValue;
        UpdateText(0);
    }

    public Tween AnimateScore()
    {
        slider.DOKill();
        return slider.DOValue(displayValue, 2f)
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                UpdateText((int)slider.value);
            });
    }

    private void UpdateText(int value)
    {
        _sb.Clear();
        _sb.Append(value);
        scoreText.SetText(_sb); 
    }
}