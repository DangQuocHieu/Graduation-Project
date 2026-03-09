namespace DQHieu.Framework
{
    using Sirenix.OdinInspector;
    using UnityEngine;
    public class FirstPersonController : MonoBehaviour
    {
        [Title("Movement")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private float _moveSpeed = 5f;

        [Title("Mouse Look")]
        [SerializeField] private float _mouseSensitivity = 100f;
        [SerializeField] private Transform _camera;
        private float _xRotation = 0f;
        [SerializeField] private bool _lockCursor = true;

        void Awake()
        {
            if(_lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("Lock Cursor");
            }
        }

        private void Update()
        {
            HandleMovement();
        }

        void LateUpdate()
        {
            HandleMouseLook();
        }

        private void HandleMovement()
        {
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _rb.linearVelocity = transform.TransformDirection(moveDirection) * _moveSpeed;
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -75f, 75f);
            _camera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}