using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ArmatureRoot))]
    public class ArmatureRootEditor : Editor
    {
        private SerializedProperty _armatureAssetProp;
        private void OnEnable()
        {
            _armatureAssetProp = serializedObject.FindProperty("armatureAsset");
            Enable();
        }

        protected SerializedProperty GetArmatureAssetProperty()
        {
            return _armatureAssetProp;
        }
        
        protected ArmatureAsset GetArmatureAsset()
        {
            return (ArmatureAsset) _armatureAssetProp?.objectReferenceValue;
        }

        protected virtual void Enable()
        {
            
        }

        private readonly Color headerBgColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
        
        private string _searchString;
        private Vector2 _scrollPos = Vector2.zero;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawHeaderGUI();
            EditorGUILayout.Separator();
            SerializedProperty mappedBonesProp = serializedObject.FindProperty("m_MappedBones");
            mappedBonesProp.isExpanded = EditorGUILayout.Foldout(mappedBonesProp.isExpanded, "Mapped Bones");
            if (mappedBonesProp.isExpanded)
            {
                _searchString = EditorGUILayout.TextField(_searchString, EditorStyles.toolbarSearchField);
                
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (var s = new EditorGUILayout.ScrollViewScope(_scrollPos,
                               false, false,
                               GUILayout.MaxHeight(160)))
                    {

                        ArrayGUI<MappedBone>(mappedBonesProp, false, 
                            m =>
                            {
                                bool stringIsEmpty = string.IsNullOrEmpty(_searchString);
                                string boneName = m.boneName.ToLower();
                                return stringIsEmpty || boneName.Contains(_searchString.ToLower());
                            }, () =>
                            {
                                EditorGUILayout.LabelField(new GUIContent("There are no bones mapped to this armature."));
                            });

                        _scrollPos = s.scrollPosition;
                    }
                }
            }
            

            DrawGUI();
            serializedObject.ApplyModifiedProperties();
        }

        void ArrayAddElementUI(SerializedProperty arrayProperty, bool allowEditing = true)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (allowEditing)
            {
                if (GUILayout.Button("+", GUILayout.Width(20)))
                    arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            }
            
            
            EditorGUILayout.EndHorizontal();
        }
        
        void ArrayGUI<T>(SerializedProperty arrayProperty, bool allowEditing = true, Func<T,bool> predicate = null,
            Action emptyCallback = null)
        {
            if (arrayProperty.arraySize == 0)
            {
                emptyCallback?.Invoke();
            }
            else
            {
                for (int i = 0; i < arrayProperty.arraySize; ++i)
                {
                    var element = arrayProperty.GetArrayElementAtIndex(i);

                    if (predicate != null)
                    {
                        if(!predicate.Invoke((T) element.boxedValue))
                            continue;
                    }
                
                    using var hScope = new EditorGUILayout.HorizontalScope();
                    using (new EditorGUI.DisabledScope(!allowEditing))
                    {
                        EditorGUILayout.PropertyField(element);
                    }
                
                    if (allowEditing && GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        arrayProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }
            
            ArrayAddElementUI(arrayProperty, allowEditing);
        }

        void DrawHeaderGUI()
        {
            Color c = GUI.backgroundColor;
            GUI.backgroundColor = headerBgColor;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_armatureAssetProp, GUIContent.none, GUILayout.Height(25));

            if (_armatureAssetProp.objectReferenceValue)
            {
                bool settingsButtonPressed = EditorGUILayout.DropdownButton(
                    CRStyles.SettingsIcon, FocusType.Keyboard,
                    CRStyles.ToolbarButton(25),
                    GUILayout.Width(25), GUILayout.Height(25));
                if (settingsButtonPressed)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Reset to Bind Pos"), false, SnapToBindPos);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Duplicate Asset"), false, DuplicateAsset);
                    CreateSettingsItems(menu);
                    menu.ShowAsContext();
                }
            }
            else
            {
                bool createClicked = GUILayout.Button("Create New Asset",
                    CRStyles.ToolbarButton(25), GUILayout.Width(120));
                
                if (createClicked)
                {
                    ArmatureRoot root = target as ArmatureRoot;
                    if (!root)
                        return;
                    
                    ArmatureAsset asset = ArmatureAsset.Create(root.transform);
                    _armatureAssetProp.objectReferenceValue = asset;
                    
                    AssetDatabase.CreateAsset(asset, $"Assets/{asset.name}.asset");
                    AssetDatabase.SaveAssets();
                }
            }

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = c;
        }

        protected virtual void DrawGUI()
        {
            
        }

        protected virtual void CreateSettingsItems(GenericMenu menu)
        {
            //menu.AddSeparator("");
        }

        protected bool ShowDestructiveDialog()
        {
            return EditorUtility.DisplayDialog("Destructive Action",
                "This action is destructive. Are you sure you want to continue?", 
                "Yes", "No");
        }
        
        void SnapToBindPos()
        {
            bool result = ShowDestructiveDialog();
            if(!result)
                return;
            
            var armature = target as ArmatureRoot;
            if (armature != null)
            {
                armature.SetToBindPos();
                EditorUtility.SetDirty(armature);
            }
        }

        void DuplicateAsset()
        {
        }
    }
}