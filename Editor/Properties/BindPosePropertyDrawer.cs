using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomPropertyDrawer(typeof(BindPose))]
    public class BindPosePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginDisabledGroup(true);
            
            Rect drawPos = new Rect(position);
            Rect positionRect = new Rect(drawPos.x, drawPos.y, drawPos.width, EditorGUIUtility.singleLineHeight);
            
            var positionProperty = property.FindPropertyRelative("localPosition");
            
            DrawVec3(positionRect, new GUIContent("Position"), positionProperty);

            drawPos.y += positionRect.height;
            Rect rotationRect = new Rect(drawPos.x, drawPos.y, drawPos.width, EditorGUIUtility.singleLineHeight);
            
            var rotationProperty = property.FindPropertyRelative("localRotation");
            
            DrawQuaternionAsVec3(rotationRect, new GUIContent("Rotation"), rotationProperty);
            
            drawPos.y += positionRect.height;
            Rect scaleRect = new Rect(drawPos.x, drawPos.y, drawPos.width, EditorGUIUtility.singleLineHeight);
            
            var scaleProperty = property.FindPropertyRelative("localScale");
            
            DrawVec3(scaleRect, new GUIContent("Scale"), scaleProperty);
            
            EditorGUI.EndDisabledGroup();
        }

        void DrawVec3(Rect rect, GUIContent label, SerializedProperty property)
        {
            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.BeginChangeCheck();
            
            Vector3 pos = EditorGUI.Vector3Field(rect, label, property.vector3Value);
            
            if (EditorGUI.EndChangeCheck())
                property.vector3Value = pos;
            
            EditorGUI.EndProperty();
        }
        
        void DrawQuaternionAsVec3(Rect rect, GUIContent label, SerializedProperty property)
        {
            Vector3 euler = property.quaternionValue.eulerAngles;
            
            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.BeginChangeCheck();
            
            euler = EditorGUI.Vector3Field(rect, label, euler);
            
            if (EditorGUI.EndChangeCheck())
                property.quaternionValue = Quaternion.Euler(euler);
            
            EditorGUI.EndProperty();
        }
    }
}