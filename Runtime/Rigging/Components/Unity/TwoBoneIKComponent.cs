using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Unity/Inverse Kinematics/Two-Bone IK")]
    
    // Use this to serialize data across changed Class names
    //[MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
    //    sourceNamespace: "ControlRigging", 
    //    sourceClassName: "InverseKinematicsComponent")]
    
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "TwoBoneIKComponent")]
    public class TwoBoneIKComponent : CustomRigComponent
    {
        public IKBinding[] bindings;
        
        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            foreach (var b in bindings)
            {
                GameObject constraintObject = gameObject.CreateChild($"IK_{b.targetBone.boneName}");
                var constraint = constraintObject.AddComponent<TwoBoneIKConstraint>();
                
                constraint.data.root = rig.GetBone(b.rootBone);
                constraint.data.mid = rig.GetBone(b.midBone);
                constraint.data.tip = rig.GetBone(b.tipBone);
            
                constraint.data.target = rig.GetBone(b.targetBone);
                var hintBone = rig.GetBone(b.hintBone);
                if (hintBone)
                {
                    constraint.data.hint = hintBone;
                    constraint.data.hintWeight = 1;
                }

                constraint.data.maintainTargetRotationOffset = true;
            }
            
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            gameObject.Destroy();
        }
    }


}