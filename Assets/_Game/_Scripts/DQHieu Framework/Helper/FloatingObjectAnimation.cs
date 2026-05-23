namespace DQHieu.Framework
{
    using UnityEngine;
    using DG.Tweening; 

    public class FloatingObjectAnimation : MonoBehaviour
    {
        [Header("Cấu hình di chuyển")]
        [SerializeField] private float moveDistance = 0.5f; 
        [SerializeField] private float duration = 1.5f;     

        private void Start()
        {
            float startY = transform.position.y;
            transform.DOMoveY(startY + moveDistance, duration)
                .SetEase(Ease.InOutQuad) 
                .SetLoops(-1, LoopType.Yoyo); 
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }
    }
}