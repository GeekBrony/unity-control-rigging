using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Unity/Multi-Aim (Look At)")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "LookAtComponent")]
    public class LookAtComponent : CustomRigComponent
    {
        [SerializeField]
        private AimBinding m_Binding;
        public AimBinding Binding
        {
            get => m_Binding;
            set => m_Binding = value;
        }
        
        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            if(Binding == null) 
                return;
            
            Transform constrainedObject = rig.GetBone(Binding.bone);
            var constraint = gameObject.AddComponent<MultiAimConstraint>();
            
            constraint.data.constrainedObject = constrainedObject;
            constraint.data.aimAxis = (MultiAimConstraintData.Axis) Binding.aimAxis;
            constraint.data.upAxis = (MultiAimConstraintData.Axis) Binding.upAxis;
            constraint.data.worldUpType = Binding.worldUpType;
            constraint.data.worldUpAxis = Binding.worldUpAxis;

            if (Binding.useWorldUpBinding)
            {
                constraint.data.worldUpObject = rig.GetBone(Binding.worldUpBinding);
            }
            else
            {
                constraint.data.worldUpObject = gameObject.transform;
            }

            var data = new WeightedTransformArray(Binding.targets.Length);
            for (int i = 0; i < Binding.targets.Length; ++i)
            {
                var target = Binding.targets[i];
                data.SetTransform(i, rig.GetBone(target.target));
                data.SetWeight(i, target.weight);
                constraint.data.sourceObjects = data;
            }

            constraint.data.constrainedXAxis = Binding.constrainedAxes.x;
            constraint.data.constrainedYAxis = Binding.constrainedAxes.y;
            constraint.data.constrainedZAxis = Binding.constrainedAxes.z;
            constraint.data.limits = new Vector2(Binding.minLimit, Binding.maxLimit);
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            MultiAimConstraint multiAimConstraint = gameObject.GetComponent<MultiAimConstraint>();
            if (multiAimConstraint)
                multiAimConstraint.Destroy();
            
            gameObject.Destroy();
        }
    }
}