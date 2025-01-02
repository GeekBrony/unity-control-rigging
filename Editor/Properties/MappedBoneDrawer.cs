using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomPropertyDrawer(typeof(MappedBone))]
    public class MappedBoneDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // base.OnGUI(position, property, label);
            var boneNameProp = property.FindPropertyRelative("boneName");
            var transformProp = property.FindPropertyRelative("transform");
            
            Rect boneNameLabelPos = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            EditorGUI.LabelField(boneNameLabelPos, new GUIContent(boneNameProp.stringValue));
            
            Rect transformPropPos = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 
                    position.width - boneNameLabelPos.width, position.height);
            EditorGUI.PropertyField(transformPropPos, transformProp, GUIContent.none);
        }
    }
}