using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Scripting.APIUpdating;

namespace ControlRigging
{
    [Serializable]
    [RigComponentMenu("Unity/Multi-Rotation Constraint")]
    [MovedFrom(true, sourceAssembly: "Assembly-CSharp", 
        sourceNamespace: "ControlRigging", 
        sourceClassName: "MultiRotationComponent")]
    public class MultiRotationComponent : CustomRigComponent
    {
        [SerializeField]
        private MultiRotationBinding m_Binding;
        public MultiRotationBinding Binding
        {
            get => m_Binding;
            set => m_Binding = value;
        }
        
        protected override void OnGenerateRig(RigRoot rig, GameObject gameObject)
        {
            if(Binding == null) 
                return;
            
            Transform constrainedObject = rig.GetBone(Binding.bone);
            var constraint = gameObject.AddComponent<MultiRotationConstraint>();
            
            constraint.data.constrainedObject = constrainedObject;

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
        }

        protected override void OnDestroyRig(RigRoot rig, GameObject gameObject)
        {
            MultiRotationConstraint constraint = gameObject.GetComponent<MultiRotationConstraint>();
            if (constraint)
                constraint.Destroy();
            
            gameObject.Destroy();
        }
    }
}