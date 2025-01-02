using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    [CustomPropertyDrawer(typeof(ArmatureBinding))]
    public class ArmatureBindingPropertyDrawer : PropertyDrawer
    {
        private ArmatureAsset[] assets;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect pos = new Rect(position);
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
            
            pos.x += labelRect.width;
            //position.y += labelRect.height;
            pos.width -= labelRect.width;

            SerializedProperty armatureProperty = property.FindPropertyRelative("armature");
            Rect armatureRect = new Rect(pos.x, pos.y, (pos.width / 2) - 5, EditorGUIUtility.singleLineHeight);
            //var a = DropdownSelectArmature(armatureRect, armatureProperty);
            ArmatureAsset armatureAsset = (ArmatureAsset) armatureProperty.objectReferenceValue;
            string assetName = armatureAsset ? armatureAsset.name : "None";
            
            if (GUI.Button(armatureRect, assetName, EditorStyles.toolbarPopup))
            {
                PopupWindow.Show(armatureRect, new ArmatureSelectPopupWindow(armatureProperty));
            }
            
            property.serializedObject.ApplyModifiedProperties();
            
            pos.x += armatureRect.width;
            pos.width -= armatureRect.width;

            Color c = GUI.backgroundColor;
            ArmatureBinding binding = (ArmatureBinding) property.boxedValue;
            if (binding.armature)
                GUI.backgroundColor = binding.IsValid ? Color.cyan : Color.red;
            
            SerializedProperty boneNameProp = property.FindPropertyRelative("boneName");
            Rect boneRect = new Rect(pos.x + 5, pos.y, pos.width - 10, EditorGUIUtility.singleLineHeight);
                
            string bName = boneNameProp.stringValue;
            if (string.IsNullOrEmpty(bName))
                bName = "None";
            
            DropdownSelectBoneFrom(armatureAsset, boneRect, boneNameProp, new GUIContent(bName));

            GUI.backgroundColor = c;

        }
        
        void DropdownSelectBoneFrom(ArmatureAsset armatureAsset, Rect position, SerializedProperty property, GUIContent content)
        {
            EditorGUI.BeginDisabledGroup(!armatureAsset);
            bool isDropdown = EditorGUI.DropdownButton(position, content, FocusType.Keyboard, EditorStyles.toolbarPopup);
            EditorGUI.EndDisabledGroup();
            
            if (!isDropdown)
                return;

            string value = property.stringValue;
            
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("None"), string.IsNullOrEmpty(value), SelectBone, new StringMenuSelection()
            {
                Property = property,
                Value = null
            });
            
            menu.AddSeparator("");
            
            var boneNames = GetBoneNames(armatureAsset)?.OrderBy(s => s);
            if (boneNames != null)
            {
                foreach (var bName in boneNames)
                {
                    bool isSelected = value != null && value.Equals(bName);
                    
                    menu.AddItem(new GUIContent(bName), isSelected, SelectBone, new StringMenuSelection()
                    {
                        Property = property,
                        Value = bName
                    });
                }
            }
            
            menu.DropDown(position);
        }
        
        string[] GetBoneNames(ArmatureAsset armatureAsset)
        {
            if (!armatureAsset)
                return null;

            string[] boneNames = new string[armatureAsset.Bones.Length];
            for (int i = 0; i < armatureAsset.Bones.Length; ++i)
            {
                Bone bone = armatureAsset.Bones[i];
                if (bone == null)
                    continue;
                
                boneNames[i] = bone.name;
            }

            return boneNames;
        }
        
        void SelectBone(object o)
        {
            var selection = (StringMenuSelection) o;
            selection.Set();
        }
    }
}