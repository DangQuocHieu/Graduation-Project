namespace DQHieu.Framework
{
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SceneSwitcherWindow : EditorWindow
{
    private Vector2 scrollPos;

    [MenuItem("Tools/Scene Switcher")]
    public static void ShowWindow()
    {
        GetWindow<SceneSwitcherWindow>("Scene Switcher");
    }

    private void OnGUI()
    {
        GUILayout.Label("Available Scenes", EditorStyles.boldLabel);

        // Tạo vùng cuộn nếu bạn có quá nhiều scene
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Lấy danh sách các scene đã được add trong Build Settings
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        if (scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found in Build Settings. Please add them first!", MessageType.Warning);
        }

        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);

                // Vẽ dòng ngang phân cách
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button(sceneName, GUILayout.Height(30)))
                {
                    SwitchScene(scene.path);
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
        }

        EditorGUILayout.EndScrollView();

        // Nút refresh để cập nhật lại danh sách nếu cần
        if (GUILayout.Button("Refresh List", GUILayout.Height(40)))
        {
            Repaint();
        }
    }

    private void SwitchScene(string scenePath)
    {
        // Kiểm tra xem scene hiện tại đã lưu chưa
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
#endif
}