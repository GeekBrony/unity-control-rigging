using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace ControlRigging
{
    [Overlay(typeof(SceneView), OverlayID, ToolbarDisplayName, "", true)]
    internal class ControlContextOverlay : ToolbarOverlay
    {
        private const string OverlayID = "ControlContextOverlay";
        private const string ToolbarDisplayName = "Rig Controls";

        public ControlContextOverlay() : base(ControlContextToolbarItem.id)
        {
            
        }

        protected override Layout supportedLayouts
        {
            get
            {
                return Layout.HorizontalToolbar;
            }
        }
    }
    
    [EditorToolbarElement(id, typeof(SceneView))]
    class ControlContextToolbarItem : OverlayToolbar
    {
        public const string id = "ControlContextToolbarItem";
        
        public ControlContextToolbarItem()
        {
            var icon = EditorGUIUtility.IconContent("AvatarMask On Icon");
            Add(new ControlContextToggle(icon));
            SetupChildrenAsButtonStrip();
        }
    }

    [EditorToolbarElement(id, typeof(SceneView))]
    class ControlContextToggle : EditorToolbarToggle
    {
        public const string id = "ControlContextToggle";
        public ControlContextToggle(GUIContent iconContent) : base((Texture2D)iconContent.image)
        {
            this.RegisterValueChangedCallback(Test);
        }

        void Test(ChangeEvent<bool> evt)
        {
            ToggleRigControls(evt.newValue);
        }

        void ToggleRigControls(bool enable)
        {
            foreach (var root in Object.FindObjectsByType<RigRoot>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                foreach (var effector in root.rig.effectors)
                {
                    if (effector == null)
                        continue;

                    effector.visible = enable;
                }
            }
        }
    }
}