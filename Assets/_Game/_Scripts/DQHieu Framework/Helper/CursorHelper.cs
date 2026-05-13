namespace DQHieu.Framework
{
    using UnityEngine;

    public static class CursorHelper
    {
        public static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void ConfineCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        public static void ToggleCursor()
        {
            if (Cursor.visible)
                HideCursor();
            else
                ShowCursor();
        }
    }
}