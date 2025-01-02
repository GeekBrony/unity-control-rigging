using System;
using ControlRigging.Constraints;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Control Rigging/Limits/Limit Distance")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "LimitDistanceComponent")]
    public class LimitDistanceComponent : CustomRigComponent
    {
        public DistanceLimitBinding[] bindings;

        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var binding in bindings)
            {
                string boneName = binding.bone.boneName;
                string goName = $"DistanceLimit_{boneName}";
                GameObject constraintObject = gameObject.CreateChild(goName);
                var constraint = constraintObject.AddComponent<LimitDistanceConstraint>();
                
                constraint.data.ConstrainedTransform = rig.GetBone(binding.bone);
                constraint.data.TargetTransform = rig.GetBone(binding.target);
                constraint.data.Offset = binding.offset;
                constraint.data.Distance = binding.distance;
                constraint.data.Mode = binding.mode;
            }
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var c in gameObject.GetComponentsInChildren<LimitDistanceConstraint>())
            {
                GameObject go = c.gameObject;
                c.Destroy();
                go.Destroy();
            }
            
            gameObject.Destroy();
        }
    }
}