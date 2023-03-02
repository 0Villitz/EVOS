
using Game2D;
using UnityEditor;
using UnityEngine;

namespace Game.Room.Builder.Editor
{
    [CustomEditor(typeof(RoomNavBuilder))]
    public class RoomNavBuilderEditor : UnityEditor.Editor
    {   
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            if (Application.isPlaying && target is RoomNavBuilder builder)
            {
                if (builder.buildCamera == null)
                {
                    EditorGUILayout.HelpBox("Missing camera to ray cast from", MessageType.Error);                    
                }
                
                if (builder.navigationController == null)
                {
                    EditorGUILayout.HelpBox("Missing " + typeof(NavigationController), MessageType.Error);                    
                }
                
                EditorGUI.BeginDisabledGroup(builder.buildCamera == null || builder.navigationController == null);
                DisplayCreateOptions(builder);
                EditorGUI.EndDisabledGroup();
            }
        }
        
        private void DisplayCreateOptions(RoomNavBuilder builder)
        {   
            EditorGUILayout.LabelField("Current Node Id: " + builder.currentId);
            string buttonText = builder.building ? "Stop Building" : "Start Building";
            if (GUILayout.Button(buttonText))
            {
                builder.SetBuildingFlag(!builder.building);
            }
            
        
            if (builder.building)
            {
                EditorGUILayout.HelpBox(
                    "Click on " + builder.gameObject.name + " object to place navigation nodes",
                    MessageType.Info
                );
                
                if (GUILayout.Button("Discard Map"))
                {
                    builder.Discard();
                }
            }
        
            EditorGUI.BeginDisabledGroup(!builder.building);
            if (GUILayout.Button("Save Prefab"))
            {
                builder.SavePrefab();
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}