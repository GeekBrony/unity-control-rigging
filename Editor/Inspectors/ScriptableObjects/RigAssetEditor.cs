using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomEditor(typeof(RigAsset))]
    public class RigAssetEditor : Editor
    {
        private SerializedProperty _effectorsProp;
        private SerializedProperty _componentsProp;
        private SerializedProperty _transformsProp;

        private RigAsset Asset => target as RigAsset;

        private void OnEnable()
        {
            _effectorsProp = serializedObject.FindProperty("effectors");
            _transformsProp = serializedObject.FindProperty("transforms");
            _componentsProp = serializedObject.FindProperty("components");
        }

        [SerializeField]
        private int toolBarSelection = 0;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.BeginHorizontal();
            
            toolBarSelection = GUILayout.Toolbar(toolBarSelection, 
                new[] { "Components", "Effectors", "Transforms" }, 
                GUILayout.Height(24));
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            
            switch (toolBarSelection)
            {
                case 0: ComponentsGUI(); 
                    break;
                case 1: EffectorsGUI(); 
                    break;
                case 2: TransformsGUI(); 
                    break;
            }
            
            serializedObject.ApplyModifiedProperties();
        }
        
        void EffectorsGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(_effectorsProp);
            ArrayGUI(_effectorsProp);
            serializedObject.ApplyModifiedProperties();
        }

        void TransformsGUI()
        {
            serializedObject.Update();
            ArrayGUI(_transformsProp);
            serializedObject.ApplyModifiedProperties();
        }

        void ArrayGUI(SerializedProperty arrayProperty)
        {
            for (int i = 0; i < arrayProperty.arraySize; ++i)
            {
                var element = arrayProperty.GetArrayElementAtIndex(i);
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    using var indent = new EditorGUI.IndentLevelScope(1);
                    EditorGUILayout.PropertyField(element);
                    
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        arrayProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("+", GUILayout.Width(20)))
                arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            
            EditorGUILayout.EndHorizontal();
        }
        
        void ComponentsGUI()
        {
            string componentsLabelString = "Rig Components";
            if (_componentsProp.arraySize > 0)
            {
                componentsLabelString += $" ({_componentsProp.arraySize})";
            }
                
            _componentsProp.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(
                _componentsProp.isExpanded, componentsLabelString, EditorStyles.foldoutHeader);
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();
            
            if(!_componentsProp.isExpanded)
                return;

            serializedObject.Update();
            
            Color c = GUI.backgroundColor;
            GUI.backgroundColor = Color.black * 2f;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUI.backgroundColor = c;
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (_componentsProp.isExpanded)
                {
                    GUILayout.Space(5);
                    GUI.backgroundColor = Color.green;
                    ComponentDropdown(new GUIContent("Add Component"), GUILayout.Width(114));
                    GUI.backgroundColor = c;
                }
            
                
                EditorGUILayout.EndHorizontal();

                if (_componentsProp.isExpanded)
                    ComponentsListGUI();
                
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private int renamingIndex = -1;
        void ComponentsListGUI()
        {
            for (int i = 0; i < _componentsProp.arraySize; i++)
            {
                EditorGUI.indentLevel++;
                Color c = GUI.backgroundColor;
                
                GUI.backgroundColor = Color.grey * 2f;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = c;
                EditorGUILayout.BeginHorizontal();
                
                SerializedProperty componentProp = _componentsProp.GetArrayElementAtIndex(i);
                var componentValue = componentProp.boxedValue;
                IRigComponent component = componentValue as IRigComponent;
                string componentName = GetComponentName(componentProp, component);
                
                using (var scope = new EditorGUILayout.HorizontalScope())
                {
                    Rect rect = scope.rect;

                    Rect foldoutRect = new Rect(rect.x, rect.y, 10, 20);
                    componentProp.isExpanded =
                        EditorGUI.Foldout(foldoutRect, componentProp.isExpanded, GUIContent.none);

                    rect.width -= foldoutRect.width;
                    rect.x += foldoutRect.width-5;
                        
                    Rect toggleRect = new Rect(rect.x, rect.y, 30, 20);
                    
                    if (component != null)
                        component.Enabled = EditorGUI.Toggle(toggleRect, component.Enabled);
                        
                    rect.width -= toggleRect.width-18;
                    rect.x += toggleRect.width+5;
                        
                    Rect labelRect = new Rect(rect.x, rect.y, rect.width - 50, 18);

                    bool isRenaming = renamingIndex == i;
                    
                    if (!isRenaming)
                    {
                        if (GUI.Button(labelRect, new GUIContent(componentName), EditorStyles.boldLabel))
                        {
                            componentProp.isExpanded = !componentProp.isExpanded;
                        }
                    }
                    else
                    {
                        labelRect.x -= 16;
                        labelRect.y += 2;
                        GUI.SetNextControlName($"ComponentName[{i}]");
                        component.Name = EditorGUI.DelayedTextField(labelRect, GUIContent.none, component.Name);
                        if(!EditorGUIUtility.editingTextField)
                        {
                            renamingIndex = -1;
                        }
                    }
                    
                   
                }
                
                GUILayout.Space(4);

                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(22 * 3)))
                {
                    if (GUILayout.Button(CRIcons.Pencil, EditorStyles.toolbarButton, GUILayout.Width(22)))
                    {
                        renamingIndex = i;
                        
                        EditorGUI.FocusTextInControl($"ComponentName[{i}]");
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }
                    
                    using (new EditorGUI.DisabledScope(i == 0))
                    {
                        if (GUILayout.Button("\u25b2", EditorStyles.toolbarButton, GUILayout.Width(22)))
                        {
                            _componentsProp.MoveArrayElement(i, i - 1);
                            serializedObject.ApplyModifiedProperties();
                            return;
                        }
                    }
                    
                    using (new EditorGUI.DisabledScope(i == _componentsProp.arraySize - 1))
                    {
                        if (GUILayout.Button("\u25bc", EditorStyles.toolbarButton, GUILayout.Width(22)))
                        {
                            _componentsProp.MoveArrayElement(i, i + 1);
                            serializedObject.ApplyModifiedProperties();
                            return;
                        }
                    }
                }
                
                GUILayout.Space(4);
                
                GUI.backgroundColor = new Color(1,0.5f,0.5f) * 1.5f;
                if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(25)))
                {
                    if(!ShowDestructiveDialog())
                        return;
                    
                    _componentsProp.DeleteArrayElementAtIndex(i);
                    return;
                }
                GUI.backgroundColor = c;
                
                EditorGUILayout.EndHorizontal();
                
                if (component == null)
                {
                    EditorGUILayout.HelpBox($"Component \"{componentName}\" not found.", MessageType.Error);
                }
                
                if (component != null && componentProp.isExpanded)
                {
                    EditorGUILayout.Separator();
                    using (EditorGUILayout.HorizontalScope scope = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(2);
                        EditorGUILayout.PropertyField(componentProp, true);
                        GUILayout.Space(2);
                    }
                }
                
                EditorGUILayout.EndVertical();
                    
                EditorGUI.indentLevel--;
            }
        }

        string GetComponentName(SerializedProperty property, IRigComponent component)
        {
            if (component != null)
            {
                if (!string.IsNullOrEmpty(component.Name))
                    return component.Name;
                
                var attr = RigComponentMenuAttribute.GetAttribute(component.GetType());
                if (attr != null)
                    return Path.GetFileName(attr.ComponentName);

                return "";
            }

            return property.type;
        }
        
        bool ShowDestructiveDialog()
        {
            return EditorUtility.DisplayDialog("Destructive Action",
                "This action is destructive. Are you sure you want to continue?", 
                "Yes", "No");
        }

        void ComponentDropdown(GUIContent label, params GUILayoutOption[] options)
        {
            bool dropDown = EditorGUILayout.DropdownButton(label, FocusType.Keyboard, EditorStyles.popup, options);
            if(!dropDown)
                return;

            GenericMenu menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("Select a Rig Component"));
            menu.AddSeparator("");
            
            var types = GetSubclassesAsTypes<CustomRigComponent>().OrderBy(type =>
            {
                var attr = RigComponentMenuAttribute.GetAttribute(type);
                string n = type.Name;
                if (attr != null)
                    n = attr.ComponentName;
                return n;
            });
            
            foreach (var type in types)
            {
                var attr = RigComponentMenuAttribute.GetAttribute(type);
                
                string componentName = type.Name;
                if (attr != null)
                    componentName = attr.ComponentName;
                
                GUIContent item = new GUIContent(componentName);
                menu.AddItem(item, false, SelectComponent, type);
            }

            menu.ShowAsContext();
        }

        void SelectComponent(object obj)
        {
            Type componentType = (Type) obj;
            
            int index = _componentsProp.arraySize;
            _componentsProp.InsertArrayElementAtIndex(index);
            SerializedProperty componentProp = _componentsProp.GetArrayElementAtIndex(index);
            
            dynamic newComponent = Activator.CreateInstance(componentType);
            componentProp.boxedValue = Convert.ChangeType(newComponent, componentType);
            
            serializedObject.ApplyModifiedProperties();
        }

        private static IEnumerable<Type> GetSubclassesAsTypes<T>() where T : class
        {
            List<Type> objects = new List<Type>();
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes()
                             .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                {
                    objects.Add(type);
                }
            }
            return objects;
        }
    }
}