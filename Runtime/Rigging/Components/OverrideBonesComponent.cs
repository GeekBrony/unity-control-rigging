using System;
using System.Collections.Generic;
using ControlRigging.Constraints;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Control Rigging/Transform/Override Bones")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "OverrideBonesComponent")]
    public class OverrideBonesComponent : CustomRigComponent
    {
        public OverrideBinding binding;
        
        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            var overrideBindings = binding.bindings;
            if (overrideBindings != null && overrideBindings.Length > 0)
            {
                var constraint = gameObject.AddComponent<OverrideBonesConstraint>();
                constraint.data.Transforms = new List<CopyTransformDataSingle>();
            
                for (int i = 0; i < overrideBindings.Length; ++i)
                {
                    var b = overrideBindings[i];
                    var data = new CopyTransformDataSingle()
                    {
                        ConstrainedTransform = rig.GetBone(b.bone),
                        CopiedTransform = rig.GetBone(b.target)
                    };
                    constraint.data.Transforms.Add(data);

                    constraint.data.PositionWeight = binding.positionWeight;
                    constraint.data.RotationWeight = binding.rotationWeight;
                    constraint.data.ScaleWeight = binding.scaleWeight;
                }
            }
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            OverrideBonesConstraint constraint = gameObject.GetComponent<OverrideBonesConstraint>();
            if (constraint)
                constraint.Destroy();
            
            gameObject.Destroy();
        }
    }
}