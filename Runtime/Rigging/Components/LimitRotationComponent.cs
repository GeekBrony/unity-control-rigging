using System;
using ControlRigging.Constraints;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Control Rigging/Limits/Limit Rotation")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "LimitRotationComponent")]
    public class LimitRotationComponent : CustomRigComponent
    {
        public RotationLimitBinding[] bindings;

        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var binding in bindings)
            {
                string boneName = binding.bone.boneName;
                string axisString = binding.axis.ToFormattedString();
                string goName = $"RotationLimit_{boneName}_{axisString}";
                GameObject constraintObject = gameObject.CreateChild(goName);
                var constraint = constraintObject.AddComponent<LimitRotationConstraint>();
                
                constraint.data.ConstrainedTransform = rig.GetBone(binding.bone);
                constraint.data.ConstrainedAxis = binding.axis;
                constraint.data.Limit = binding.limit;
            }
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var c in gameObject.GetComponentsInChildren<LimitRotationConstraint>())
            {
                GameObject go = c.gameObject;
                c.Destroy();
                go.Destroy();
            }
            
            gameObject.Destroy();
        }
    }
}