using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomPropertyDrawer(typeof(CachedBinding))]
    public class CachedBindingPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;
            return EditorGUI.GetPropertyHeight(property)
                   - EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var bindingProperty = property.FindPropertyRelative("binding");
            position.height = EditorGUI.GetPropertyHeight(bindingProperty);
            
            EditorGUI.PropertyField(position, bindingProperty, new GUIContent(property.propertyPath), true);
            
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}