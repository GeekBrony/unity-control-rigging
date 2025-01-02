using UnityEditor;
using UnityEngine;

namespace ControlRigging
{
    public static class CRStyles
    {
        public static GUIStyle ToolbarButton(int height = 20, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            var style = new GUIStyle(EditorStyles.toolbarButton)
            {
                fixedHeight = height,
                alignment = alignment
            };
            return style;
        }

        public static GUIContent SettingsIcon => CRIcons.SettingsIcon;
    }

    
}