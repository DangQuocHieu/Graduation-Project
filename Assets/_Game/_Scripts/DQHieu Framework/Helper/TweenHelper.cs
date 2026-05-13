namespace DQHieu.Framework
{
    using UnityEngine;
    using DG.Tweening;
    using System;
    
    public static class TweenHelper
    {
        public static Tween AnimateInt(int startValue, int endValue, float duration, Action<int> onUpdate)
        {
            int currentValue = startValue;
            onUpdate?.Invoke(currentValue); 

            return DOTween.To(() => currentValue, x =>
            {
                currentValue = x;
                onUpdate?.Invoke(currentValue);
            }, endValue, duration).SetEase(Ease.OutQuad);
        }
    }

}