using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomEditor(typeof(ArmatureAsset))]
    public class ArmatureAssetEditor : Editor
    {
        private SerializedProperty _parentProp;
        private SerializedProperty _bonesProp;
        private void OnEnable()
        {
            _bonesProp = serializedObject.FindProperty("m_Bones");
            _parentProp = serializedObject.FindProperty("m_Parent");
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

        void ParentGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_parentProp);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        public override void OnInspectorGUI()
        {
            ImportFoldoutGUI();
            EditorGUILayout.Separator();

            ParentGUI();

            serializedObject.Update();
            if (_bonesProp == null) return;

            _bonesProp.isExpanded =
                EditorGUILayout.BeginFoldoutHeaderGroup(_bonesProp.isExpanded, $"Bones ({_bonesProp.arraySize})");

            if (_bonesProp.isExpanded)
            {
                for (int idx = 0; idx < _bonesProp.arraySize; ++idx)
                {
                    var prop = _bonesProp.GetArrayElementAtIndex(idx);
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
                        
                        _bonesProp.DeleteArrayElementAtIndex(idx);
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
            if (_bonesProp == null)
                return;

            serializedObject.Update();
            
            for (int idx = _bonesProp.arraySize - 1; idx > 0; --idx)
                _bonesProp.DeleteArrayElementAtIndex(idx);

            _bonesProp.arraySize = 0;

            serializedObject.ApplyModifiedProperties();
        }

        void Import(Transform source)
        {
            serializedObject.Update();

            if(!ShowDestructiveDialog())
                return;
            
            if (_bonesProp == null)
                return;

            Clear();

            (target as ArmatureAsset)?.Import(source, _includeRootBone);

            serializedObject.ApplyModifiedProperties();
        }
    }
}