using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace CoreGame.Movement
{
    public class KCCManager : MonoBehaviour
    {
        public PlayerMovement Character;
        public KCCCamera CharacterCamera;
        
        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        // Thêm biến cờ để kiểm soát việc chặn input
        private bool isInputBlocked = false;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            // Vẫn cho phép click chuột để lock trỏ chuột kể cả khi bị chặn input (tùy chọn)
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            float mouseLookAxisUp = 0f;
            float mouseLookAxisRight = 0f;
            float scrollInput = 0f;

            // Chỉ đọc input camera nếu không bị chặn
            if (!isInputBlocked)
            {
                mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
                mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);

                // Handle toggling zoom level
                if (Input.GetMouseButtonDown(1))
                {
                    CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
                }
            }

            // Create the look input vector for the camera
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

#if UNITY_WEBGL
            scrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Vẫn luôn cần truyền CameraRotation để KCC biết hướng của nhân vật
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;

            // Chỉ lấy các input di chuyển/hành động nếu không bị chặn
            if (!isInputBlocked)
            {
                characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
                characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
                characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
                characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
                characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
            }
            else
            {
                characterInputs.MoveAxisForward = 0f;
                characterInputs.MoveAxisRight = 0f;
                characterInputs.JumpDown = false;
                characterInputs.CrouchDown = false;
                characterInputs.CrouchUp = false;
            }

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        // --- CÁC PHƯƠNG THỨC ĐIỀU KHIỂN INPUT ---

        /// <summary>
        /// Gọi hàm này để chặn mọi thao tác điều khiển (ví dụ: khi mở UI, xem Cutscene)
        /// </summary>
        public void BlockInput()
        {
            isInputBlocked = true;
        }

        /// <summary>
        /// Gọi hàm này để trả lại quyền điều khiển cho người chơi
        /// </summary>
        public void UnblockInput()
        {
            isInputBlocked = false;
        }

        /// <summary>
        /// Kiểm tra xem input có đang bị chặn hay không
        /// </summary>
        public bool IsInputBlocked()
        {
            return isInputBlocked;
        }
    }
}