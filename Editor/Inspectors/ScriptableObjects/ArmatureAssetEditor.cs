using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomEditor(typeof(ArmatureAsset))]
    public class ArmatureAssetEditor : Editor
    {
        private SerializedProperty bonesProp;
        private void OnEnable()
        {
            bonesProp = serializedObject.FindProperty("m_Bones");
        }

        Transform _sourceTransform;
        private bool _importExpanded;
        private bool _includeRootBone;
        
        void ImportFoldoutGUI()
        {
            _importExpanded =
                EditorGUILayout.BeginFoldoutHeaderGroup(_importExpanded, "Import");
            if (_importExpanded)
            {
                
                _sourceTransform =
                    EditorGUILayout.ObjectField(new GUIContent("Source"), _sourceTransform, typeof(Transform), true) as
                        Transform;
                
                EditorGUILayout.BeginHorizontal();
                _includeRootBone = EditorGUILayout.ToggleLeft(new GUIContent("Include Root Bone"), _includeRootBone);
                if (_sourceTransform)
                {
                    GUILayout.FlexibleSpace();
                    bool importClicked = GUILayout.Button("Import", GUILayout.Width(120));
                    if (importClicked)
                        Import(_sourceTransform);
                }
                EditorGUILayout.EndHorizontal();
                
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        
        protected bool ShowDestructiveDialog()
        {
            return Event.current.shift || EditorUtility.DisplayDialog("Destructive Action",
                "This action is destructive. Are you sure you want to continue?", 
                "Yes", "No");
        }

        public override void OnInspectorGUI()
        {
            ImportFoldoutGUI();
            EditorGUILayout.Separator();

            serializedObject.Update();

            if (bonesProp == null) return;

            bonesProp.isExpanded =
                EditorGUILayout.BeginFoldoutHeaderGroup(bonesProp.isExpanded, $"Bones ({bonesProp.arraySize})");

            if (bonesProp.isExpanded)
            {
                for (int idx = 0; idx < bonesProp.arraySize; ++idx)
                {
                    var prop = bonesProp.GetArrayElementAtIndex(idx);
                    var nameProp = prop.FindPropertyRelative("name");
                    var bindPosProp = prop.FindPropertyRelative("bindPos");

                    EditorGUILayout.BeginHorizontal();
                    Color c = GUI.backgroundColor;
                    bool isButtonPressed = GUILayout.Button(nameProp.stringValue, EditorStyles.foldoutHeader);
                    if (isButtonPressed)
                        bindPosProp.isExpanded = !bindPosProp.isExpanded;

                    GUI.backgroundColor = new Color(1,0.5f,0.5f) * 1.5f;
                    GUILayout.Space(5);
                    bool isRemovePressed = GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20));
                    GUI.backgroundColor = c;
                    
                    if (isRemovePressed)
                    {
                        if(!ShowDestructiveDialog())
                            return;
                        
                        bonesProp.DeleteArrayElementAtIndex(idx);
                        serializedObject.ApplyModifiedProperties();
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    if (bindPosProp.isExpanded)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.PropertyField(bindPosProp);
                        EditorGUILayout.EndVertical();
                    }
                }
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            //base.OnInspectorGUI();
        }

        void Clear()
        {
            if (bonesProp == null)
                return;

            serializedObject.Update();
            
            for (int idx = bonesProp.arraySize - 1; idx > 0; --idx)
                bonesProp.DeleteArrayElementAtIndex(idx);

            bonesProp.arraySize = 0;

            serializedObject.ApplyModifiedProperties();
        }

        void Import(Transform source)
        {
            serializedObject.Update();

            if(!ShowDestructiveDialog())
                return;
            
            if (bonesProp == null)
                return;

            Clear();

            (target as ArmatureAsset)?.Import(source, _includeRootBone);

            serializedObject.ApplyModifiedProperties();
        }
    }
}