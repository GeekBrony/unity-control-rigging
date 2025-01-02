using System;
using ControlRigging.Constraints;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Control Rigging/Limits/Limit Position")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "LimitPositionComponent")]
    public class LimitPositionComponent : CustomRigComponent
    {
        public PositionLimitBinding[] bindings;

        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var binding in bindings)
            {
                string boneName = binding.bone.boneName;
                string goName = $"PositionLimit_{boneName}";
                GameObject constraintObject = gameObject.CreateChild(goName);
                var constraint = constraintObject.AddComponent<LimitPositionConstraint>();
                
                constraint.data.ConstrainedTransform = rig.GetBone(binding.bone);
                
                constraint.data.LimitMin = binding.limitMin;
                constraint.data.LimitMax = binding.limitMax;
                constraint.data.Minimum = binding.minimum;
                constraint.data.Maximum = binding.maximum;
                constraint.data.space = binding.space;
            }
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var c in gameObject.GetComponentsInChildren<LimitPositionConstraint>())
            {
                GameObject go = c.gameObject;
                c.Destroy();
                go.Destroy();
            }
            
            gameObject.Destroy();
        }
    }
}