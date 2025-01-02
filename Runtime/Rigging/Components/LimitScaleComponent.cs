using System;
using ControlRigging.Constraints;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Control Rigging/Limits/Limit Scale")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "LimitScaleComponent")]
    public class LimitScaleComponent : CustomRigComponent
    {
        public ScaleLimitBinding[] bindings;

        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var binding in bindings)
            {
                string boneName = binding.bone.boneName;
                string goName = $"ScaleLimit_{boneName}";
                GameObject constraintObject = gameObject.CreateChild(goName);
                var constraint = constraintObject.AddComponent<LimitScaleConstraint>();
                
                constraint.data.ConstrainedTransform = rig.GetBone(binding.bone);
                constraint.data.LimitMin = binding.limitMin;
                constraint.data.LimitMax = binding.limitMax;
                constraint.data.Minimum = binding.minimum;
                constraint.data.Maximum = binding.maximum;
            }
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var c in gameObject.GetComponentsInChildren<LimitScaleConstraint>())
            {
                GameObject go = c.gameObject;
                c.Destroy();
                go.Destroy();
            }
            
            gameObject.Destroy();
        }
    }
}