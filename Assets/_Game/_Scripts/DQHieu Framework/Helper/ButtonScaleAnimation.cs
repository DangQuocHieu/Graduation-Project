namespace DQHieu.Framework
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening; // Nhớ import thư viện DOTween

    [RequireComponent(typeof(RectTransform))]
    public class ButtonScaleAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [Tooltip("Tỷ lệ scale khi nhấn xuống (ví dụ 0.9 = giảm 10% kích thước)")]
        [SerializeField] private float scaleMultiplier = 0.9f;

        [Tooltip("Thời gian thực hiện animation")]
        [SerializeField] private float animationDuration = 0.1f;

        [Tooltip("Đường cong animation (Ease out tạo cảm giác mượt và có lực hơn)")]
        [SerializeField] private Ease easeType = Ease.OutQuad;

        private Vector3 originalScale;

        private void Awake()
        {
            // Lưu lại scale ban đầu của button
            originalScale = transform.localScale;
        }

        // Khi người chơi nhấn chuột/chạm tay xuống
        public void OnPointerDown(PointerEventData eventData)
        {
            // Dừng tất cả các animation đang chạy trên Transform này để tránh lỗi giật lag nếu bấm quá nhanh
            transform.DOKill();

            // Scale nhỏ lại theo tỷ lệ
            transform.DOScale(originalScale * scaleMultiplier, animationDuration).SetEase(easeType);
        }

        // Khi người chơi nhả chuột/nhấc tay lên
        public void OnPointerUp(PointerEventData eventData)
        {
            ReturnToOriginalScale();
        }

        // Xử lý trường hợp người chơi nhấn giữ nhưng kéo chuột/tay ra khỏi phạm vi của button
        public void OnPointerExit(PointerEventData eventData)
        {
            ReturnToOriginalScale();
        }

        private void ReturnToOriginalScale()
        {
            transform.DOKill();

            // Scale trở lại kích thước ban đầu với thời gian và ease tương tự
            // Có thể đổi Ease thành OutBack để tạo hiệu ứng nảy (bouncy) nhẹ nếu muốn
            transform.DOScale(originalScale, animationDuration).SetEase(easeType);
        }

        private void OnDisable()
        {
            // Đảm bảo khi button bị ẩn đi, nó sẽ lập tức reset về scale gốc và dọn dẹp tween
            transform.DOKill();
            transform.localScale = originalScale;
        }
    }
}