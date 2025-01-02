using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    public class ArmatureSelectPopupWindow : PopupWindowContent
    {
        private SerializedProperty _property;
        private ArmatureAsset[] _assets;
        
        public ArmatureSelectPopupWindow(SerializedProperty property)
        {
            _property = property;
            _assets = GetAssetsOfType<ArmatureAsset>();
        }
        
        
        public override void OnGUI(Rect rect)
        {
            _property.serializedObject.Update();
            
            bool selected = false;
            int selection = -1;
            if (GUILayout.Button("None", ButtonStyle))
                selected = true;
            
            for(int i = 0; i < _assets.Length; ++i)
            {
                ArmatureAsset a = _assets[i];
                if (GUILayout.Button(a.name, ButtonStyle))
                {
                    selection = i;
                    selected = true;
                }
            }

            if (!selected)
                return;
            
            _property.objectReferenceValue = selection == -1 ? null : _assets[selection];
            _property.serializedObject.ApplyModifiedProperties();
            
            editorWindow.Close();
        }
        
        T[] GetAssetsOfType<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", null);
            return guids.Select(s => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(s)))
                .ToArray();
        }
        
        private GUIStyle ButtonStyle => EditorStyles.toolbarButton;
        
        public override Vector2 GetWindowSize()
        {
            Vector2 ws = base.GetWindowSize();
            float singleHeight = ButtonStyle.CalcHeight(new GUIContent(" "), ws.x);
            
            return new Vector2(ws.x, singleHeight*(_assets.Length+1));
        }
    }
}