using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Unity/Inverse Kinematics/FABRIK Chain")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "FABRIKChainComponent")]
    public class FABRIKChainComponent : CustomRigComponent
    {
        public FABRIKChainBinding binding;
        
        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            var constraint = gameObject.AddComponent<ChainIKConstraint>();
                
            constraint.data.root = rig.GetBone(binding.rootBone);
            constraint.data.tip = rig.GetBone(binding.tipBone);
            constraint.data.target = rig.GetBone(binding.targetBone);

            constraint.data.maxIterations = binding.maxIterations;
            constraint.data.tolerance = binding.tolerance;
            constraint.data.chainRotationWeight = binding.chainRotationWeight;
            constraint.data.tipRotationWeight = binding.tipRotationWeight;
            
            constraint.data.maintainTargetRotationOffset = true;
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            var constraint = gameObject.GetComponent<ChainIKConstraint>();
            if (constraint)
                constraint.Destroy();
            
            gameObject.Destroy();
        }
    }


}