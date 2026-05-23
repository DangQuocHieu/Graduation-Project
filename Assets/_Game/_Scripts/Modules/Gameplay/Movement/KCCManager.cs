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

        // Tách biệt hai cờ kiểm soát để quản lý độc lập
        private bool isMoveInputBlocked = false;
        private bool isLookInputBlocked = false;

        private void Start()
        {
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
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

            // Chỉ đọc input camera nếu LOOK không bị chặn
            if (!isLookInputBlocked)
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

            // Chỉ lấy các input di chuyển/hành động nếu MOVE không bị chặn
            if (!isMoveInputBlocked)
            {
                characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
                characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
                characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
                characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.LeftShift);
                characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.LeftShift);
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

        // --- CÁC PHƯƠNG THỨC ĐIỀU KHIỂN INPUT MỚI ---

        /// <summary>
        /// Chặn hoặc mở chặn riêng input di chuyển và nhảy/ngồi
        /// </summary>
        public void SetMoveInputBlocked(bool isBlocked)
        {
            isMoveInputBlocked = isBlocked;
        }

        /// <summary>
        /// Chặn hoặc mở chặn riêng input xoay góc nhìn camera
        /// </summary>
        public void SetLookInputBlocked(bool isBlocked)
        {
            isLookInputBlocked = isBlocked;
        }

        /// <summary>
        /// Hàm tiện ích để chặn/mở chặn nhanh cả hai cùng lúc khi cần (như mở Menu chính)
        /// </summary>
        public void SetAllInputBlocked(bool isBlocked)
        {
            isMoveInputBlocked = isBlocked;
            isLookInputBlocked = isBlocked;
        }

        // --- PROPERTY KIỂM TRA TRẠNG THÁI ---
        public bool IsMoveInputBlocked => isMoveInputBlocked;
        public bool IsLookInputBlocked => isLookInputBlocked;
    }
}