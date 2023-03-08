
using System;
using System.Collections.Generic;
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
                
                if (builder.LevelControllerPrefab == null)
                {
                    EditorGUILayout.HelpBox("Missing " + typeof(LevelController), MessageType.Error);                    
                }
                
                EditorGUI.BeginDisabledGroup(builder.buildCamera == null || builder.LevelControllerPrefab == null);
                DisplayCreateOptions(builder);
                EditorGUI.EndDisabledGroup();
            }
        }

        private string[] unitMovements = null;
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

                if (unitMovements == null)
                {
                    unitMovements = Enum.GetNames(typeof(UnitMovement));
                }

                List<string> cachedActions = new List<string>();
                foreach (UnitMovement unitMovement in builder.cachedNodeActions)
                {
                    cachedActions.Add(unitMovement.ToString());
                }
                cachedActions.Sort(
                    (x, y) => Enum.Parse<UnitMovement>(x).CompareTo(Enum.Parse<UnitMovement>(y))
                    );
                
                EditorGUILayout.HelpBox(string.Join(", ", cachedActions.ToArray()), MessageType.None);

                int editorIndex = EditorGUILayout.Popup(
                    nameof(UnitMovement), 
                    builder.nodeActionIndex, 
                    unitMovements
                );
                if (editorIndex != builder.nodeActionIndex)
                {
                    builder.nodeActionIndex = editorIndex;
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Action"))
                {
                    builder.AddNodeAction(Enum.Parse<UnitMovement>(unitMovements[builder.nodeActionIndex]));
                }
                if (GUILayout.Button("Remove Action"))
                {
                    builder.RemoveAction(Enum.Parse<UnitMovement>(unitMovements[builder.nodeActionIndex]));
                }
                EditorGUILayout.EndHorizontal();
                
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