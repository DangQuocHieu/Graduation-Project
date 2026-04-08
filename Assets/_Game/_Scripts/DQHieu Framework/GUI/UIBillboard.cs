namespace DQHieu.Framework
{
    using UnityEngine;

    public class UIBillboard : MonoBehaviour
    {
        private Camera mainCamera;

        void Start()
        {
            // Gán biến mainCamera là camera chính trong Scene
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            // Đảm bảo camera tồn tại trước khi xoay
            if (mainCamera != null)
            {
                // Xoay Transform của UI sao cho nó đồng bộ với hướng nhìn của Camera
                transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                 mainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}