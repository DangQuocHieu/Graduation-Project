namespace DQHieu.Framework
{
    using UnityEngine;
    using DG.Tweening; // Nhớ thêm namespace này của DOTween

    public class ScaleUpDown : MonoBehaviour
    {
        [Header("Scale Settings")]
        [SerializeField] private Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f); // Kích thước lớn nhất khi scale lên
        [SerializeField] private float duration = 1.0f; // Thời gian hoàn thành 1 chu kỳ scale (giây)

        [Header("Tween Settings")]
        [SerializeField] private Ease easeType = Ease.InOutQuad; // Kiểu mượt mà ở hai đầu (giúp hiệu ứng tự nhiên hơn)

        private void Start()
        {
            // Thực hiện scale từ kích thước hiện tại đến targetScale
            transform.DOScale(targetScale, duration)
                .SetEase(easeType)
                .SetLoops(-1, LoopType.Yoyo); // -1 là lặp vô hạn, LoopType.Yoyo giúp nó tự động scale ngược lại kích thước cũ
        }

        private void OnDestroy()
        {
            // Tốt nhất là nên kill tween khi object bị hủy để tránh lỗi rác bộ nhớ (Memory Leak)
            transform.DOKill();
        }
    }
}