using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace ControlRigging
{
    [CustomEditor(typeof(RigRoot))]
    public class RigRootEditor : Editor
    {
        private SerializedProperty _rigAssetProp;
        private SerializedProperty _showDebugMenuProp;
        private SerializedProperty _hideConstraintsProp;
        public RigRoot Root => target as RigRoot;

        private List<SerializedObject> RigConstraints = new List<SerializedObject>();

        private void OnEnable()
        {
            _rigAssetProp = serializedObject.FindProperty("asset");

            _showDebugMenuProp = serializedObject.FindProperty("showDebugMenu");
            _hideConstraintsProp = serializedObject.FindProperty("hideConstraints");

            RefreshConstraints();
        }

        void RefreshConstraints()
        {
            foreach (var constraint in RigConstraints.ToArray())
                constraint.Dispose();

            RigConstraints.Clear();

            if (Root.generated && Root.constraintRoot)
            {
                var constraintRoot = Root.constraintRoot;
                foreach (var behaviour in constraintRoot.GetComponentsInChildren<MonoBehaviour>())
                {
                    if (behaviour is not IRigConstraint)
                        continue;

                    RigConstraints.Add(new SerializedObject(behaviour));
                }
            }
        }

        [SerializeField] private int toolBarSelection = 0;

        private readonly Color headerBgColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //base.OnInspectorGUI();
            Color c = GUI.backgroundColor;
            GUI.backgroundColor = headerBgColor;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(_rigAssetProp, GUIContent.none, GUILayout.Height(25));

                bool settingsButtonPressed = EditorGUILayout.DropdownButton(
                    CRStyles.SettingsIcon, FocusType.Keyboard,
                    CRStyles.ToolbarButton(25),
                    GUILayout.Width(25), GUILayout.Height(25));

                if (settingsButtonPressed)
                {
                    GenericMenu menu = new GenericMenu();
                    CreateSettingsItems(menu);
                    menu.ShowAsContext();
                }
            }

            GUI.backgroundColor = c;

            DrawGenerateGUI();

            if (RigConstraints.Any())
            {
                EditorGUILayout.Separator();

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.BeginHorizontal();
                    toolBarSelection = GUILayout.Toolbar(toolBarSelection,
                        new[] { "Weights", "Effectors" },
                        GUILayout.Height(24));

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();

                    switch (toolBarSelection)
                    {
                        case 0:
                            WeightsGUI();
                            break;
                        case 1:
                            EffectorsGUI();
                            break;
                    }
                }

                EditorGUILayout.Separator();
                DrawDebugMenuGUI();

            }

            serializedObject.ApplyModifiedProperties();
        }

        void WeightsGUI()
        {
            EditorGUILayout.LabelField($"Weights", EditorStyles.largeLabel);
            foreach (var o in RigConstraints.ToArray())
            {
                if (o == null)
                    continue;

                o.Update();

                using (new EditorGUILayout.HorizontalScope())
                {
                    var weightProperty = o.FindProperty("m_Weight");
                    if (weightProperty.isAnimated)
                    {
                        EditorGUILayout.LabelField(CRIcons.AnimatedValueIcon, GUILayout.Width(16));
                    }

                    EditorGUILayout.LabelField(o.targetObject.name, EditorStyles.miniLabel);
                    EditorGUILayout.PropertyField(weightProperty, GUIContent.none);
                }

                o.ApplyModifiedProperties();
            }
        }

        void EffectorsGUI()
        {
            EditorGUILayout.LabelField($"Effectors", EditorStyles.largeLabel);

            bool anyVisible = Root.rig.effectors.Any(e => e.visible);
            bool allVisible = EditorGUILayout.ToggleLeft("Everything", anyVisible);
            if (anyVisible != allVisible)
            {
                foreach (var effector in Root.rig.effectors)
                {
                    if (effector == null)
                        continue;

                    effector.visible = allVisible;
                }
            }

            foreach (var effector in Root.rig.effectors)
            {
                if (effector == null || !effector.transform)
                    continue;

                using (new EditorGUILayout.HorizontalScope())
                {
                    effector.visible = EditorGUILayout.ToggleLeft(effector.transform.name, effector.visible);
                }
            }
        }

        void DrawGenerateGUI()
        {
            if (!_rigAssetProp.objectReferenceValue)
                return;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(Root.generated))
                {
                    if (GUILayout.Button("Generate", GUILayout.Height(25)))
                    {
                        GenerateRig();
                    }
                }

                using (new EditorGUI.DisabledScope(!Root.generated))
                {
                    if (GUILayout.Button("Clear", GUILayout.Height(25)))
                        ClearRig();
                }
            }
        }

        void DrawDebugMenuGUI()
        {
            _showDebugMenuProp.boolValue =
                EditorGUILayout.BeginFoldoutHeaderGroup(_showDebugMenuProp.boolValue, "Debug");
            if (_showDebugMenuProp.boolValue)
            {
                EditorGUILayout.PropertyField(_hideConstraintsProp, new GUIContent("Hide Constraints"));
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        protected void CreateSettingsItems(GenericMenu menu)
        {
            if (!Root) return;

            menu.AddItem(new GUIContent("Validate Hierarchy"), false, ValidateHierarchy);
            menu.AddSeparator("");
            if (Root.generated)
            {
                menu.AddDisabledItem(new GUIContent("Generate"), false);
                menu.AddItem(new GUIContent("Clear"), false, ClearRig);
            }
            else
            {
                menu.AddItem(new GUIContent("Generate"), false, GenerateRig);
                menu.AddDisabledItem(new GUIContent("Clear"), false);
            }
        }

        private void ValidateHierarchy()
        {
            if (!Root)
                return;

            if (Root.ValidateHierarchy())
                Debug.Log("The rig hierarchy is valid! :)");

            RefreshConstraints();
        }

        void GenerateRig()
        {
            if (!Root) return;
            //if (!ShowDestructiveDialog()) return;

            Root.GenerateRig();

            RefreshConstraints();
        }

        void ClearRig()
        {
            if (!Root) return;
            if (!ShowDestructiveDialog()) return;
            Root.ClearRig();

            RefreshConstraints();
        }

        protected bool ShowDestructiveDialog()
        {
            return EditorUtility.DisplayDialog("Destructive Action",
                "This action is destructive. Are you sure you want to continue?",
                "Yes", "No");
        }
    }
}