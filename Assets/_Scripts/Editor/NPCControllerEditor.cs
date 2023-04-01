
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace Game2D.Editor
{
    [CustomEditor(typeof(NPCController))]
    public class NPCControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                NPCController controller = target as NPCController;
                DrawHelperInspector(controller);
            }
        }
        
        private void DrawHelperInspector(NPCController controller)
        {
            EditorGUI.BeginDisabledGroup(true);
            Transform playerTransform = controller.Player?.GetTransform();
            EditorGUILayout.ObjectField(playerTransform, typeof(Transform), true);
            EditorGUI.EndDisabledGroup();
        }
    }
}