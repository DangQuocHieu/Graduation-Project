namespace DQHieu.Framework
{
    using DG.Tweening;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ToggleButton : MonoBehaviour
    {

        [Title("Button Properties")]
        public Image handleRectImage;
        public TextMeshProUGUI buttonStateText;
        public Color onColor;
        public Color offColor;
        public Button button;

        [Title("Animations")]
        public float onPositionX;
        public float offPositionX;
        public Ease slideEase = Ease.InOutQuad;
        public float slideDuration = 0.25f;

        [Title("Runtime Tracking")]
        public bool isOn = true;
        private RectTransform handleRectTransform;

        void Awake()
        {
            handleRectTransform = handleRectImage.rectTransform;
            
        }
        void OnEnable()
        {
            button.onClick.AddListener(Toggle);
        }

        void OnDisable()
        {
            button.onClick.RemoveListener(Toggle);
        }

        public System.Action<bool> onValueChanged;

        private void Toggle()
        {
            isOn = !isOn;
            UpdateVisuals(instant: false);
            onValueChanged?.Invoke(isOn);
        }

        private void UpdateVisuals(bool instant)
        {
            if (handleRectImage != null) handleRectImage.color = isOn ? onColor : offColor;
            if (buttonStateText != null) buttonStateText.text = isOn ? "ON" : "OFF";
            if (handleRectTransform != null)
            {
                handleRectTransform.DOKill();
                float targetX = isOn ? onPositionX : offPositionX;
                if (instant)
                {
                    Vector2 currentPos = handleRectTransform.anchoredPosition;
                    handleRectTransform.anchoredPosition = new Vector2(targetX, currentPos.y);
                }
                else
                {
                    handleRectTransform.DOAnchorPosX(targetX, slideDuration).SetEase(slideEase);
                }
            }

        }


        public void SetState(bool state, bool instant = true)
        {
            isOn = state;
            UpdateVisuals(instant);
        }
    }

}