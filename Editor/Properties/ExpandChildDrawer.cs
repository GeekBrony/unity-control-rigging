using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomPropertyDrawer(typeof(ExpandChildAttribute))]
    public class ExpandChildDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            property.isExpanded = true;
            return EditorGUI.GetPropertyHeight(property)
                   - EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var endProperty = property.GetEndProperty();
            var childProperty = property.Copy();
            childProperty.NextVisible(true);
            while (!SerializedProperty.EqualContents(childProperty, endProperty))
            {
                position.height = EditorGUI.GetPropertyHeight(childProperty);
                OnChildPropertyGUI(position, childProperty);
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                childProperty.NextVisible(false);
            }
        }

        protected virtual void OnChildPropertyGUI(Rect position, SerializedProperty childProperty)
        {
            EditorGUI.PropertyField(position, childProperty, true);
        }
    }
}